using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorProductService
{
    public Task<ResponseUpdateProduct> UpdateProduct(RequestUpdateProduct requestUpdateProduct);
    public Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct,int vendorUserId);
}