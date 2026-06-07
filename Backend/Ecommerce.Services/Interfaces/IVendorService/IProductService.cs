using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorProductService
{
    public Task<ResponseUpdateProduct> UpdateProduct(RequestUpdateProduct requestUpdateProduct);
    public Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct,int vendorUserId);
    Task<List<ResponseVendorGetAllProductDTO>> GetAllProductsByVendorId(int vendorId);
    Task<List<ResponseVendorGetAllProductDTO>> GetAllAvailableProductsByVendorId(int vendorId);
    Task<List<ResponseVendorGetDraftProductDTO>> GetAllDraftProducts(int vendorId);
    Task<List<ResponseVendorGetStockProductDTO>> GetAllLowStockProducts(int vendorId, int threshold = 5);
    Task<List<ResponseVendorGetStockProductDTO>> GetAllOutOfStockProducts(int vendorId);
    Task<List<ResponseVendorGetAllProductDTO>> GetAllProductsWithPendingVariants(int vendorId);
}