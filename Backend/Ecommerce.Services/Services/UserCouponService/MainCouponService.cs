using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserCouponService : IUserCouponService
{
    private readonly ICouponValidation _couponValidation;
    private readonly ICartValidation _cartValidation;

    private readonly IMapper _mapper;
    public UserCouponService(ICouponValidation couponValidation ,IMapper mapper,ICartValidation cartValidation)
    {
        _couponValidation = couponValidation;
        _cartValidation = cartValidation;
        _mapper = mapper;
    }
}