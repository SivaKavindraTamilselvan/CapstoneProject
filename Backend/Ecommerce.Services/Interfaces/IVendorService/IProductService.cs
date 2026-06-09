using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorProductService
{
    public Task<ResponseUpdateProduct> UpdateRejectedOrPendingProduct(RequestUpdateProduct requestUpdateProduct, int vendorUserId);
    public Task<ResponseUpdateProduct> UpdateProduct(RequestUpdateProductStatus requestUpdateProduct, int vendorUserId);
    public Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct, int vendorUserId);
    Task<List<ResponseVendorGetAllProductDTO>> GetAllProductsByVendorId(int? approval, int? status, int vendorId, int? subcategory,int pageNumber,int pageSize,bool? hasIssues,bool? isAvailableForSale);
    Task<List<ResponseVendorGetAllProductDTO>> GetAllAvailableProductsByVendorId(int vendorId);
    Task<List<ResponseVendorGetStockProductDTO>> GetAllLowStockProducts(int vendorId, int threshold = 5);
    Task<List<ResponseVendorGetStockProductDTO>> GetAllOutOfStockProducts(int vendorId);
    Task<List<ResponseVendorGetAllProductDTO>> GetAllProductsWithPendingVariants(int vendorId);
}