using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class CouponService : ICouponService
{
    private readonly ICouponRepsository _couponRepsository;
    private readonly ICouponValidation _couponValidation;
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly IMapper _mapper;
    private readonly ILogger<CouponService> _logger;
    public CouponService(ILogger<CouponService> logger, IAdminUserValidation adminUserValidation, ICouponRepsository couponRepsository, IMapper mapper, ICouponValidation couponValidation)
    {
        _adminUserValidation = adminUserValidation;
        _couponRepsository = couponRepsository;
        _couponValidation = couponValidation;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResponseAddCouponDTO> AddCoupon(RequestAddCouponDTO requestAddCouponDTO, int roleId, int UserId)
    {
        await _couponValidation.ValidateCouponCode(requestAddCouponDTO.CouponCode);
        var coupon = _mapper.Map<Coupons>(requestAddCouponDTO);
        if (roleId == 1)
        {
            coupon.CouponTypeId = 1;
        }
        else
        {
            coupon.CouponTypeId = 2;
        }
        coupon.CreatedByUserId = UserId;
        await _couponRepsository.Create(coupon);
        return _mapper.Map<ResponseAddCouponDTO>(coupon);
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

    public async Task<ResponseAddCouponDTO> UpdateCouponByIdForAdmin(UpdateCouponDto update, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);

        _logger.LogInformation("Fetching coupon with id {CouponId}", update.CouponId);

        var coupon = await _couponRepsository.Get(update.CouponId);

        if (coupon == null)
        {
            _logger.LogWarning("Coupon with id {CouponId} not found", update.CouponId);
            throw new DataNotFoundException($"Coupon with id {update.CouponId} not found");
        }

        if (update.CouponDescription != null)
        {
            coupon.CouponDescription = update.CouponDescription;
        }

        if (update.DiscountValue.HasValue)
        {
            coupon.DiscountValue = update.DiscountValue.Value;
        }

        if (update.MinimumOrderAmount.HasValue)
        {
            coupon.MinimumOrderAmount = update.MinimumOrderAmount.Value;
        }

        if (update.StartDate.HasValue)
        {
            coupon.StartDate = update.StartDate.Value;
        }

        if (update.EndDate.HasValue)
        {
            coupon.EndDate = update.EndDate.Value;
        }

        if (update.MinimumNumberOfUsage.HasValue)
        {
            coupon.MinimumNumberOfUsage = update.MinimumNumberOfUsage.Value;
        }

        if (update.IsActive.HasValue)
        {
            coupon.IsActive = update.IsActive.Value;
        }

        coupon = await _couponRepsository.Update(coupon.CouponId, coupon);
        return _mapper.Map<ResponseAddCouponDTO>(coupon);

    }
    public async Task<ResponseAddCouponDTO> DeactivateCoupon(int couponId, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);

        _logger.LogInformation("Fetching coupon with id {CouponId}", couponId);

        var coupon = await _couponRepsository.Get(couponId);

        if (coupon == null)
        {
            _logger.LogWarning("Coupon with id {CouponId} not found", couponId);
            throw new DataNotFoundException($"Coupon with id {couponId} not found");
        }

        if (coupon.IsActive == false)
        {
            throw new DataAlreadyRegisteredException("Coupon Already Deactivated");
        }
        coupon.IsActive = false;
        coupon = await _couponRepsository.Update(coupon.CouponId, coupon);
        return _mapper.Map<ResponseAddCouponDTO>(coupon);

    }
    public async Task<ResponseAddCouponDTO> ActivateCoupon(int couponId ,int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);

        _logger.LogInformation("Fetching coupon with id {CouponId}", couponId);

        var coupon = await _couponRepsository.Get(couponId);

        if (coupon == null)
        {
            _logger.LogWarning("Coupon with id {CouponId} not found", couponId);
            throw new DataNotFoundException($"Coupon with id {couponId} not found");
        }

        if (coupon.IsActive == true)
        {
            throw new DataAlreadyRegisteredException("Coupon Already Activated");
        }

        coupon.IsActive = true;
        coupon = await _couponRepsository.Update(coupon.CouponId, coupon);
        return _mapper.Map<ResponseAddCouponDTO>(coupon);

    }
}