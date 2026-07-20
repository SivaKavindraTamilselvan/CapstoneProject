using AutoMapper;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;
public partial class UserCouponService : IUserCouponService
{
    private readonly IUserValidation _userValidation;
    private readonly ICouponValidation _couponValidation;
    private readonly ICartValidation _cartValidation;
    private readonly ILogger<UserCouponService> _logger;
    private readonly IMapper _mapper;
    public UserCouponService(IUserValidation userValidation,ILogger<UserCouponService> logger,ICouponValidation couponValidation ,IMapper mapper,ICartValidation cartValidation)
    {
        _userValidation = userValidation;
        _couponValidation = couponValidation;
        _cartValidation = cartValidation;
        _mapper = mapper;
        _logger = logger;
    }
}