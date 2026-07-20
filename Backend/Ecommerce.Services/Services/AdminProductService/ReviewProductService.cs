using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductService : IAdminProductService
{
    public async Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            var product = await _productValidation.ValidateProduct(requestReviewOfProductDTO.ProductId);
            if (product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved || product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Rejected)
            {
                throw new DataApprovalStatusException("Product Already Reviewed by admin");
            }
            if (product.ProductApprovalStatusId != (int)ProductApprovalStatusEnum.Vendor_Approved)
            {
                throw new DataApprovalStatusException("Product is not approved by vendor yet");
            }
            var adminUser = await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);

            ApprovalHistory approvalHistory = new ApprovalHistory();
            approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
            approvalHistory.EntityType = "Product";
            approvalHistory.EntityId = product.ProductId;
            approvalHistory.Remarks = requestReviewOfProductDTO.Remarks;
            approvalHistory.ReviewerType = "Admin";
            approvalHistory.ReviewerId = adminUser.AdminUserId;
            approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;
            product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
            product.UpdatedAt = DateTime.Now;

            var updatedProduct = await _productRepsository.Update(product.ProductId, product);
            if (updatedProduct == null)
            {
                throw new DataRegistrationException("Product Not Updated for review");
            }

            var createdHistory = await _approvalHistoryRepsository.Create(approvalHistory);
            if (createdHistory == null)
            {
                throw new DataRegistrationException("Approval History is not created for the product review");
            }
            var previousStatus = product.ProductApprovalStatusId;

            var logChanges = new LogChanges
            {
                TableName = nameof(Product),
                RecordId = updatedProduct.ProductId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductId={product.ProductId}, ProductApprovalStatusId={previousStatus}",
                NewValue = $"ProductId={updatedProduct.ProductId}, ProductApprovalStatusId={requestReviewOfProductDTO.ApprovalStatusId}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

            _logger.LogInformation("Audit log created for ProductId {ProductId}",product.ProductId);

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
            return _mapper.Map<ResponseReviewOfProductDTO>(updatedProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "...");
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back...");
            throw;
        }
    }
    public async Task<ResponseReviewOfProductVariantDTO> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            var product = await _productValidation.AdminValidateProductVariant(requestReviewOfProductDTO.ProductVariantId);
            if (product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved || product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Rejected)
            {
                throw new DataApprovalStatusException("Product Already Reviewed by admin");
            }
            if (product.ProductApprovalStatusId != (int)ProductApprovalStatusEnum.Vendor_Approved)
            {
                throw new DataApprovalStatusException("Product Variant is not approved by vendor yet");
            }
            var adminUser = await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);

            ApprovalHistory approvalHistory = new ApprovalHistory();
            approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
            approvalHistory.EntityType = "Product_Variant";
            approvalHistory.EntityId = product.ProductVariantId;
            approvalHistory.ReviewerType = "Admin";
            approvalHistory.ReviewerId = adminUser.AdminUserId;
            approvalHistory.Remarks = requestReviewOfProductDTO.Remarks;
            approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;
            product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
            product.UpdatedAt = DateTime.Now;

            var updatedVariant = await _productVariantRepsository.Update(product.ProductVariantId, product);
            if(updatedVariant == null)
            {
                throw new DataRegistrationException("Product Variant Not Updated for review");
            }
            var createdHistory =  await _approvalHistoryRepsository.Create(approvalHistory);
            if(createdHistory == null)
            {
                throw new DataRegistrationException("Approval History is not created for the variant review");
            }
            var previousStatus = product.ProductApprovalStatusId;

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductVariant),
                RecordId = updatedVariant.ProductVariantId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductVariantId={product.ProductVariantId}, ProductApprovalStatusId={previousStatus}",
                NewValue = $"ProductVariantId={updatedVariant.ProductVariantId}, ProductApprovalStatusId={requestReviewOfProductDTO.ApprovalStatusId}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

            _logger.LogInformation("Audit log created for ProductVariantId {ProductVariantId}",product.ProductVariantId);

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
            return _mapper.Map<ResponseReviewOfProductVariantDTO>(updatedVariant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "...");
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back...");
            throw;
        }
    }
}
