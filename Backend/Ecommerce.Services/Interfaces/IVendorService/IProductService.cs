using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorProductService
{
    public Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct,int vendorUserId);
    public Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO,int vendorUserId);
    public Task<ResponseAddProductImage> AddProductImage(RequestAddProductImage requestAddProductImage,int vendorUserId);
}