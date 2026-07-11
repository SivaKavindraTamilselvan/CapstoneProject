using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class CouponRepsository : AbstractRepository<int, Coupons>, ICouponRepsository
{
    public CouponRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    public async Task<Coupons?> GetCouponByCode(string code)
    {
        return await _ecommerceContext.Coupons.FirstOrDefaultAsync(c => c.CouponCode == code);
    }
    public async Task<(List<Coupons> Coupons, int TotalCount)> GetAdminCouponsAsync(AdminCouponFilter filter)
    {
        var query = _ecommerceContext.Coupons.Include(c => c.CouponType).Include(c => c.CreatedByUser).Include(c => c.CouponUsages).AsQueryable();

        if (filter.CouponId.HasValue)
        {
            query = query.Where(c => c.CouponId == filter.CouponId.Value);
        }
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(c => c.CouponCode.ToLower().Contains(search) || c.CouponDescription.ToLower().Contains(search));
        }
        if (filter.CouponTypeId.HasValue)
        {
            query = query.Where(c => c.CouponTypeId == filter.CouponTypeId.Value);
        }
        if (filter.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == filter.IsActive.Value);
        }
        if (filter.IsExpired.HasValue)
        {
            var now = DateTime.Now;
            query = filter.IsExpired.Value ? query.Where(c => c.EndDate < now) : query.Where(c => c.EndDate >= now);
        }
        if (filter.ValidFrom.HasValue)
        {
            query = query.Where(c => c.StartDate >= filter.ValidFrom.Value);
        }
        if (filter.ValidTo.HasValue)
        {
            query = query.Where(c => c.EndDate <= filter.ValidTo.Value);
        }
        if (filter.MinDiscountValue.HasValue)
        {
            query = query.Where(c => c.DiscountValue >= filter.MinDiscountValue.Value);
        }
        if (filter.MaxDiscountValue.HasValue)
        {
            query = query.Where(c => c.DiscountValue <= filter.MaxDiscountValue.Value);
        }
        if (filter.MinOrderAmount.HasValue)
        {
            query = query.Where(c => c.MinimumOrderAmount >= filter.MinOrderAmount.Value);
        }
        if (filter.MaxOrderAmount.HasValue)
        {
            query = query.Where(c => c.MinimumOrderAmount <= filter.MaxOrderAmount.Value);
        }
        var totalCount = await query.CountAsync();

        query = query.OrderByDescending(c => c.CreatedAt).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
        var coupons = await query.ToListAsync();
        return (coupons, totalCount);
    }

    public async Task<Coupons?> GetCouponByIdAsync(int couponId)
    {
        return await _ecommerceContext.Coupons.Include(c => c.CouponType).Include(c => c.CreatedByUser).Include(c => c.CouponsProducts)
        .Include(c => c.CouponUsages).ThenInclude(cu => cu.Order).ThenInclude(o => o!.Users).FirstOrDefaultAsync(c => c.CouponId == couponId);
    }

    public async Task<int> GetUsageCountAsync(int couponId)
    {
        return await _ecommerceContext.CouponUsage.CountAsync(cu => cu.CouponId == couponId);
    }

    public async Task<List<Coupons>> GetAllActiveCoupon(int userId)
    {
        var coupon = await _ecommerceContext.Coupons.Include(c => c.CouponsProducts).ThenInclude(cp => cp.Product).Include(cu => cu.CouponUsages).ThenInclude(cu => cu.Order)
        .Where(c => c.IsActive == true && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now).ToListAsync();
        coupon = coupon.Where(c => c.CouponUsages.Count(cu => cu.Order!.UserId == userId) < c.MinimumNumberOfUsage).ToList();
        return coupon;
    }
    public async Task<List<Coupons>> GetAllAvailableCoupon(decimal totalCartAmount, List<int> productIds, int userId)
    {
        var coupons = await _ecommerceContext.Coupons.Include(c => c.CouponsProducts).ThenInclude(cp => cp.Product).Include(c => c.CouponUsages).ThenInclude(cu => cu.Order)
        .Where(c => c.IsActive && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now && c.MinimumOrderAmount <= totalCartAmount).ToListAsync();
        coupons = coupons.Where(c => c.CouponUsages.Count(cu => cu.Order!.UserId == userId) < c.MinimumNumberOfUsage).ToList();
        var generalCoupons = coupons.Where(c => !c.CouponsProducts.Any(cp => cp.IsActive)).ToList();
        var productCoupons = coupons.Where(c => c.CouponsProducts.Any(cp => cp.IsActive && productIds.Contains(cp.ProductId))).ToList();
        return generalCoupons.Union(productCoupons).ToList();
    }
    public List<CouponUsageDto> GetUsageHistory(Coupons coupon)
    {
        var usageList = new List<CouponUsageDto>();
        var orderedUsages = coupon.CouponUsages.OrderByDescending(u => u.UsedAt);

        foreach (var usage in orderedUsages)
        {
            var dto = new CouponUsageDto
            {
                CouponUsageId = usage.CouponUsageId,
                OrderId = usage.OrderId,
                UsedAt = usage.UsedAt
            };

            if (usage.Order != null)
            {
                dto.OrderNumber = usage.Order.OrderNumber;
                dto.OrderFinalAmount = usage.Order.FinalAmount;
                dto.UserId = usage.Order.UserId;

                if (usage.Order.Users != null)
                {
                    dto.UserName = usage.Order.Users.FirstName + " " + usage.Order.Users.LastName;
                }
            }
            usageList.Add(dto);
        }

        return usageList;
    }
}