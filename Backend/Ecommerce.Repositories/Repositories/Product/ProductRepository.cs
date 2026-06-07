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
            .Include(p => p.Vendor)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.Inventories)
            .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.ProductVariantAttributes)
                    .ThenInclude(pva => pva.ProductSubCategoryAttribute)
                        .ThenInclude(psa => psa.AttributeMaster)
            .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.ProductImages);
    }

    // 1. Get all available products for users
    // Admin_Approved(4) + Active(2) + variant Admin_Approved(4) + Active(2) + stock > 0
    public async Task<List<Product>> GetAllAvailableProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductApprovalStatusId == 4
                     && p.ProductStatusId == 2
                     && p.ProductVariants.Any(pv => pv.ProductApprovalStatusId == 4
                                                 && pv.ProductVariantStatusId == 2
                                                 && pv.Inventories.Any(i => i.AvailableQuantity > 0)))
            .ToListAsync();
    }

    // 2. Get all products pending admin approval
    // Vendor_Approved(2) means vendor submitted for admin review
    public async Task<List<Product>> GetAllPendingAdminApprovalProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductApprovalStatusId == 2)
            .ToListAsync();
    }

    // 3. Get all products pending vendor approval (newly added variants etc.)
    public async Task<List<Product>> GetAllPendingVendorApprovalProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductApprovalStatusId == 1)
            .ToListAsync();
    }

    // 4. Get all admin approved products
    public async Task<List<Product>> GetAllAdminApprovedProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductApprovalStatusId == 4)
            .ToListAsync();
    }

    // 5. Get all admin rejected products
    public async Task<List<Product>> GetAllAdminRejectedProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductApprovalStatusId == 5)
            .ToListAsync();
    }

    // 6. Get all vendor rejected products
    public async Task<List<Product>> GetAllVendorRejectedProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductApprovalStatusId == 3)
            .ToListAsync();
    }

    // 7. Get all deleted by admin products
    public async Task<List<Product>> GetAllDeletedByAdminProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductApprovalStatusId == 6)
            .ToListAsync();
    }

    // 8. Get all draft products (vendor not yet submitted)
    public async Task<List<Product>> GetAllDraftProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductStatusId == 1)
            .ToListAsync();
    }

    // 9. Get all active products
    public async Task<List<Product>> GetAllActiveProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductStatusId == 2)
            .ToListAsync();
    }

    // 10. Get all temporarily unavailable products
    public async Task<List<Product>> GetAllTemporarilyUnavailableProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductStatusId == 3)
            .ToListAsync();
    }

    // 11. Get all archived products
    public async Task<List<Product>> GetAllArchivedProducts()
    {
        return await BaseQuery()
            .Where(p => p.ProductStatusId == 4)
            .ToListAsync();
    }

    // 12. Get all products by vendor
    public async Task<List<Product>> GetAllProductsByVendorId(int vendorId)
    {
        return await BaseQuery()
            .Where(p => p.VendorId == vendorId)
            .ToListAsync();
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
    public async Task<Product?> GetProductWithFullDetails(int productId)
    {
        return await BaseQuery()
            .FirstOrDefaultAsync(p => p.ProductId == productId);
    }

    // 20. Get all products with pending variant approvals
    public async Task<List<Product>> GetAllProductsWithPendingVariants()
    {
        return await BaseQuery()
            .Where(p => p.ProductVariants.Any(pv => pv.ProductApprovalStatusId == 1))
            .ToListAsync();
    }

}