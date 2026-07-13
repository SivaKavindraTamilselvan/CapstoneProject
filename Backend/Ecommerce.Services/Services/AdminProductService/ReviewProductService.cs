using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductService : IAdminProductService
{
    public async Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO, int adminUserId)
    {
        var product = await _productValidation.ValidateProduct(requestReviewOfProductDTO.ProductId);
        if(product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved || product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Rejected)
        {
            throw new DataApprovalStatusException("Product Already Reviewed by admin");
        }
        if (product.ProductApprovalStatusId != (int)ProductApprovalStatusEnum.Vendor_Approved)
        {
            throw new DataApprovalStatusException("Product is not approved by vendor yet");
        }
        var adminUser = await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        ApprovalHistory approvalHistory = new ApprovalHistory();
        approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
        approvalHistory.EntityType = "Product";
        approvalHistory.EntityId = product.ProductId;
        approvalHistory.ReviewerType = "Admin";
        approvalHistory.ReviewerId = adminUser.AdminUserId;
        approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.UpdatedAt = DateTime.Now;
        var updated = await _productRepsository.Update(product.ProductId, product);
        await _approvalHistoryRepsository.Create(approvalHistory);

        var ownerVendor = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(product.VendorId);
        if (ownerVendor != null)
        {
            string title = "";
            string message = "";
            if (requestReviewOfProductDTO.ApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved)
            {
                title = "Product Approved";
                message = $"Your product '{product.ProductName}' has been approved by admin.";
            }
            else if (requestReviewOfProductDTO.ApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Rejected)
            {
                title = "Product Rejected";
                message = $"Your product '{product.ProductName}' has been rejected by admin. Reason: {requestReviewOfProductDTO.Remarks}";
            }
            await _notificationService.SendToUser(
                ownerVendor.UserId,
                title,
                message,
                notificationTypeId: 1,
                referenceType: "Product",
                referenceId: product.ProductId);

            _logger.LogInformation("Product review notification sent to Vendor Owner UserId {UserId} for ProductId {ProductId}", ownerVendor.UserId, product.ProductId);
        }
        return _mapper.Map<ResponseReviewOfProductDTO>(product);
    }
    public async Task<ResponseReviewOfProductVariantDTO> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO, int adminUserId)
    {
        var product = await _productValidation.AdminValidateProductVariant(requestReviewOfProductDTO.ProductVariantId);
        if(product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved || product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Rejected)
        {
            throw new DataApprovalStatusException("Product Already Reviewed by admin");
        }
        if (product.ProductApprovalStatusId != (int)ProductApprovalStatusEnum.Vendor_Approved)
        {
            throw new DataApprovalStatusException("Product Variant is not approved by vendor yet");
        }
        var adminUser = await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        ApprovalHistory approvalHistory = new ApprovalHistory();
        approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
        approvalHistory.EntityType = "Product_Variant";
        approvalHistory.EntityId = product.ProductId;
        approvalHistory.ReviewerType = "Admin";
        approvalHistory.ReviewerId = adminUser.AdminUserId;
        approvalHistory.Remarks = requestReviewOfProductDTO.Remarks;
        approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.UpdatedAt = DateTime.Now;
        var updated = await _productVariantRepsository.Update(product.ProductVariantId, product);
        await _approvalHistoryRepsository.Create(approvalHistory);

        var ownerVendor = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(product.Product!.VendorId);
        if (ownerVendor != null)
        {
            string title = "";
            string message = "";
            if (requestReviewOfProductDTO.ApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved)
            {
                title = "Product Variant Approved";
                message = $"Your product '{product.ProductVariantId}' has been approved by admin.";
            }
            else if (requestReviewOfProductDTO.ApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Rejected)
            {
                title = "Product Variant Rejected";
                message = $"Your product '{product.ProductVariantId}' has been rejected by admin. Reason: {requestReviewOfProductDTO.Remarks}";
            }
            await _notificationService.SendToUser(
                ownerVendor.UserId,
                title,
                message,
                notificationTypeId: 1,
                referenceType: "Product_Variant",
                referenceId: product.ProductVariantId);
                
            _logger.LogInformation("Product review notification sent to Vendor Owner UserId {UserId} for ProductId {ProductId}", ownerVendor.UserId, product.ProductId);
        }
        return _mapper.Map<ResponseReviewOfProductVariantDTO>(product);
    }
}
