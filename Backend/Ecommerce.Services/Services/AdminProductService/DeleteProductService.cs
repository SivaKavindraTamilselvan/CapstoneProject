using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductService : IAdminProductService
{
    public async Task<ResponseReviewOfProductDTO> DeleteProduct(RequestDeleteProductDTO requestDeleteProductDTO, int adminUserId)
    {
        _logger.LogInformation("Admin UserId {AdminUserId} initiated deletion of ProductId {ProductId}", adminUserId, requestDeleteProductDTO.ProductId);
        var product = await _productValidation.ValidateProduct(requestDeleteProductDTO.ProductId);
        var adminUser = await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Product {ProductId} - {ProductName} found. Current Approval Status: {StatusId}", product.ProductId, product.ProductName, product.ProductApprovalStatusId);

        
        ApprovalHistory approvalHistory = new ApprovalHistory();
        approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
        approvalHistory.EntityType = "Product";
        approvalHistory.EntityId = product.ProductId;
        approvalHistory.ReviewerId = adminUser.AdminUserId;
        approvalHistory.Remarks = requestDeleteProductDTO.Remarks;
        approvalHistory.NewStatusId = (int)ProductApprovalStatusEnum.Deleted_By_Admin;
        product.ProductApprovalStatusId = (int)ProductApprovalStatusEnum.Deleted_By_Admin;
        product.UpdatedAt = DateTime.Now;
        

        _logger.LogInformation("Product {ProductId} marked as Deleted_By_Admin by AdminUserId {AdminUserId}", product.ProductId, adminUser.AdminUserId);
        await _productRepsository.Update(product.ProductId, product);
        await _approvalHistoryRepsository.Create(approvalHistory);
        _logger.LogInformation("Approval history created for ProductId {ProductId}", product.ProductId);

        var ownerVendor = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(product.VendorId);
        if (ownerVendor != null)
        {
            await _notificationService.SendToUser(
                ownerVendor.UserId,
                "Product Deleted By Admin",
                $"Your product '{product.ProductName}' has been deleted by admin. Reason: {requestDeleteProductDTO.Remarks}",
                notificationTypeId: 1,
                referenceType: "Product",
                referenceId: product.ProductId);
             _logger.LogInformation("Product delete notification sent to Vendor Owner UserId {UserId} for ProductId {ProductId}",ownerVendor.UserId,product.ProductId);
        }
        return _mapper.Map<ResponseReviewOfProductDTO>(product);
    }
    public async Task<ResponseReviewOfProductVariantDTO> DeleteProductVaraint(RequestDeleteVariantDTO requestDeleteProductDTO, int adminUserId)
    {
        _logger.LogInformation("Admin UserId {AdminUserId} initiated deletion of ProductId {ProductId}", adminUserId, requestDeleteProductDTO.ProductVariantId);
        var product = await _productValidation.AdminValidateProductVariant(requestDeleteProductDTO.ProductVariantId);
        var adminUser = await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Product {ProductId} - {ProductName} found. Current Approval Status: {StatusId}", product.ProductId, product.SKU, product.ProductApprovalStatusId);

        
        ApprovalHistory approvalHistory = new ApprovalHistory();
        approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
        approvalHistory.EntityType = "Product_Variant";
        approvalHistory.EntityId = product.ProductId;
        approvalHistory.ReviewerId = adminUser.AdminUserId;
        approvalHistory.Remarks = requestDeleteProductDTO.Remarks;
        approvalHistory.NewStatusId = (int)ProductApprovalStatusEnum.Deleted_By_Admin;
        product.ProductApprovalStatusId = (int)ProductApprovalStatusEnum.Deleted_By_Admin;
        product.UpdatedAt = DateTime.Now;
        

        _logger.LogInformation("Product {ProductId} marked as Deleted_By_Admin by AdminUserId {AdminUserId}", product.ProductId, adminUser.AdminUserId);
        await _productVariantRepsository.Update(product.ProductVariantId, product);
        await _approvalHistoryRepsository.Create(approvalHistory);
        _logger.LogInformation("Approval history created for ProductId {ProductId}", product.ProductId);

        var productDetails = await _productRepsository.Get(product.ProductId);
        var ownerVendor = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(productDetails!.VendorId);
        if (ownerVendor != null)
        {
            await _notificationService.SendToUser(
                ownerVendor.UserId,
                "Product Deleted By Admin",
                $"Your product '{product.SKU}' has been deleted by admin. Reason: {requestDeleteProductDTO.Remarks}",
                notificationTypeId: 1,
                referenceType: "Product_Variant",
                referenceId: product.ProductVariantId);
             _logger.LogInformation("Product delete notification sent to Vendor Owner UserId {UserId} for ProductId {ProductId}",ownerVendor.UserId,product.ProductId);
        }
        return _mapper.Map<ResponseReviewOfProductVariantDTO>(product);
    }
}