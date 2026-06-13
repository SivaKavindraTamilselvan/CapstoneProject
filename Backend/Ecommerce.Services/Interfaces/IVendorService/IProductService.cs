using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorProductService
{
  public Task<PagedResponse<ResponseVendorGetProductVariantOnly>> GetAllProductVariant(RequestVendorProductVariantFilter request, int vendorUserId);
  public Task<ResponseVendorGetAllProductDTO> GetProductWithFullDetails(int productId, int userId);
  public Task<PagedResponse<ResponseVendorGetAllProductDTO>> GetAllProductsByVendorId(RequestVendorProductFilter request, int vendorId);
  public Task<ResponseUpdateProduct> UpdateRejectedOrPendingProduct(RequestUpdateProduct requestUpdateProduct, int vendorUserId);
  public Task<ResponseUpdateProduct> UpdateProduct(RequestUpdateProductStatus requestUpdateProduct, int vendorUserId);
  public Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct, int vendorUserId);
}