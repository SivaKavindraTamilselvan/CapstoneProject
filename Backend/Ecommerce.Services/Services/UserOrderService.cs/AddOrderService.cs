using System.Text.Json;
using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

public partial class UserOrderService : IUserOrderService
{
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

    public UserOrderService(
        EcommerceContext ecommerceContext,
        IIdempotencyService idempotencyService,
        INotificationService notificationService,
        IProductRepsository productRepsository,
        IShipmentRepsository shipmentRepsository,
        IOrderRepsository orderRepsository,
        IPaymentService paymentService,
        IOrderService orderService,
        IShipmentService shipmentService,
        ICartItemsRepsository cartItemsRepsository,
        IShipRocketService shipRocketService,
        IUserCartService userCartService,
        IUserCouponService userCouponService,
        IAddressRepsository addressRepsository,
        IOrderItemRepsository orderItemRepsository,
        IMapper mapper)
    {
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
    }

    // add order if the cart is pressed to checkout
    public async Task<ResponseAddOrderDTO> AddOrder(RequestAddOrderDTO requestAddOrderDTO, int userId, string idempotencyKey)
    {
        var requestBody = JsonSerializer.Serialize(requestAddOrderDTO);

        var (isDuplicate, existing) = await _idempotencyService.TryBeginOperation(idempotencyKey, userId, requestBody);

        if (isDuplicate)
        {
            if (existing!.IsCompleted)
            {
                return JsonSerializer.Deserialize<ResponseAddOrderDTO>(existing.ResponseBody!)!;
            }
            throw new InvalidOperationException("Order placement already in progress for this request.");
        }

        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var cartItems = await ValidateProduct(userId);

            var deliveryAddress = await ValidateAddress(requestAddOrderDTO.AddressId, userId);

            decimal productCharge = CalculateProductCharge(cartItems);

            int cod = requestAddOrderDTO.PaymentMethod == 1 ? 1 : 0;

            Coupons? selectedCoupon = null;

            if (requestAddOrderDTO.CouponId != null)
            {
                selectedCoupon = await ValidateCoupon(userId, requestAddOrderDTO.CouponId.Value);
            }
            decimal couponCharge = selectedCoupon?.DiscountValue ?? 0;

            var selectedItems = await GetTheInventoryPickupAddress(cartItems, deliveryAddress, cod);

            var groupedShipments = selectedItems.GroupBy(x => new
            {
                VendorId = x.CartItem.ProductVariant!.Product!.VendorId,
                PickupAddressId = x.Inventory.AddressId
            }).ToList();

            decimal totalShippingCharge = 0;
            var shipmentChargeDetails = new List<(int PickupAddressId, decimal ShippingCharge)>();
            foreach (var group in groupedShipments)
            {
                var pickupAddress = group.First().Inventory.Address!;
                decimal shipmentWeight = group.Sum(x => x.CartItem.Qunatity * x.CartItem.ProductVariant!.WeightInKgs);
                decimal shippingCharge = await CalculateShippingCharge(pickupAddress, deliveryAddress, shipmentWeight, cod);
                totalShippingCharge += shippingCharge;
                shipmentChargeDetails.Add((group.Key.PickupAddressId, shippingCharge));
            }

            decimal totalOrderCost = totalShippingCharge + productCharge - couponCharge;
            totalOrderCost = totalOrderCost * 1.18m;
            Console.WriteLine(totalOrderCost + " " + totalShippingCharge + " " + productCharge);

            RequestCreateOrderDTO requestCreateOrderDTO = new RequestCreateOrderDTO();
            requestCreateOrderDTO.TotalShippingAmount = totalShippingCharge;
            requestCreateOrderDTO.TotalProductAmount = productCharge;
            requestCreateOrderDTO.TotalCouponAmount = couponCharge;
            requestCreateOrderDTO.UserId = userId;
            requestCreateOrderDTO.AddressId = deliveryAddress.AddressId;

            var order = await _orderService.CreateOrder(requestCreateOrderDTO);
            var orderItems = await _orderService.CreateOrderItems(selectedItems, order, selectedCoupon);
            var payment = await _paymentService.CreatePayment(order.OrderId, requestAddOrderDTO.PaymentMethod);

            if (cod == 1)
            {
                await OnPaymentVerified(order.OrderId);
            }

            foreach (var group in groupedShipments)
            {
                var pickupAddress = group.First().Inventory.Address!;
                Console.WriteLine(shipmentChargeDetails);
                var shippingCharge = shipmentChargeDetails.First(x => x.PickupAddressId == group.Key.PickupAddressId).ShippingCharge;
                Console.WriteLine(shippingCharge);
                var ExpectedDeliveryDate = group.First().ExpectedDeliveryDate;
                var courier = group.First().CourierName;

                var shipment = await CreateShipment(order, pickupAddress, shippingCharge, ExpectedDeliveryDate, courier);
                foreach (var selected in group)
                {
                    var orderItem = orderItems.First(oi => oi.ProductVariantId == selected.CartItem.ProductVariantId && oi.InventoryId == selected.Inventory.InventoryId);
                    await _shipmentService.CreateShipmentItems(shipment.ShipmentId, orderItem.OrderItemsId);
                }
                await CreateShipmentTracking(shipment);
            }

            var response = _mapper.Map<ResponseAddOrderDTO>(order);

            await transaction.CommitAsync();

            await _idempotencyService.CompleteOperation(
                existing?.IdempotencyKeyId ?? 0,
                200,
                JsonSerializer.Serialize(response));

            await _notificationService.SendToUser(
                userId,
                "Order Placed Successfully",
                $"Your order #{order.OrderId} has been placed successfully.",
                notificationTypeId: 1,
                referenceType: "Order",
                referenceId: order.OrderId);

            return response;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}