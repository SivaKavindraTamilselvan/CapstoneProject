using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductService : IVendorProductService
{
    public async Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} initiated product creation. ProductName: {ProductName}", vendorUserId, requestAddProduct.ProductName);
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);

        _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);
        var vendor = await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);

        _logger.LogInformation("Vendor {VendorId} is approved and eligible to create products", vendor.VendorId);
        await _productCategoryValidation.ValidateSubCategory(requestAddProduct.ProductSubCategoryId);
        _logger.LogInformation("Product SubCategory {SubCategoryId} validated successfully", requestAddProduct.ProductSubCategoryId);
        
        var product = _mapper.Map<Product>(requestAddProduct);
        product.VendorId = vendor.VendorId;
        product.AddedByVendorUserId = vendorUser.VendorUserId;
        await _productRepsository.Create(product);
        _logger.LogInformation("Product created successfully. ProductId: {ProductId}, ProductName: {ProductName}, VendorId: {VendorId}", product.ProductId, product.ProductName, vendor.VendorId);
        _logger.LogInformation("Sending product review notification to {AdminCount} vendor admins", vendor.VendorId);

        var ownerUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendor.VendorId);
        if (ownerUser != null)
        {
            await _notificationService.SendToUser(
                ownerUser.UserId,
                "New Product Submitted",
                $"A new product '{product.ProductName}' has been submitted and is waiting for approval.",
                notificationTypeId: 1,
                referenceType: "Product",
                referenceId: product.ProductId);
        }
        return _mapper.Map<ResponseAddProduct>(product);
    }
}