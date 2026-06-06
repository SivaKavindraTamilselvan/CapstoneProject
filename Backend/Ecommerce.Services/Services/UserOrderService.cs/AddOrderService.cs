using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

public partial class UserOrderService : IUserOrderService
{
    private readonly ICartItemsRepsository _cartItemsRepsository;
    private readonly IUserCouponService _userCouponService;
    private readonly IAddressRepsository _addressRepsository;
    private readonly IUserCartService _userCartService;
    private readonly IShipRocketService _shipRocketService;
    private readonly IShipmentService _shipmentService;
    private readonly IOrderService _orderService;
    private readonly IInventoryService _inventoryService;
    private readonly IMapper _mapper;

    public UserOrderService(IInventoryService inventoryService, IOrderService orderService,IShipmentService shipmentService, ICartItemsRepsository cartItemsRepsository, IShipRocketService shipRocketService, IUserCartService userCartService, IUserCouponService userCouponService, IAddressRepsository addressRepsository, IOrderRepsository orderRepsository, IOrderItemRepsository orderItemRepsository, IMapper mapper)
    {
        _cartItemsRepsository = cartItemsRepsository;
        _userCouponService = userCouponService;
        _addressRepsository = addressRepsository;
        _userCartService = userCartService;
        _shipRocketService = shipRocketService;
        _shipmentService = shipmentService;
        _orderService = orderService;
        _inventoryService = inventoryService;
        _mapper = mapper;
    }
    public async Task<ResponseAddOrderDTO> AddOrder(RequestAddOrderDTO requestAddOrderDTO, int userId)
    {
        var cartItems = await ValidateProduct(userId);

        var deliveryAddress = await ValidateAddress(requestAddOrderDTO.AddressId);

        decimal productCharge = CalculateProductCharge(cartItems);

        int cod = requestAddOrderDTO.PaymentMethod == 1 ? 1 : 0;

        Coupons? selectedCoupon = null;

        if (requestAddOrderDTO.CouponId != null)
        {
            selectedCoupon = await ValidateCoupon(userId, requestAddOrderDTO.CouponId.Value);
        }
        decimal couponCharge = selectedCoupon?.DiscountValue ?? 0;

        var selectedItems = await GetTheInventoryPickupAddress(cartItems,deliveryAddress,cod);

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
            decimal shipmentWeight = group.Sum(x =>x.CartItem.Qunatity * x.CartItem.ProductVariant!.WeightInKgs);
            decimal shippingCharge = await CalculateShippingCharge(pickupAddress,deliveryAddress,shipmentWeight,cod);
            totalShippingCharge += shippingCharge;
            shipmentChargeDetails.Add((group.Key.PickupAddressId,shippingCharge));
        }

        RequestCreateOrderDTO requestCreateOrderDTO = new RequestCreateOrderDTO();
        requestCreateOrderDTO.TotalShippingAmount = totalShippingCharge;
        requestCreateOrderDTO.TotalProductAmount = productCharge;
        requestCreateOrderDTO.TotalCouponAmount = couponCharge;
        requestCreateOrderDTO.UserId = userId;
        requestCreateOrderDTO.AddressId = deliveryAddress.AddressId;
        var order = await _orderService.CreateOrder(requestCreateOrderDTO);
        var orderItems = await _orderService.CreateOrderItems(selectedItems,order,selectedCoupon);

        foreach (var group in groupedShipments)
        {
            var pickupAddress = group.First().Inventory.Address!;
            var shippingCharge = shipmentChargeDetails.First(x => x.PickupAddressId == group.Key.PickupAddressId).ShippingCharge;
            var ExpectedDeliveryDate = group.First().ExpectedDeliveryDate;

            var shipment = await CreateShipment(order,pickupAddress,shippingCharge, ExpectedDeliveryDate);
            foreach (var selected in group)
            {
                var orderItem = orderItems.First(oi => oi.ProductVariantId == selected.CartItem.ProductVariantId && oi.InventoryId == selected.Inventory.InventoryId);
                await _shipmentService.CreateShipmentItems(shipment.ShipmentId, orderItem.OrderItemsId);
            }
            await CreateShipmentTracking(shipment);
        }

        await _userCartService.DeleteAllCart(userId);

        return _mapper.Map<ResponseAddOrderDTO>(order);
    }
    
}