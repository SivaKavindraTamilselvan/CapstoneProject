using System.Runtime.CompilerServices;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class ProductRepsository : AbstractRepository<int, Product>, IProductRepsository
{
    public ProductRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    // Base includes
    private IQueryable<Product> BaseQuery()
    {
        return _ecommerceContext.Product
            .Include(p => p.ProductApprovalStatus)
            .Include(p => p.ProductStatus)
            .Include(p => p.ProductSubCategory)
                .ThenInclude(p => p!.ProductCategory)
            .Include(p => p.Vendor)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.Inventories)
            .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.ProductVariantAttributes)
                    .ThenInclude(pva => pva.ProductSubCategoryAttribute)
                        .ThenInclude(psa => psa!.AttributeMaster)
            .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.MainProductSubCategoryAttribute)
                    .ThenInclude(psa => psa!.AttributeMaster)
            .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.ProductImages);
    }

    public async Task<List<Product>> GetAdminProduct(int? approval, int? status, int? vendorid, int? subcategory, int pageNumber, int pageSize)
    {
        var query = BaseQuery();
        if (approval.HasValue)
        {
            query = query.Where(p => p.ProductApprovalStatusId == approval);
        }
        if (status.HasValue)
        {
            query = query.Where(p => p.ProductStatusId == status);
        }
        if (vendorid.HasValue)
        {
            query = query.Where(p => p.VendorId == vendorid);
        }
        if (subcategory.HasValue)
        {
            query = query.Where(p => p.ProductSubCategoryId == subcategory);
        }
        return await query.OrderByDescending(p => p.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<List<Product>> GetVendorProduct(int? approval, int? status, int vendorid, int? subcategory, int pageNumber, int pageSize)
    {
        var query = BaseQuery().Where(v => v.VendorId == vendorid);
        if (approval.HasValue)
        {
            query = query.Where(p => p.ProductApprovalStatusId == approval);
        }
        if (status.HasValue)
        {
            query = query.Where(p => p.ProductStatusId == status);
        }
        if (subcategory.HasValue)
        {
            query = query.Where(p => p.ProductSubCategoryId == subcategory);
        }
        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }
    public async Task<List<Product>> GetUserProducts(int? categoryId, int? subcategoryId, string? searchTerm,int pageNumber,int pageSize)
    {
        var query = BaseQuery().Where(p =>
            p.ProductApprovalStatusId == 4 &&
            p.ProductStatusId == 2 &&
            p.ProductSubCategory!.IsActive &&
            p.ProductSubCategory.ProductCategory!.IsActive &&
            p.ProductVariants.Any(pv =>
                pv.ProductApprovalStatusId == 4 &&
                pv.ProductVariantStatusId == 2 &&
                pv.MainProductSubCategoryAttribute!.IsActive &&
                pv.MainProductSubCategoryAttribute.AttributeMaster!.IsActive &&
                pv.Inventories.Any(i => i.AvailableQuantity > 0) &&
                pv.ProductVariantAttributes.All(a =>
                    a.ProductSubCategoryAttribute!.IsActive &&
                    a.ProductSubCategoryAttribute.AttributeMaster!.IsActive
                )
            )
        );
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.ProductSubCategory!.ProductCategoryId == categoryId);
        }

        if (subcategoryId.HasValue)
        {
            query = query.Where(p => p.ProductSubCategoryId == subcategoryId);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.ProductName.ToLower().Contains(searchTerm.ToLower()));
        }

        return await query.OrderByDescending(p => p.CreatedAt).Skip((pageNumber - 1)*pageSize).Take(pageSize).ToListAsync();
    }
    public async Task<Product?> GetProductWithFullDetails(int productId)
    {
        return await BaseQuery().FirstOrDefaultAsync(p => p.ProductId == productId);
    }
    // 13. Get all available products by vendor
    public async Task<List<Product>> GetAllAvailableProductsByVendorId(int vendorId)
    {
        return await BaseQuery()
            .Where(p => p.VendorId == vendorId
                     && p.ProductApprovalStatusId == 4
                     && p.ProductStatusId == 2
                     && p.ProductVariants.Any(pv => pv.ProductApprovalStatusId == 4
                                                 && pv.ProductVariantStatusId == 2
                                                 && pv.Inventories.Any(i => i.AvailableQuantity > 0)))
            .ToListAsync();
    }

    // 14. Get all products by subcategory
    public async Task<List<Product>> GetAllAvailableProductsBySubCategoryId(int subCategoryId)
    {
        return await BaseQuery()
            .Where(p => p.ProductSubCategoryId == subCategoryId
                     && p.ProductApprovalStatusId == 4
                     && p.ProductStatusId == 2
                     && p.ProductVariants.Any(pv => pv.ProductApprovalStatusId == 4
                                                 && pv.ProductVariantStatusId == 2
                                                 && pv.Inventories.Any(i => i.AvailableQuantity > 0)))
            .ToListAsync();
    }

    // 15. Get all products by category
    public async Task<List<Product>> GetAllAvailableProductsByCategoryId(int categoryId)
    {
        return await BaseQuery()
            .Where(p => p.ProductSubCategory!.ProductCategoryId == categoryId
                     && p.ProductApprovalStatusId == 4
                     && p.ProductStatusId == 2
                     && p.ProductVariants.Any(pv => pv.ProductApprovalStatusId == 4
                                                 && pv.ProductVariantStatusId == 2
                                                 && pv.Inventories.Any(i => i.AvailableQuantity > 0)))
            .ToListAsync();
    }

    // 16. Get all out of stock products
    public async Task<List<Product>> GetAllOutOfStockProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductApprovalStatusId == 4
                     && p.ProductStatusId == 2
                     && p.ProductVariants.All(pv => pv.Inventories.All(i => i.AvailableQuantity == 0)))
            .ToListAsync();
    }

    // 17. Get all low stock products
    public async Task<List<Product>> GetAllLowStockProducts(int threshold = 5)
    {
        return await BaseQuery()
            .Where(p => p.ProductApprovalStatusId == 4
                     && p.ProductStatusId == 2
                     && p.ProductVariants.Any(pv => pv.Inventories.Any(i => i.AvailableQuantity <= threshold
                                                                          && i.AvailableQuantity > 0)))
            .ToListAsync();
    }

    // 18. Search products by name
    public async Task<List<Product>> SearchProductsByName(string searchTerm)
    {
        return await BaseQuery()
            .Where(p => p.ProductName.ToLower().Contains(searchTerm.ToLower())
                     && p.ProductApprovalStatusId == 4
                     && p.ProductStatusId == 2)
            .ToListAsync();
    }

    // 19. Get single product with full details
    // 20. Get all products with pending variant approvals
    public async Task<List<Product>> GetAllProductsWithPendingVariants()
    {
        return await BaseQuery()
            .Where(p => p.ProductVariants.Any(pv => pv.ProductApprovalStatusId == 1))
            .ToListAsync();
    }

}