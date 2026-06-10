using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductVariantRepsository : AbstractRepository<int, ProductVariant>, IProductVariantRepsository
{
    private IQueryable<ProductVariant> BaseQuery()
    {
        return _ecommerceContext.ProductVariant
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.ProductSubCategory)
                    .ThenInclude(sc => sc!.ProductCategory)
            .Include(pv => pv.AddedByVendorUser)
                .ThenInclude(vu => vu!.Vendor)
            .Include(pv => pv.ProductVariantStatus)
            .Include(pv => pv.ProductApprovalStatus)
            .Include(pv => pv.MainProductSubCategoryAttribute)
                .ThenInclude(psa => psa!.AttributeMaster)
            .Include(pv => pv.ProductVariantAttributes)
                .ThenInclude(pva => pva.ProductSubCategoryAttribute)
                    .ThenInclude(psa => psa!.AttributeMaster)
            .Include(pv => pv.ProductImages)
            .Include(pv => pv.Inventories);
    }
    public ProductVariantRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<ProductVariant?> GetProductByProductVariant(int productVariantId)
    {
        return await _ecommerceContext.ProductVariant.Include(p => p.Product).ThenInclude(v => v!.Vendor).FirstOrDefaultAsync(p => p.ProductVariantId == productVariantId);
    }
    public async Task<(List<ProductVariant> Items, int TotalCount)> GetAllVariantsForAdmin(ProductVariantFilterDto filter)
    {
        var query = BaseQuery();
        query = ApplyCommonFilters(query, filter);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(pv => pv.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, total);
    }
    public async Task<(List<ProductVariant> Items, int TotalCount)> GetAllVariantsForVendor(
        int vendorUserId, ProductVariantFilterDto filter)
    {
        var query = BaseQuery()
            .Where(pv => pv.AddedByVendorUserId == vendorUserId);

        query = ApplyCommonFilters(query, filter);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(pv => pv.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, total);
    }
    private static IQueryable<ProductVariant> ApplyCommonFilters(
        IQueryable<ProductVariant> query, ProductVariantFilterDto filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query = query.Where(pv =>
                pv.SKU.Contains(filter.SearchTerm) ||
                pv.Product!.ProductName.Contains(filter.SearchTerm));

        if (filter.CategoryId.HasValue)
            query = query.Where(pv =>
                pv.Product!.ProductSubCategory!.ProductCategoryId == filter.CategoryId);

        if (filter.SubCategoryId.HasValue)
            query = query.Where(pv =>
                pv.Product!.ProductSubCategoryId == filter.SubCategoryId);

        if (filter.StatusId.HasValue)
            query = query.Where(pv => pv.ProductVariantStatusId == filter.StatusId);

        if (filter.ApprovalStatusId.HasValue)
            query = query.Where(pv => pv.ProductApprovalStatusId == filter.ApprovalStatusId);

        if (filter.MinPrice.HasValue)
            query = query.Where(pv => pv.Price >= filter.MinPrice);

        if (filter.MaxPrice.HasValue)
            query = query.Where(pv => pv.Price <= filter.MaxPrice);

        return query;
    }

}