using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

public class UserOrderService : IUserOrderService
{
    private readonly ICartItemsRepsository _cartItemsRepsository;
    private readonly IUserCouponService _userCouponService;
    private readonly IAddressRepsository _addressRepsository;
    private readonly IOrderRepsository _orderRepsository;
    private readonly IOrderItemRepsository _orderItemRepsository;
    private readonly IUserCartService _userCartService;
    private readonly IMapper _mapper;
    public UserOrderService(ICartItemsRepsository cartItemsRepsository,IUserCartService userCartService, IUserCouponService userCouponService, IAddressRepsository addressRepsository, IOrderRepsository orderRepsository,IOrderItemRepsository orderItemRepsository,IMapper mapper)
    {
        _cartItemsRepsository = cartItemsRepsository;
        _userCouponService = userCouponService;
        _addressRepsository = addressRepsository;
        _orderRepsository = orderRepsository;
        _orderItemRepsository = orderItemRepsository;
        _userCartService = userCartService;
        _mapper = mapper;
    }
    public async Task<ResponseAddOrderDTO> AddOrder(RequestAddOrderDTO requestAddOrderDTO, int userId)
    {
        var cartItems = await _cartItemsRepsository.GetCartItemsByUserId(userId);
        if (cartItems.Count == 0)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        var validateCoupon = await _userCouponService.GetAllAvailableCoupons(userId);
        Coupons? selectedCoupon = null;
        if (requestAddOrderDTO.CouponId != null)
        {
            selectedCoupon = validateCoupon.FirstOrDefault(c => c.CouponId == requestAddOrderDTO.CouponId.Value);
            if (selectedCoupon == null)
            {
                throw new DataNotFoundException("The Coupon Is Not Available For You");
            }
        }
        var address = await _addressRepsository.Get(requestAddOrderDTO.AddressId);
        if (address == null)
        {
            throw new DataNotFoundException("Address Not Found");
        }
        Order order = new Order();
        order.OrderNumber = address.AddressId.ToString() + cartItems[0].CartItemsId.ToString() +"hhjjkkjj";
        order.UserId = userId;
        order.TotalProductAmount = cartItems.Sum(c => c.Qunatity * c.ProductVariant!.Price);
        order.TotalCouponAmount = selectedCoupon!.DiscountValue;
        order.TotalShippingAmount = 100;
        order.FinalAmount = order.TotalProductAmount + order.TotalCouponAmount + order.TotalShippingAmount;
        order.AddressId = requestAddOrderDTO.AddressId;
        order.OrderDate = DateTime.Now;
        order.OrderStatusId = 1;
        await _orderRepsository.Create(order);
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
            await _orderItemRepsository.Create(orderItems);
        }
        await _userCartService.DeleteAllCart(userId);
        return _mapper.Map<ResponseAddOrderDTO>(order);
    }
}