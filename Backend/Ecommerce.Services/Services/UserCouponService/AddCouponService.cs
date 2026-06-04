using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserCouponService : IUserCouponService
{
    private readonly ICouponRepsository _couponRepsository;
    private readonly ICartItemsRepsository _cartItemsRepsository;
    private readonly IMapper _mapper;
    public UserCouponService(ICouponRepsository couponRepsository, ICartItemsRepsository cartItemsRepsository, IMapper mapper)
    {
        _couponRepsository = couponRepsository;
        _cartItemsRepsository = cartItemsRepsository;
        _mapper = mapper;
    }
}