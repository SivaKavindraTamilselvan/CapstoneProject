using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public class ProductValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Issues { get; set; } = new();
}
public partial class AdminProductService : IAdminProductService
{

    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllProducts(
    int? approval,
    int? status,
    int? vendorId,
    int? subcategory,
    bool? hasIssues)
    {
        var products = await _productRepsository.GetAdminProduct(
            approval,
            status,
            vendorId,
            subcategory);

        var response = _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);

        for (int i = 0; i < products.Count; i++)
        {
            var validation = await ValidateProductChain(products[i]);

            response[i].IsAvailableForSale =
                products[i].ProductApprovalStatusId == 4 &&
                products[i].ProductStatusId == 2 &&
                validation.IsValid &&
                products[i].ProductVariants.Any(pv =>
                    pv.ProductApprovalStatusId == 4 &&
                    pv.ProductVariantStatusId == 2 &&
                    pv.Inventories.Any(inv => inv.AvailableQuantity > 0));

            response[i].ValidationIssues = validation.Issues;
        }
        if (hasIssues.HasValue)
    {
        response = hasIssues.Value
            ? response.Where(p => p.ValidationIssues.Any()).ToList()
            : response.Where(p => !p.ValidationIssues.Any()).ToList();
    }
        return response;
    }
    public async Task<List<ResponseAdminGetStockProductDTO>> GetAllOutOfStockProducts()
    {
        var products = await _productRepsository.GetAllOutOfStockProducts();
        return _mapper.Map<List<ResponseAdminGetStockProductDTO>>(products);
    }
    public async Task<List<ResponseAdminGetStockProductDTO>> GetAllLowStockProducts(int threshold = 5)
    {
        var products = await _productRepsository.GetAllLowStockProducts(threshold);
        return _mapper.Map<List<ResponseAdminGetStockProductDTO>>(products);
    }

    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllProductsWithPendingVariants()
    {
        var products = await _productRepsository.GetAllProductsWithPendingVariants();
        return _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
    }

    public async Task<ResponseAdminGetAllProductDTO> GetProductWithFullDetails(int productId)
    {
        var product = await _productRepsository.GetProductWithFullDetails(productId);
        if (product == null)
        {
            throw new DataNotFoundException("Product not found");
        }
        return _mapper.Map<ResponseAdminGetAllProductDTO>(product);
    }

    // Check full chain when vendor fetches products
    public async Task<ProductValidationResult> ValidateProductChain(Product product)
    {
        var issues = new List<string>();

        // 1. Category check
        if (!product.ProductSubCategory!.ProductCategory!.IsActive)
            issues.Add($"Category '{product.ProductSubCategory.ProductCategory.ProductCategoryName}' is deactivated by admin");

        // 2. SubCategory check
        if (!product.ProductSubCategory.IsActive)
            issues.Add($"SubCategory '{product.ProductSubCategory.ProductSubCategoryName}' is deactivated by admin");

        // 3. Attribute check — subcategory attributes deactivated
        foreach (var variant in product.ProductVariants)
        {
            
            if (variant.MainProductSubCategoryAttribute != null && !variant.MainProductSubCategoryAttribute.IsActive)
            {
                issues.Add($"Main variant attribute '{variant.MainProductSubCategoryAttribute.AttributeMaster!.AttributeName}' is deactivated");
            }

            foreach (var variantAttr in variant.ProductVariantAttributes)
            {
                if (variantAttr.ProductSubCategoryAttribute != null &&
                    !variantAttr.ProductSubCategoryAttribute.IsActive)
                {
                    issues.Add($"Attribute '{variantAttr.ProductSubCategoryAttribute.AttributeMaster!.AttributeName}' is deactivated for this subcategory");
                }
            }
        }

        return new ProductValidationResult
        {
            IsValid = issues.Count == 0,
            Issues = issues
        };
    }


}