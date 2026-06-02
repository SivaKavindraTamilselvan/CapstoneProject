using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IProductService
{
    public Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct,int vendorUserId);
}