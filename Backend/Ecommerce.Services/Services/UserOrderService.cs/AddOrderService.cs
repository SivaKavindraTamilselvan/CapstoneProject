using System.Text.Json;
using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

public partial class UserOrderService : IUserOrderService
{
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly EcommerceContext _ecommerceContext;
    private readonly IIdempotencyService _idempotencyService;
    private readonly INotificationService _notificationService;
    private readonly IProductRepsository _productRepsository;
    private readonly ICartItemsRepsository _cartItemsRepsository;
    private readonly IUserCouponService _userCouponService;
    private readonly IAddressRepsository _addressRepsository;
    private readonly IUserCartService _userCartService;
    private readonly IShipRocketService _shipRocketService;
    private readonly IShipmentService _shipmentService;
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private readonly IOrderRepsository _orderRepsository;
    private readonly IMapper _mapper;
    private readonly IShipmentRepsository _shipmentRepsository;
    private readonly ILogger<UserOrderService> _logger;
    private readonly ILogChanges _logChanges;
    private readonly IVendorUserRepsository _vendorUserRepsository;

    public UserOrderService(IVendorUserRepsository vendorUserRepsository,IOrderItemRepsository orderItemRepsository,IAdminUserRepsository adminUserRepsository, EcommerceContext ecommerceContext, IIdempotencyService idempotencyService, INotificationService notificationService, IProductRepsository productRepsository, IShipmentRepsository shipmentRepsository, IOrderRepsository orderRepsository, IPaymentService paymentService, IOrderService orderService, IShipmentService shipmentService, ICartItemsRepsository cartItemsRepsository, IShipRocketService shipRocketService, IUserCartService userCartService, IUserCouponService userCouponService, IAddressRepsository addressRepsository, IMapper mapper, ILogger<UserOrderService> logger, ILogChanges logChanges)
    {
        _vendorUserRepsository = vendorUserRepsository;
        _orderItemRepsository = orderItemRepsository;
        _adminUserRepsository = adminUserRepsository;
        _ecommerceContext = ecommerceContext;
        _idempotencyService = idempotencyService;
        _notificationService = notificationService;
        _productRepsository = productRepsository;
        _shipmentRepsository = shipmentRepsository;
        _orderRepsository = orderRepsository;
        _cartItemsRepsository = cartItemsRepsository;
        _userCouponService = userCouponService;
        _addressRepsository = addressRepsository;
        _userCartService = userCartService;
        _shipRocketService = shipRocketService;
        _shipmentService = shipmentService;
        _orderService = orderService;
        _paymentService = paymentService;
        _mapper = mapper;
        _logger = logger;
        _logChanges = logChanges;
    }

    public async Task<ResponseAddOrderDTO> AddOrder(RequestAddOrderDTO requestAddOrderDTO, int userId, string idempotencyKey)
    {
        _logger.LogInformation("UserId {UserId} initiated order placement. IdempotencyKey {IdempotencyKey}", userId, idempotencyKey);

        var requestBody = JsonSerializer.Serialize(requestAddOrderDTO);

        var (isDuplicate, existing) = await _idempotencyService.TryBeginOperation(idempotencyKey, userId, requestBody);

        if (isDuplicate)
        {
            _logger.LogWarning("Duplicate order request detected for UserId {UserId}, IdempotencyKey {IdempotencyKey}", userId, idempotencyKey);
            if (existing!.IsCompleted)
            {
                _logger.LogInformation("Returning previously completed order response for IdempotencyKey {IdempotencyKey}", idempotencyKey);
                return JsonSerializer.Deserialize<ResponseAddOrderDTO>(existing.ResponseBody!)!;
            }
            _logger.LogWarning("Order placement already in progress for IdempotencyKey {IdempotencyKey}", idempotencyKey);
            throw new InvalidOperationException("Order placement already in progress for this request.");
        }

        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            // validate each and every product before placing order
            var cartItems = await ValidateProduct(userId);
            _logger.LogInformation("Cart items validated for UserId {UserId}. ItemCount {ItemCount}", userId, cartItems.Count);

            // validate the address for the order needed to be placed
            var deliveryAddress = await ValidateAddress(requestAddOrderDTO.AddressId, userId);
            _logger.LogInformation("Delivery AddressId {AddressId} validated for UserId {UserId}", deliveryAddress.AddressId, userId);

            // calculate the total product charge
            decimal productCharge = CalculateProductCharge(cartItems);
            _logger.LogInformation("Calculated ProductCharge {ProductCharge} for UserId {UserId}", productCharge, userId);

            int cod = requestAddOrderDTO.PaymentMethod == 1 ? 1 : 0;

            Coupons? selectedCoupon = null;

            // validate the selected coupoun to avoid in correct coupoun usage
            if (requestAddOrderDTO.CouponId != null)
            {
                selectedCoupon = await ValidateCoupon(userId, requestAddOrderDTO.CouponId.Value);
                _logger.LogInformation("CouponId {CouponId} validated for UserId {UserId}. DiscountValue {DiscountValue}", requestAddOrderDTO.CouponId, userId, selectedCoupon?.DiscountValue);
            }

            // coupon charge is calculated if the coupon is selected
            decimal couponCharge = selectedCoupon?.DiscountValue ?? 0;

            // get the inventory address for each order items
            var selectedItems = await GetTheInventoryPickupAddress(cartItems, deliveryAddress, cod);
            _logger.LogInformation("Resolved inventory pickup addresses for {ItemCount} selected items", selectedItems.Count);

            // group the order items by the vendor and pickup address
            var groupedShipments = selectedItems.GroupBy(x => new
            {
                VendorId = x.CartItem.ProductVariant!.Product!.VendorId,
                PickupAddressId = x.Inventory.AddressId
            }).ToList();

            _logger.LogInformation("Grouped selected items into {ShipmentGroupCount} shipment groups", groupedShipments.Count);

            // shipping changes calculated
            decimal totalShippingCharge = 0;
            var shipmentChargeDetails = new List<(int PickupAddressId, decimal ShippingCharge)>();
            foreach (var group in groupedShipments)
            {
                var pickupAddress = group.First().Inventory.Address!;
                decimal shipmentWeight = group.Sum(x => x.CartItem.Qunatity * x.CartItem.ProductVariant!.WeightInKgs);
                decimal shippingCharge = await CalculateShippingCharge(pickupAddress, deliveryAddress, shipmentWeight, cod);
                totalShippingCharge += shippingCharge;
                shipmentChargeDetails.Add((group.Key.PickupAddressId, shippingCharge));
                _logger.LogInformation("Calculated ShippingCharge {ShippingCharge} for PickupAddressId {PickupAddressId}, Weight {Weight}kg", shippingCharge, group.Key.PickupAddressId, shipmentWeight);
            }

            // total order cost calculated and tax is added
            decimal totalOrderCost = totalShippingCharge + productCharge - couponCharge;
            totalOrderCost = totalOrderCost * 1.18m;
            _logger.LogInformation("Calculated TotalOrderCost {TotalOrderCost} (ShippingCharge {ShippingCharge}, ProductCharge {ProductCharge}, CouponCharge {CouponCharge})", totalOrderCost, totalShippingCharge, productCharge, couponCharge);

            RequestCreateOrderDTO requestCreateOrderDTO = new RequestCreateOrderDTO();
            requestCreateOrderDTO.TotalShippingAmount = totalShippingCharge;
            requestCreateOrderDTO.TotalProductAmount = productCharge;
            requestCreateOrderDTO.TotalCouponAmount = couponCharge;
            requestCreateOrderDTO.UserId = userId;
            requestCreateOrderDTO.AddressId = deliveryAddress.AddressId;

            // order created
            var order = await _orderService.CreateOrder(requestCreateOrderDTO);
            _logger.LogInformation("Order {OrderId} created for UserId {UserId}", order.OrderId, userId);

            // log created for the order 
            var orderLog = new LogChanges
            {
                TableName = nameof(Order),
                RecordId = order.OrderId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"OrderId={order.OrderId}, UserId={userId}, TotalShippingAmount={totalShippingCharge}, TotalProductAmount={productCharge}, TotalCouponAmount={couponCharge}, AddressId={deliveryAddress.AddressId}",
                UserId = userId,
                ChangedAt = DateTime.Now
            };

            // log history is created
            var createdOrderLog = await _logChanges.Create(orderLog);
            if (createdOrderLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", orderLog.TableName, orderLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", orderLog.TableName, orderLog.RecordId);

            // order items for order is created
            var orderItems = await _orderService.CreateOrderItems(selectedItems, order, selectedCoupon);
            _logger.LogInformation("Created {OrderItemCount} order items for OrderId {OrderId}", orderItems.Count, order.OrderId);

            // payment created for the order
            var payment = await _paymentService.CreatePayment(order.OrderId, requestAddOrderDTO.PaymentMethod);
            _logger.LogInformation("Payment created for OrderId {OrderId} with PaymentMethod {PaymentMethod}", order.OrderId, requestAddOrderDTO.PaymentMethod);

            if (cod == 1)
            {
                _logger.LogInformation("Cash on Delivery selected for OrderId {OrderId}. Marking payment as verified", order.OrderId);
                await OnPaymentVerified(order.OrderId);
            }

            // create the shipment and shipment items for the grouped shipments
            foreach (var group in groupedShipments)
            {
                var pickupAddress = group.First().Inventory.Address!;
                //Console.WriteLine(shipmentChargeDetails);
                var shippingCharge = shipmentChargeDetails.First(x => x.PickupAddressId == group.Key.PickupAddressId).ShippingCharge;
                //Console.WriteLine(shippingCharge);
                var ExpectedDeliveryDate = group.First().ExpectedDeliveryDate;
                var courier = group.First().CourierName;

                _logger.LogInformation("Creating shipment for OrderId {OrderId}, PickupAddressId {PickupAddressId}, Courier {Courier}", order.OrderId, group.Key.PickupAddressId, courier);
                var shipment = await CreateShipment(order, pickupAddress, shippingCharge, ExpectedDeliveryDate, courier);
                _logger.LogInformation("Shipment {ShipmentId} created for OrderId {OrderId}", shipment.ShipmentId, order.OrderId);

                foreach (var selected in group)
                {
                    var orderItem = orderItems.First(oi => oi.ProductVariantId == selected.CartItem.ProductVariantId && oi.InventoryId == selected.Inventory.InventoryId);
                    await _shipmentService.CreateShipmentItems(shipment.ShipmentId, orderItem.OrderItemsId);
                    _logger.LogInformation("OrderItemId {OrderItemId} added to ShipmentId {ShipmentId}", orderItem.OrderItemsId, shipment.ShipmentId);
                }
                await CreateShipmentTracking(shipment);
                _logger.LogInformation("Shipment tracking created for ShipmentId {ShipmentId}", shipment.ShipmentId);
            }

            var response = _mapper.Map<ResponseAddOrderDTO>(order);

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for OrderId {OrderId}", order.OrderId);

            // once order completed the idempotency is completed
            await _idempotencyService.CompleteOperation(
                existing?.IdempotencyKeyId ?? 0,
                200,
                JsonSerializer.Serialize(response));
            _logger.LogInformation("Idempotency operation completed for IdempotencyKey {IdempotencyKey}", idempotencyKey);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while placing order for UserId {UserId}, IdempotencyKey {IdempotencyKey}", userId, idempotencyKey);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for UserId {UserId}, IdempotencyKey {IdempotencyKey}", userId, idempotencyKey);
            throw;
        }
    }
}