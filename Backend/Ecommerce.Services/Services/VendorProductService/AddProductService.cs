using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    public async Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct, int vendorUserId)
    {
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var vendor = await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
        await _productCategoryValidation.ValidateSubCategory(requestAddProduct.ProductSubCategoryId);
        var product = _mapper.Map<Product>(requestAddProduct);
        product.VendorId = vendor.VendorId;
        product.AddedByVendorUserId = vendorUser.VendorUserId;
        await _productRepsository.Create(product);
        return _mapper.Map<ResponseAddProduct>(product);
    }
}