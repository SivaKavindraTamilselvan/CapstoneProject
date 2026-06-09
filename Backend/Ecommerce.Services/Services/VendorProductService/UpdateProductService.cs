using System.Security.Authentication;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    public async Task<ResponseUpdateProduct> UpdateProduct(RequestUpdateProductStatus requestUpdateProduct, int vendorUserId)
    {
        var product = await _productValidation.ValidateProduct(requestUpdateProduct.ProductId);
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        if (vendorUser.VendorId != product.VendorId)
        {
            throw new InvalidCredentialException("You Cannot Access other vendor products");
        }
        product = _mapper.Map<Product>(product);
        product.UpdatedAt = DateTime.Now;
        product.ProductStatusId = requestUpdateProduct.ProductStatusId;
        await _productRepsository.Update(product.ProductId, product);
        return _mapper.Map<ResponseUpdateProduct>(product);
    }

    public async Task<ResponseUpdateProduct> UpdateRejectedOrPendingProduct(RequestUpdateProduct requestUpdateProduct, int vendorUserId)
    {
        var product = await _productValidation.ValidateProduct(requestUpdateProduct.ProductId);
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        if (vendorUser.VendorId != product.VendorId)
        {
            throw new InvalidCredentialException("You Cannot Access other vendor products");
        }
        if (product.ProductApprovalStatusId == 4)
        {
            throw new InvalidCredentialException("You Cannot update th admin approved product datas");
        }
        product = _mapper.Map<Product>(product);
        product.UpdatedAt = DateTime.Now;
        await _productRepsository.Update(product.ProductId, product);
        return _mapper.Map<ResponseUpdateProduct>(product);
    }

}