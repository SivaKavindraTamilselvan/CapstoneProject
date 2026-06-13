using System.Runtime.CompilerServices;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductRepsository : AbstractRepository<int, Product>, IProductRepsository
{
    public ProductRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    private IQueryable<Product> BaseQuery()
    {
        return _ecommerceContext.Product.Include(p => p.ProductApprovalStatus).Include(p => p.ProductStatus)
        .Include(p => p.ProductSubCategory).ThenInclude(p => p!.ProductCategory)
        .Include(p => p.Vendor)
        .Include(p => p.ProductImages).Include(p => p.ProductVariants).ThenInclude(pv => pv.Inventories)
        .Include(p => p.ProductVariants).ThenInclude(pv => pv.ProductVariantAttributes).ThenInclude(pva => pva.ProductSubCategoryAttribute).ThenInclude(psa => psa!.AttributeMaster)
        .Include(pv => pv.MainProductSubCategoryAttribute).ThenInclude(psa => psa!.AttributeMaster)
        .Include(p => p.ProductVariants).ThenInclude(pv => pv.ProductImages);
    }

    public async Task<(List<Product> items, int totalCount)> GetAdminProduct(RequestAdminProductFilter request)
    {
        var query = BaseQuery();
        if (request.ProductApprovalStatusId.HasValue)
        {
            query = query.Where(p => p.ProductApprovalStatusId == request.ProductApprovalStatusId.Value);
        }
        if (request.ProductCategoryId.HasValue)
        {
            query = query.Where(p => p.ProductSubCategory!.ProductCategoryId == request.ProductCategoryId.Value);
        }
        if (request.ProductStatusId.HasValue)
        {
            query = query.Where(p => p.ProductStatusId == request.ProductStatusId.Value);
        }
        if (request.VendorId.HasValue)
        {
            query = query.Where(p => p.VendorId == request.VendorId.Value);
        }
        if (request.ProductSubCategoryId.HasValue)
        {
            query = query.Where(p => p.ProductSubCategoryId == request.ProductSubCategoryId.Value);
        }
        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Price >= request.MinPrice.Value));
        }
        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Price <= request.MaxPrice.Value));
        }
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(p => p.ProductName.ToLower().Contains(request.SearchTerm.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(request.ProductName))
        {
            query = query.Where(p => p.ProductName.ToLower() == request.ProductName.ToLower());
        }
        if (request.MaxAvailableQuantity.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Inventories.Any(a => a.AvailableQuantity <= request.MaxAvailableQuantity)));
        }
        if (request.MaxReservedQuantity.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Inventories.Any(a => a.ReservedQuantity <= request.MaxReservedQuantity)));
        }
        if (request.MinAvailableQuantity.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Inventories.Any(a => a.AvailableQuantity >= request.MinAvailableQuantity)));
        }
        if (request.MinReservedQuantity.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Inventories.Any(a => a.ReservedQuantity >= request.MinReservedQuantity)));
        }
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.CreatedAt).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task<(List<Product> items, int totalCount)> GetVendorProduct(RequestVendorProductFilter request, int vendorid)
    {
        var query = BaseQuery().Where(v => v.VendorId == vendorid);
        if (request.ProductApprovalStatusId.HasValue)
        {
            query = query.Where(p => p.ProductApprovalStatusId == request.ProductApprovalStatusId.Value);
        }
        if (request.ProductCategoryId.HasValue)
        {
            query = query.Where(p => p.ProductSubCategory!.ProductCategoryId == request.ProductCategoryId.Value);
        }
        if (request.ProductStatusId.HasValue)
        {
            query = query.Where(p => p.ProductStatusId == request.ProductStatusId.Value);
        }
        if (request.ProductSubCategoryId.HasValue)
        {
            query = query.Where(p => p.ProductSubCategoryId == request.ProductSubCategoryId.Value);
        }
        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Price >= request.MinPrice.Value));
        }
        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Price <= request.MaxPrice.Value));
        }
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(p => p.ProductName.ToLower().Contains(request.SearchTerm.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(request.ProductName))
        {
            query = query.Where(p => p.ProductName.ToLower() == request.ProductName.ToLower());
        }
        if (request.MaxAvailableQuantity.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Inventories.Any(a => a.AvailableQuantity <= request.MaxAvailableQuantity)));
        }
        if (request.MaxReservedQuantity.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Inventories.Any(a => a.ReservedQuantity <= request.MaxReservedQuantity)));
        }
        if (request.MinAvailableQuantity.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Inventories.Any(a => a.AvailableQuantity >= request.MinAvailableQuantity)));
        }
        if (request.MinReservedQuantity.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Inventories.Any(a => a.ReservedQuantity >= request.MinReservedQuantity)));
        }
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.CreatedAt).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (items, totalCount);
    }
    public async Task<(List<Product> items, int totalCount)> GetUserProducts(RequestUserProductFilter request)
    {
        var query = BaseQuery().Where(p => p.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && p.ProductStatusId == (int)ProductStatusEnum.Active &&
        p.ProductSubCategory != null && p.ProductSubCategory.IsActive &&
        p.ProductSubCategory.ProductCategory != null && p.ProductSubCategory.ProductCategory.IsActive &&
        p.MainProductSubCategoryAttribute!.IsActive && p.MainProductSubCategoryAttribute.AttributeMaster!.IsActive &&
        p.ProductVariants.Any(pv => pv.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && pv.ProductVariantStatusId == (int)ProductStatusEnum.Active &&
        pv.Inventories.Any(i => i.AvailableQuantity > 0) && pv.ProductVariantAttributes.All(a =>
        a.ProductSubCategoryAttribute!.IsActive && a.ProductSubCategoryAttribute.AttributeMaster!.IsActive)));
        if (request.ProductCategoryId.HasValue)
        {
            query = query.Where(p => p.ProductSubCategory!.ProductCategoryId == request.ProductCategoryId.Value);
        }
        if (request.ProductSubCategoryId.HasValue)
        {
            query = query.Where(p => p.ProductSubCategoryId == request.ProductSubCategoryId.Value);
        }
        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Price >= request.MinPrice.Value));
        }
        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.ProductVariants.Any(p => p.Price <= request.MaxPrice.Value));
        }
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(p => p.ProductName.ToLower().Contains(request.SearchTerm.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(request.ProductName))
        {
            query = query.Where(p => p.ProductName.ToLower() == request.ProductName.ToLower());
        }
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.CreatedAt).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (items, totalCount);
    }
    public async Task<Product?> GetProductWithFullDetails(int productId)
    {
        return await BaseQuery().FirstOrDefaultAsync(p => p.ProductId == productId);
    }
    public async Task<Product?> CheckTheProduct(int ProductVariantId, int Qunatity)
    {
        var query = BaseQuery().Where(p => p.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && p.ProductStatusId == (int)ProductStatusEnum.Active &&
        p.ProductSubCategory != null && p.ProductSubCategory.IsActive &&
        p.ProductSubCategory.ProductCategory != null && p.ProductSubCategory.ProductCategory.IsActive &&
        p.MainProductSubCategoryAttribute!.IsActive && p.MainProductSubCategoryAttribute.AttributeMaster!.IsActive &&
        p.ProductVariants.Any(pv => pv.ProductVariantId == ProductVariantId &&
        pv.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && pv.ProductVariantStatusId == (int)ProductStatusEnum.Active &&
        pv.Inventories.Any(i => i.AvailableQuantity > Qunatity) && pv.ProductVariantAttributes.All(a =>
        a.ProductSubCategoryAttribute!.IsActive && a.ProductSubCategoryAttribute.AttributeMaster!.IsActive)));
        return await query.FirstOrDefaultAsync();
    }

}