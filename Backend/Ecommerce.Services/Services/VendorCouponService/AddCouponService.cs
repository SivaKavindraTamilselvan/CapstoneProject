using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

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

    public async Task<ResponseAddCouponProductDTO> AddCouponProduct(RequestAddCouponProductDTO requestAddCouponProductDTO,int UserId)
    {
        var coupon = await _couponRepsository.Get(requestAddCouponProductDTO.CouponId);
        if(coupon == null)
        {
            throw new DataNotFoundException("Coupon Not Found");
        }
        var createdCoupon = _mapper.Map<CouponsProduct>(coupon);
        createdCoupon.AddedByVendorUserId = UserId;
        await _couponProductRepsository.Create(createdCoupon);
        return _mapper.Map<ResponseAddCouponProductDTO>(createdCoupon);
    }
}