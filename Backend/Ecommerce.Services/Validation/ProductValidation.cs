using System.Security.Authentication;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class ProductValidation : IProductValidation
{
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IProductRepsository _productRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IProductImageRepsository _productImageRepsository;
    public ProductValidation(IVendorUserValidation vendorUserValidation,IProductImageRepsository productImageRepsository,IProductSubCategoryRepsository productSubCategoryRepsository, IProductRepsository productRepsository, IProductVariantRepsository productVariantRepsository, IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository)
    {
        _productRepsository = productRepsository;
        _productVariantRepsository = productVariantRepsository;
        _productImageRepsository = productImageRepsository;
        _vendorUserValidation = vendorUserValidation;
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
    public async Task<ProductImage> ValidateProductImage(int productImageId)
    {
        var product = await _productImageRepsository.Get(productImageId);
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
    public async Task<ProductVariant> ValidateProductVariant(int productVariantId,int vendorUserId)
    {
        var productVariant = await _productVariantRepsository.GetProductByProductVariant(productVariantId);
        if (productVariant == null)
        {
            throw new DataNotFoundException("Product Variant Not Found");
        }
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        if(productVariant!.Product!.VendorId != vendorUser.VendorId)
        {
            throw new InvalidCredentialException("You cannot access other vendor products");
        }
        return productVariant;
    }
}