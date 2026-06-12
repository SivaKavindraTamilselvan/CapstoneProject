using System.Security.Authentication;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductService : IVendorProductService
{
    public async Task<ResponseUpdateProduct> UpdateProduct(RequestUpdateProductStatus requestUpdateProduct, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} initiated product status update for ProductId {ProductId}", vendorUserId, requestUpdateProduct.ProductId);
        var product = await _productValidation.ValidateProduct(requestUpdateProduct.ProductId);
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        _logger.LogInformation("Product {ProductId} validated successfully for status update", product.ProductId);
        await _productValidation.VendorValidateProduct(requestUpdateProduct.ProductId, vendorUserId);
        if (vendorUser.VendorId != product.VendorId)
        {
            throw new InvalidCredentialException("You Cannot Access other vendor products");
        }
        product = _mapper.Map<Product>(product);
        product.UpdatedAt = DateTime.Now;
        product.ProductStatusId = requestUpdateProduct.ProductStatusId;
        _logger.LogInformation("Updating ProductId {ProductId} status from {OldStatus} to {NewStatus}", product.ProductId, product.ProductStatusId, requestUpdateProduct.ProductStatusId);
        await _productRepsository.Update(product.ProductId, product);
        _logger.LogInformation("ProductId {ProductId} status updated successfully", product.ProductId);
        return _mapper.Map<ResponseUpdateProduct>(product);
    }

    public async Task<ResponseUpdateProduct> UpdateRejectedOrPendingProduct(RequestUpdateProduct requestUpdateProduct, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} initiated product update for ProductId {ProductId}", vendorUserId, requestUpdateProduct.ProductId);
        var product = await _productValidation.ValidateProduct(requestUpdateProduct.ProductId);
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        _logger.LogInformation("Product {ProductId} validated successfully for modification", product.ProductId);
        await _productValidation.VendorValidateProduct(requestUpdateProduct.ProductId, vendorUserId);
        if (vendorUser.VendorId != product.VendorId)
        {
            throw new InvalidCredentialException("You Cannot Access other vendor products");
        }
        _logger.LogInformation("Current Product Approval Status: {ApprovalStatusId} for ProductId {ProductId}", product.ProductApprovalStatusId, product.ProductId);
        if (product.ProductApprovalStatusId == 4 || product.ProductApprovalStatusId == 6)
        {
            throw new InvalidCredentialException("You Cannot update th admin approved product datas");
        }
        product = _mapper.Map<Product>(product);
        product.UpdatedAt = DateTime.Now;
        _logger.LogInformation("Updating ProductId {ProductId}. Product will be resubmitted for admin review", product.ProductId);
        await _productRepsository.Update(product.ProductId, product);
        _logger.LogInformation("ProductId {ProductId} updated successfully by Vendor UserId {VendorUserId}", product.ProductId, vendorUserId);
       
        var productAdminUserIds = await _adminUserRepsository.GetProductAdminUserIds();
        _logger.LogInformation("Sending product update notification to {AdminCount} product admins for ProductId {ProductId}", productAdminUserIds.Count, product.ProductId);

        foreach (var adminUserId in productAdminUserIds)
        {
            await _notificationService.SendToUser(
                adminUserId,
                "Product Updated By Vendor",
                $"Product '{product.ProductName}' has been modified by the vendor and requires review.",
                notificationTypeId: 1,
                referenceType: "Product",
                referenceId: product.ProductId);
        }

        _logger.LogInformation("Product review notifications sent successfully for ProductId {ProductId}", product.ProductId);
        return _mapper.Map<ResponseUpdateProduct>(product);
    }

}