using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class VendorCouponService : IVendorCouponService
{
    private readonly ICouponRepsository _couponRepsository;
    private readonly ICouponProductRepsository _couponProductRepsository;
    private readonly ICouponVendorRepsository _couponVendorRepsository;
    private readonly ICouponValidation _couponValidation;
    private readonly IVendorValidation _vendorValidation;
    private readonly IProductValidation _productValidation;
    private readonly IMapper _mapper;
    public VendorCouponService(ICouponProductRepsository couponProductRepsository,ICouponRepsository couponRepsository,ICouponVendorRepsository couponVendorRepsository,IMapper mapper,ICouponValidation couponValidation,IVendorValidation vendorValidation,IProductValidation productValidation)
    {
        _couponProductRepsository = couponProductRepsository;
        _couponRepsository = couponRepsository;
        _couponVendorRepsository = couponVendorRepsository;
        _couponValidation = couponValidation;
        _vendorValidation = vendorValidation;
        _mapper = mapper;
        _productValidation = productValidation;
    }

    public async Task<ResponseAddCouponDTO> AddCoupon(RequestAddCouponDTO requestAddCouponDTO)
    {
        await _couponValidation.ValidateCouponCode(requestAddCouponDTO.CouponCode);
        var coupon = _mapper.Map<Coupons>(requestAddCouponDTO);
        await _couponRepsository.Create(coupon);
        return _mapper.Map<ResponseAddCouponDTO>(coupon);
    }
    public async Task<ResponseAddCouponDTO> AddVendorCoupon(RequestAddCouponDTO requestAddCouponDTO,int vendorId)
    {
        var vendor = await _vendorValidation.ValidateVendorUser(vendorId);
        var baseCoupon = await AddCoupon(requestAddCouponDTO);
        CouponsVendor couponsVendor = new CouponsVendor();
        couponsVendor.CouponId = baseCoupon.CouponId;
        couponsVendor.VendorId = vendor.VendorId;
        await _couponVendorRepsository.Create(couponsVendor);
        return baseCoupon;
    }

    public async Task<ResponseAddCouponDTO> AddProductCoupon(RequestAddCouponProductDTO requestAddCouponProductDTO)
    {
        var product = await _productValidation.ValidateProduct(requestAddCouponProductDTO.ProductId);
        var baseCoupon = await AddCoupon(requestAddCouponProductDTO.requestAddCouponDTO);
        CouponsProduct couponsProduct = new CouponsProduct();
        couponsProduct.CouponId = baseCoupon.CouponId;
        couponsProduct.ProductId = product.ProductId;
        await _couponProductRepsository.Create(couponsProduct);
        return baseCoupon;
    }

}