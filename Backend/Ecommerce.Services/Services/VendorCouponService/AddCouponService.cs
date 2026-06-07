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
    private readonly IMapper _mapper;
    public VendorCouponService(ICouponProductRepsository couponProductRepsository,ICouponRepsository couponRepsository,IMapper mapper,ICouponValidation couponValidation)
    {
        _couponProductRepsository = couponProductRepsository;
        _couponRepsository = couponRepsository;
        _couponValidation = couponValidation;
        _mapper = mapper;
    }

    public async Task<ResponseAddCouponDTO> AddCoupon(RequestAddCouponDTO requestAddCouponDTO,int roleId,int UserId)
    {
        await _couponValidation.ValidateCouponCode(requestAddCouponDTO.CouponCode);
        var coupon = _mapper.Map<Coupons>(requestAddCouponDTO);
        
            coupon.CreatedByUserId = UserId;
    
       
        await _couponRepsository.Create(coupon);
        return _mapper.Map<ResponseAddCouponDTO>(coupon);
    }

    public async Task<ResponseAddCouponProductDTO> AddCouponProduct(RequestAddCouponProductDTO requestAddCouponProductDTO,int UserId)
    {
        var coupon = await _couponValidation.ValidateCoupon(requestAddCouponProductDTO.CouponId);
        var createdCoupon = _mapper.Map<CouponsProduct>(coupon);
        createdCoupon.AddedByVendorUserId = UserId;
        await _couponProductRepsository.Create(createdCoupon);
        return _mapper.Map<ResponseAddCouponProductDTO>(createdCoupon);
    }
}