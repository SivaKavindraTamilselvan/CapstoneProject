using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class VendorCouponService : IVendorCouponService
{
    private readonly ICouponRepsository _couponRepsository;
    private readonly ICouponProductRepsository _couponProductRepsository;
    private readonly ICouponValidation _couponValidation;
    private readonly IVendorValidation _vendorValidation;
    private readonly IProductValidation _productValidation;
    private readonly IMapper _mapper;
    public VendorCouponService(ICouponProductRepsository couponProductRepsository,ICouponRepsository couponRepsository,IMapper mapper,ICouponValidation couponValidation,IVendorValidation vendorValidation,IProductValidation productValidation)
    {
        _couponProductRepsository = couponProductRepsository;
        _couponRepsository = couponRepsository;
        _couponValidation = couponValidation;
        _vendorValidation = vendorValidation;
        _mapper = mapper;
        _productValidation = productValidation;
    }

    public async Task<ResponseAddCouponDTO> AddCoupon(RequestAddCouponDTO requestAddCouponDTO,int roleId,int UserId)
    {
        await _couponValidation.ValidateCouponCode(requestAddCouponDTO.CouponCode);
        var coupon = _mapper.Map<Coupons>(requestAddCouponDTO);
        if(roleId == 1)
        {
            coupon.CreatedByAdminUserId = UserId;
        }
        else
        {
            coupon.CreatedByVendorUserId = UserId;
        }
        await _couponRepsository.Create(coupon);
        return _mapper.Map<ResponseAddCouponDTO>(coupon);
    }
}