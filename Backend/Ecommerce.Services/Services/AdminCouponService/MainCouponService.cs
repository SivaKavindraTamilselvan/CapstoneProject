using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class CouponService : ICouponService
{
    private readonly EcommerceContext _ecommerceContext;
    private readonly ICouponRepsository _couponRepsository;
    private readonly ICouponValidation _couponValidation;
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly IMapper _mapper;
    private readonly ILogger<CouponService> _logger;
    private readonly ILogChanges _logChanges;

    public CouponService(EcommerceContext ecommerceContext, ILogger<CouponService> logger, IAdminUserValidation adminUserValidation, ICouponRepsository couponRepsository, IMapper mapper, ICouponValidation couponValidation, ILogChanges logChanges)
    {
        _ecommerceContext = ecommerceContext;
        _adminUserValidation = adminUserValidation;
        _couponRepsository = couponRepsository;
        _couponValidation = couponValidation;
        _mapper = mapper;
        _logger = logger;
        _logChanges = logChanges;
    }

    public async Task<PagedResponse<CouponListDto>> GetCouponsForAdmin(AdminCouponFilter request, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Fetching all coupons");

        var result = await _couponRepsository.GetAdminCouponsAsync(request);

        return new PagedResponse<CouponListDto>
        {
            Items = _mapper.Map<List<CouponListDto>>(result.Coupons),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<CouponDetailDto> GetCouponByIdForAdmin(int couponId, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Fetching coupon with id {CouponId}", couponId);

        var coupon = await _couponRepsository.GetCouponByIdAsync(couponId);

        if (coupon == null)
        {
            _logger.LogWarning("Coupon with id {CouponId} not found", couponId);
            throw new DataNotFoundException($"Coupon with id {couponId} not found");
        }

        var dto = _mapper.Map<CouponDetailDto>(coupon);
        dto.UsageHistory = _couponRepsository.GetUsageHistory(coupon);
        return dto;
    }

}