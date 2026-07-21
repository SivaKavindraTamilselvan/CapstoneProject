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
            _logger.LogInformation("Admin UserId {AdminUserId} reviewing ProductId {ProductId} with ApprovalStatusId {ApprovalStatusId}", adminUserId, requestReviewOfProductDTO.ProductId, requestReviewOfProductDTO.ApprovalStatusId);

            var product = await _productValidation.ValidateProduct(requestReviewOfProductDTO.ProductId);
            if (product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved || product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Rejected)
            {
                _logger.LogWarning("ProductId {ProductId} already reviewed by admin. Current ApprovalStatusId {ApprovalStatusId}", product.ProductId, product.ProductApprovalStatusId);
                throw new DataApprovalStatusException("Product Already Reviewed by admin");
            }
            if (product.ProductApprovalStatusId != (int)ProductApprovalStatusEnum.Vendor_Approved)
            {
                _logger.LogWarning("ProductId {ProductId} cannot be reviewed. Not yet Vendor_Approved. Current ApprovalStatusId {ApprovalStatusId}", product.ProductId, product.ProductApprovalStatusId);
                throw new DataApprovalStatusException("Product is not approved by vendor yet");
            }
            var adminUser = await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);

            int previousStatus = product.ProductApprovalStatusId;

            ApprovalHistory approvalHistory = new ApprovalHistory();
            approvalHistory.PreviousStatusId = previousStatus;
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
                _logger.LogError("Failed to update ProductId {ProductId}", product.ProductId);
                throw new DataRegistrationException("Product Not Updated for review");
            }

            var createdHistory = await _approvalHistoryRepsository.Create(approvalHistory);
            if (createdHistory == null)
            {
                _logger.LogError("Failed to create approval history for ProductId {ProductId}", product.ProductId);
                throw new DataRegistrationException("Approval History is not created for the product review");
            }

            var logChanges = new LogChanges
            {
                TableName = nameof(Product),
                RecordId = updatedProduct.ProductId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductId={product.ProductId}, ProductApprovalStatusId={previousStatus}",
                NewValue = $"ProductId={updatedProduct.ProductId}, ProductApprovalStatusId={updatedProduct.ProductApprovalStatusId}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for ProductId {ProductId}", updatedProduct.ProductId);

            var ownerVendor = await _vendorUserRepsository.GetProductVendorUserByVendorId(updatedProduct.VendorId);
            foreach (var item in ownerVendor)
            {
                string title = "";
                string message = "";
                if (requestReviewOfProductDTO.ApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved)
                {
                    title = "Product Approved";
                    message = $"Your product '{updatedProduct.ProductName}' has been approved by admin.";
                }
                else if (requestReviewOfProductDTO.ApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Rejected)
                {
                    title = "Product Rejected";
                    message = $"Your product '{updatedProduct.ProductName}' has been rejected by admin. Reason: {requestReviewOfProductDTO.Remarks}";
                }
                await _notificationService.SendToUser(
                    item.UserId,
                    title,
                    message,
                    notificationTypeId: (int)NotificationTypeEnum.ProductReviewed,
                    referenceType: "Product",
                    referenceId: updatedProduct.ProductId);

                _logger.LogInformation("Product review notification sent to Vendor Owner UserId {UserId} for ProductId {ProductId}", item.UserId, updatedProduct.ProductId);
            }

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for ProductId {ProductId}", updatedProduct.ProductId);

            return _mapper.Map<ResponseReviewOfProductDTO>(updatedProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while reviewing ProductId {ProductId}", requestReviewOfProductDTO.ProductId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for ProductId {ProductId}", requestReviewOfProductDTO.ProductId);
            throw;
        }
    }

    public async Task<ResponseReviewOfProductVariantDTO> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Admin UserId {AdminUserId} reviewing ProductVariantId {ProductVariantId} with ApprovalStatusId {ApprovalStatusId}", adminUserId, requestReviewOfProductDTO.ProductVariantId, requestReviewOfProductDTO.ApprovalStatusId);

            var product = await _productValidation.AdminValidateProductVariant(requestReviewOfProductDTO.ProductVariantId);
            if (product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved || product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Rejected)
            {
                _logger.LogWarning("ProductVariantId {ProductVariantId} already reviewed by admin. Current ApprovalStatusId {ApprovalStatusId}", product.ProductVariantId, product.ProductApprovalStatusId);
                throw new DataApprovalStatusException("Product Already Reviewed by admin");
            }
            if (product.ProductApprovalStatusId != (int)ProductApprovalStatusEnum.Vendor_Approved)
            {
                _logger.LogWarning("ProductVariantId {ProductVariantId} cannot be reviewed. Not yet Vendor_Approved. Current ApprovalStatusId {ApprovalStatusId}", product.ProductVariantId, product.ProductApprovalStatusId);
                throw new DataApprovalStatusException("Product Variant is not approved by vendor yet");
            }
            var adminUser = await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);

            int previousStatus = product.ProductApprovalStatusId;

            ApprovalHistory approvalHistory = new ApprovalHistory();
            approvalHistory.PreviousStatusId = previousStatus;
            approvalHistory.EntityType = "Product_Variant";
            approvalHistory.EntityId = product.ProductVariantId;
            approvalHistory.ReviewerType = "Admin";
            approvalHistory.ReviewerId = adminUser.AdminUserId;
            approvalHistory.Remarks = requestReviewOfProductDTO.Remarks;
            approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;

            product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
            product.UpdatedAt = DateTime.Now;

            var updatedVariant = await _productVariantRepsository.Update(product.ProductVariantId, product);
            if (updatedVariant == null)
            {
                _logger.LogError("Failed to update ProductVariantId {ProductVariantId}", product.ProductVariantId);
                throw new DataRegistrationException("Product Variant Not Updated for review");
            }

            var createdHistory = await _approvalHistoryRepsository.Create(approvalHistory);
            if (createdHistory == null)
            {
                _logger.LogError("Failed to create approval history for ProductVariantId {ProductVariantId}", product.ProductVariantId);
                throw new DataRegistrationException("Approval History is not created for the variant review");
            }

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductVariant),
                RecordId = updatedVariant.ProductVariantId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductVariantId={product.ProductVariantId}, ProductApprovalStatusId={previousStatus}",
                NewValue = $"ProductVariantId={updatedVariant.ProductVariantId}, ProductApprovalStatusId={updatedVariant.ProductApprovalStatusId}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for ProductVariantId {ProductVariantId}", updatedVariant.ProductVariantId);

            if (updatedVariant.Product == null)
            {
                _logger.LogWarning("Parent Product not loaded for ProductVariantId {ProductVariantId}. Skipping vendor notification", updatedVariant.ProductVariantId);
            }
            else
            {
                var ownerVendor = await _vendorUserRepsository.GetProductVendorUserByVendorId(updatedVariant.Product.VendorId);
                foreach (var item in ownerVendor)
                {
                    string title = "";
                    string message = "";
                    if (requestReviewOfProductDTO.ApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved)
                    {
                        title = "Product Variant Approved";
                        message = $"Your product '{updatedVariant.ProductVariantId}' has been approved by admin.";
                    }
                    else if (requestReviewOfProductDTO.ApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Rejected)
                    {
                        title = "Product Variant Rejected";
                        message = $"Your product '{updatedVariant.ProductVariantId}' has been rejected by admin. Reason: {requestReviewOfProductDTO.Remarks}";
                    }
                    await _notificationService.SendToUser(
                        item.UserId,
                        title,
                        message,
                        notificationTypeId: (int)NotificationTypeEnum.ProductReviewed,
                        referenceType: "Product_Variant",
                        referenceId: updatedVariant.ProductVariantId);

                    _logger.LogInformation("Product review notification sent to Vendor Owner UserId {UserId} for ProductVariantId {ProductVariantId}", item.UserId, updatedVariant.ProductVariantId);
                }
            }

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for ProductVariantId {ProductVariantId}", updatedVariant.ProductVariantId);

            return _mapper.Map<ResponseReviewOfProductVariantDTO>(updatedVariant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while reviewing ProductVariantId {ProductVariantId}", requestReviewOfProductDTO.ProductVariantId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for ProductVariantId {ProductVariantId}", requestReviewOfProductDTO.ProductVariantId);
            throw;
        }
    }
}