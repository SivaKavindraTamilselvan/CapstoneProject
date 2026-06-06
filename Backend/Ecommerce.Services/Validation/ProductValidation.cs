using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class ProductValidation : IProductValidation
{
    private readonly IProductRepsository _productRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    public ProductValidation(IProductSubCategoryRepsository productSubCategoryRepsository, IProductRepsository productRepsository, IProductVariantRepsository productVariantRepsository, IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository)
    {
        _productRepsository = productRepsository;
        _productVariantRepsository = productVariantRepsository;
    }
    public async Task<Product> ValidateProduct(int productId)
    {
        var product = await _productRepsository.Get(productId);
        if (product == null)
        {
            throw new DataNotFoundException("Product Not Found");
        }
        return product;
    }
    public async Task<Product> ValidateProductIfApproved(int productId)
    {
        var product = await ValidateProduct(productId);
        if (product.ProductApprovalStatusId != 4)
        {
            throw new DataApprovalStatusException("Product Is Not Yet Approved");
        }
        return product;
    }
    public async Task<ProductVariant> ValidateProductVariant(int productVariantId)
    {
        var productVariant = await _productVariantRepsository.Get(productVariantId);
        if (productVariant == null)
        {
            throw new DataNotFoundException("Product Variant Not Found");
        }
        return productVariant;
    }
}