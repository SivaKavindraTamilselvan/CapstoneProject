using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    public async Task<ResponseUpdateProduct> UpdateProduct(RequestUpdateProduct requestUpdateProduct)
    {
        var product = await _productValidation.ValidateProduct(requestUpdateProduct.ProductId);
        product = _mapper.Map<Product>(product);
        product.UpdatedAt = DateTime.Now;
        product.ProductStatusId = requestUpdateProduct.ProductStatusId;
        await _productRepsository.Update(product.ProductId,product);
        return _mapper.Map<ResponseUpdateProduct>(product);
    }
}