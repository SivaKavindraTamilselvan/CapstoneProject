using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IProductValidation
{
    public Task<ProductVariant> ValidateProductVariantIfApproved(int productVariantId);
    public Task<ProductValidationResult> ValidateProductChain(Product product);
    public Task<Product> VendorValidateProduct(int productId,int vendorId);
    public Task<ProductVariant> AdminValidateProductVariant(int productVariantId);
    public Task<Product> ValidateProduct(int productId);
    public Task<Product> ValidateProductIfApproved(int productId);
    public Task<ProductVariant> ValidateProductVariant(int productVariantId, int vendorUserId);
    public Task<ProductImage> ValidateProductImage(int productImageId);
}