using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductVariantRepsository : AbstractRepository<int, ProductVariant>, IProductVariantRepsository
{
    private IQueryable<ProductVariant> BaseQuery()
    {
        return _ecommerceContext.ProductVariant.Include(pv => pv.Product)
        .ThenInclude(p => p!.ProductSubCategory).ThenInclude(sc => sc!.ProductCategory)
        .Include(pv => pv.AddedByVendorUser).ThenInclude(vu => vu!.Vendor)
        .Include(pv => pv.AddedByVendorUser).ThenInclude(vu => vu!.User)
        .Include(pv => pv.ProductVariantStatus).Include(pv => pv.ProductApprovalStatus)
        .Include(p => p.Product).ThenInclude(pv => pv!.MainProductSubCategoryAttribute).ThenInclude(psa => psa!.AttributeMaster)
        .Include(pv => pv.ProductVariantAttributes).ThenInclude(pva => pva.ProductSubCategoryAttribute).ThenInclude(psa => psa!.AttributeMaster)
        .Include(pv => pv.ProductImages).ThenInclude(p => p.DisplayOrder)
        .Include(pv => pv.Inventories)
        .Include(pv => pv.Inventories).ThenInclude(i => i.Address);

    }
    public ProductVariantRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<ProductVariant?> GetProductByProductVariant(int productVariantId)
    {
        return await _ecommerceContext.ProductVariant.Include(p => p.Product).ThenInclude(v => v!.Vendor).FirstOrDefaultAsync(p => p.ProductVariantId == productVariantId);
    }
    public async Task<(List<ProductVariant> Items, int TotalCount)> GetAllVariantsForAdmin(RequestAdminProductVariantFilter request)
    {
        var query = BaseQuery();
        if (request.AddedByVendorUserId.HasValue)
        {
            query = query.Where(p => p.AddedByVendorUserId == request.AddedByVendorUserId.Value);
        }
        if (request.ApprovalStatusId.HasValue)
        {
            query = query.Where(p => p.ProductApprovalStatusId == request.ApprovalStatusId.Value);
        }
        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.Product!.ProductSubCategory!.ProductCategoryId == request.CategoryId.Value);
        }
        if (request.SubCategoryId.HasValue)
        {
            query = query.Where(p => p.Product!.ProductSubCategoryId == request.SubCategoryId.Value);
        }
        if (request.IsReturn.HasValue)
        {
            query = query.Where(p => p.IsReturn == request.IsReturn.Value);
        }
        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }
        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= request.MinPrice.Value);
        }
        if (request.VendorId.HasValue)
        {
            query = query.Where(p => p.Product!.VendorId == request.VendorId);
        }
        if (!string.IsNullOrEmpty(request.SKU))
        {
            query = query.Where(p => p.SKU.ToLower() == request.SKU.ToLower());
        }
        if (request.ProductId.HasValue)
        {
            query = query.Where(p => p.ProductId == request.ProductId);
        }
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(p => p.Product!.ProductName.ToLower().Contains(request.SearchTerm.ToLower()));
        }
        if (request.StatusId.HasValue)
        {
            query = query.Where(p => p.ProductVariantStatusId == request.StatusId.Value);
        }
        if (request.MainProductSubCategoryAttributeId.HasValue)
        {
            query = query.Where(p => p.Product!.MainProductSubCategoryAttributeId == request.MainProductSubCategoryAttributeId.Value);
        }
        if (request.MinimuQuantityPerUser.HasValue)
        {
            query = query.Where(p => p.MinimuQuantityPerUser >= request.MinimuQuantityPerUser.Value);
        }
        if (request.includeIsDeleted.HasValue)
        {
            if (request.includeIsDeleted.Value)
            {
                query = query.Where(p => p.ProductApprovalStatusId == 6);
            }
            else
            {
                query = query.Where(p => p.ProductApprovalStatusId != 6);
            }

        }

        var total = await query.CountAsync();

        var items = await query.OrderByDescending(pv => pv.CreatedAt).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (items, total);
    }
    public async Task<(List<ProductVariant> Items, int TotalCount)> GetAllVariantsForVendor(RequestVendorProductVariantFilter request, int vendorid)
    {
        var query = BaseQuery().Where(p => p.Product!.VendorId == vendorid);
        if (request.AddedByVendorUserId.HasValue)
        {
            query = query.Where(p => p.AddedByVendorUserId == request.AddedByVendorUserId.Value);
        }
        if (request.ApprovalStatusId.HasValue)
        {
            query = query.Where(p => p.ProductApprovalStatusId == request.ApprovalStatusId.Value);
        }
        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.Product!.ProductSubCategory!.ProductCategoryId == request.CategoryId.Value);
        }
        if (request.SubCategoryId.HasValue)
        {
            query = query.Where(p => p.Product!.ProductSubCategoryId == request.SubCategoryId.Value);
        }
        if (request.IsReturn.HasValue)
        {
            query = query.Where(p => p.IsReturn == request.IsReturn.Value);
        }
        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }
        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= request.MinPrice.Value);
        }
        if (!string.IsNullOrEmpty(request.SKU))
        {
            query = query.Where(p => p.SKU.ToLower() == request.SKU.ToLower());
        }
        if (request.ProductId.HasValue)
        {
            query = query.Where(p => p.ProductId == request.ProductId);
        }
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(p => p.Product!.ProductName.ToLower().Contains(request.SearchTerm.ToLower()));
        }
        if (request.StatusId.HasValue)
        {
            query = query.Where(p => p.ProductVariantStatusId == request.StatusId.Value);
        }
        if (request.MainProductSubCategoryAttributeId.HasValue)
        {
            query = query.Where(p => p.Product!.MainProductSubCategoryAttributeId == request.MainProductSubCategoryAttributeId.Value);
        }
        if (request.MinimuQuantityPerUser.HasValue)
        {
            query = query.Where(p => p.MinimuQuantityPerUser >= request.MinimuQuantityPerUser.Value);
        }
        if (request.includeIsDeleted.HasValue)
        {
            if (request.includeIsDeleted.Value)
            {
                query = query.Where(p => p.ProductApprovalStatusId == 6 || p.ProductVariantStatusId == 4);
            }
            else
            {
                query = query.Where(p => p.ProductApprovalStatusId != 6 && p.ProductVariantStatusId != 4);
            }

        }

        var total = await query.CountAsync();

        var items = await query.OrderByDescending(pv => pv.CreatedAt).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (items, total);
    }
    public async Task<ProductVariant?> GetVariantsForVendor(int variantId)
    {
        return await BaseQuery().Where(p => p.ProductVariantId == variantId).FirstOrDefaultAsync();

    }

}