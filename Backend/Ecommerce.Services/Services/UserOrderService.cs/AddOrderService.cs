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
    private readonly IOrderRepsository _orderRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IUserCartService _userCartService;
    private readonly IShipRocketService _shipRocketService;
    private readonly IMapper _mapper;
    public UserOrderService(ICartItemsRepsository cartItemsRepsository, IShipRocketService shipRocketService, IUserCartService userCartService, IUserCouponService userCouponService, IAddressRepsository addressRepsository, IOrderRepsository orderRepsository, IOrderItemRepsository orderItemRepsository, IMapper mapper)
    {
        _cartItemsRepsository = cartItemsRepsository;
        _userCouponService = userCouponService;
        _addressRepsository = addressRepsository;
        _orderRepsository = orderRepsository;
        _orderItemRepsository = orderItemRepsository;
        _userCartService = userCartService;
        _shipRocketService = shipRocketService;
        _mapper = mapper;
    }

    private async Task<decimal> CalculateShippingCharge(Address address, decimal weight, int cod)
    {
        ServiceabilityRequestDTO serviceabilityRequestDTO = new ServiceabilityRequestDTO();
        serviceabilityRequestDTO.Cod = cod;
        serviceabilityRequestDTO.DeliveryPostcode = address.PinCode;
        serviceabilityRequestDTO.Weight = weight;
        var bestCourier = await _shipRocketService.CheckServiceability(serviceabilityRequestDTO);
        if (bestCourier == null)
        {
            throw new Exception("No courier available");
        }
        return bestCourier.Rate;

    }
    private async Task<List<CartItems>> ValidateProduct(int userId)
    {
        var cartItems = await _cartItemsRepsository.GetCartItemsByUserId(userId);
        if (cartItems.Count == 0)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        if (cartItems.Any(c => c.ProductVariant == null))
        {
            throw new DataNotFoundException("Product variant not loaded");
        }
        return cartItems;
    }
    private async Task<Address> ValidateAddress(int addressId)
    {
        var address = await _addressRepsository.Get(addressId);
        if (address == null)
        {
            throw new DataNotFoundException("Address Not Found");
        }
        return address;
    }
    private async Task<Coupons> ValidateCoupon(int userId, int couponId)
    {
        var validateCoupon = await _userCouponService.GetAllAvailableCoupons(userId);
        Coupons? selectedCoupon = null;

        selectedCoupon = validateCoupon.FirstOrDefault(c => c.CouponId == couponId);
        if (selectedCoupon == null)
        {
            throw new DataNotFoundException("The Coupon Is Not Available For You");
        }
        return selectedCoupon;

    }
    private decimal CalculateProductCharge(List<CartItems> cartItems)
    {
        decimal productCharge = cartItems.Sum(c => c.Qunatity * c.ProductVariant!.Price);
        return productCharge;
    }
    private decimal CalculateOrderWeight(List<CartItems> cartItems)
    {
        decimal Weight = cartItems.Sum(c => c.Qunatity * c.ProductVariant!.WeightInKgs);
        return Weight;
    }
    private async Task<Order> CreateOrder(decimal prductCharge, decimal shippingCharge, decimal couponCharge, int userId, int addressId)
    {
        Order order = new Order();
        order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";
        order.UserId = userId;
        order.TotalProductAmount = prductCharge;
        order.TotalCouponAmount = couponCharge;
        order.TotalShippingAmount = shippingCharge;
        order.FinalAmount = order.TotalProductAmount - order.TotalCouponAmount + order.TotalShippingAmount;
        order.AddressId = addressId;
        order.OrderDate = DateTime.Now;
        order.OrderStatusId = 1;
        var createdOrder = await _orderRepsository.Create(order);
        if (createdOrder == null)
        {
            throw new DataNotFoundException("Order Not Created");
        }
        return createdOrder;
    }
    private async Task<List<OrderItems>> CreateOrderItems(List<CartItems> cartItems, Order order, Coupons? selectedCoupon)
    {
        List<OrderItems> orderItemsList = new List<OrderItems>();
        foreach (var list in cartItems)
        {
            OrderItems orderItems = new OrderItems();
            orderItems.ProductVariantId = list.ProductVariantId;
            orderItems.Quantity = list.Qunatity;
            orderItems.OrderId = order.OrderId;
            orderItems.UnitPrice = list.ProductVariant!.Price;
            orderItems.Discount = 0;
            orderItems.OrderItemStatusId = 1;
            if (selectedCoupon != null)
            {
                bool applicable = selectedCoupon.CouponsProducts.Any(p => p.ProductId == list.ProductVariant.ProductId);
                if (applicable)
                {
                    orderItems.Discount = selectedCoupon.DiscountValue;
                }
            }
            var createdOrderItem = await _orderItemRepsository.Create(orderItems);
            if (createdOrderItem != null)
            {
                orderItemsList.Add(createdOrderItem);
            }
        }
        return orderItemsList;
    }
    public async Task<ResponseAddOrderDTO> AddOrder(RequestAddOrderDTO requestAddOrderDTO, int userId)
    {
        var cartItems = await ValidateProduct(userId);

        var address = await ValidateAddress(requestAddOrderDTO.AddressId);

        decimal orderWeight = CalculateOrderWeight(cartItems);

        decimal productCharge = CalculateProductCharge(cartItems);

        int cod = requestAddOrderDTO.PaymentMethod == 1 ? 1 : 0;

        decimal shippingCharge = await CalculateShippingCharge(address, orderWeight, cod);
        Coupons? selectedCoupon = null;
        if (requestAddOrderDTO.CouponId != null)
        {
            selectedCoupon = await ValidateCoupon(userId, requestAddOrderDTO.CouponId.Value);
        }
        decimal couponCharge = selectedCoupon?.DiscountValue ?? 0;

        var order = await CreateOrder(productCharge, shippingCharge, couponCharge, userId, address.AddressId);
        await CreateOrderItems(cartItems, order, selectedCoupon);

        await _userCartService.DeleteAllCart(userId);
        return _mapper.Map<ResponseAddOrderDTO>(order);
    }
}