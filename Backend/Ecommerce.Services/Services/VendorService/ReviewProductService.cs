using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorService : IVendorService
{
    public async Task<ResponseReviewOfProductDTO> ReviewProductByVendor(RequestReviewOfProductDTO requestReviewOfProductDTO, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            var User = await _vendorUserValidation.ValidateProductVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor UserId {VendorUserId} validated for VendorId {VendorId}", vendorUserId, User.VendorId);

            await _productValidation.VendorValidateProduct(requestReviewOfProductDTO.ProductId, User.VendorId);
            _logger.LogInformation("ProductId {ProductId} validated for VendorId {VendorId}", requestReviewOfProductDTO.ProductId, User.VendorId);
            var vendor = await _vendorValidation.ValidateVendorIfApproved(User.VendorId);


            _logger.LogInformation("Vendor UserId {VendorUserId} reviewing ProductId {ProductId} with StatusId {StatusId}", vendorUserId, requestReviewOfProductDTO.ProductId, requestReviewOfProductDTO.ApprovalStatusId);
            if (requestReviewOfProductDTO.ApprovalStatusId != (int)ProductApprovalStatusEnum.Vendor_Approved && requestReviewOfProductDTO.ApprovalStatusId != (int)ProductApprovalStatusEnum.Vendor_Rejected)
            {
                _logger.LogWarning("Invalid approval status {StatusId} provided by Vendor UserId {VendorUserId}", requestReviewOfProductDTO.ApprovalStatusId, vendorUserId);
                throw new InvalidOperationException("Vendor can only approve or reject a product");
            }

            var product = await _productValidation.ValidateProduct(requestReviewOfProductDTO.ProductId);
            if (product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Vendor_Approved || product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Vendor_Rejected)
            {
                _logger.LogWarning("ProductId {ProductId} already reviewed with StatusId {StatusId}", product.ProductId, product.ProductApprovalStatusId);
                throw new InvalidOperationException("Product already reviewed");
            }

            var vendorUser = await _vendorUserValidation.ValidateProductVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor UserId {VendorUserId} resolved to VendorUserId {VendorUserId2}", vendorUserId, vendorUser.VendorUserId);

            int previousStatusId = product.ProductApprovalStatusId;

            ApprovalHistory approvalHistory = new ApprovalHistory();
            approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
            approvalHistory.EntityType = "Product";
            approvalHistory.EntityId = product.ProductId;
            approvalHistory.ReviewerType = "Vendor";
            approvalHistory.ReviewerId = vendorUser.VendorUserId;
            approvalHistory.Remarks = requestReviewOfProductDTO.Remarks;
            approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;
            product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;

            product = await _productRepsository.Update(product.ProductId, product);
            if (product == null)
            {
                _logger.LogError("Failed to update ProductId {ProductId}", requestReviewOfProductDTO.ProductId);
                throw new DataRegistrationException("Updation of the product failed");
            }
            _logger.LogInformation("ProductId {ProductId} updated successfully. ApprovalStatusId {ApprovalStatusId}", product.ProductId, product.ProductApprovalStatusId);

            var updatedproduct = await _productRepsository.Get(product!.ProductId);
            if (updatedproduct == null)
            {
                _logger.LogError("Failed to retrieve updated ProductId {ProductId}", product.ProductId);
                throw new DataNotFoundException("Product Not Found");
            }

            var createdApprovalHistory = await _approvalHistoryRepsository.Create(approvalHistory);
            if (createdApprovalHistory == null)
            {
                _logger.LogError("Failed to create approval history for ProductId {ProductId}", product.ProductId);
                throw new DataRegistrationException("Approval history creation failed.");
            }
            _logger.LogInformation("Approval history created for ProductId {ProductId}. PreviousStatus {PreviousStatus}, NewStatus {NewStatus}", product.ProductId, approvalHistory.PreviousStatusId, approvalHistory.NewStatusId);

            var productLog = new LogChanges
            {
                TableName = nameof(Product),
                RecordId = product.ProductId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductId={product.ProductId}, ProductApprovalStatusId={previousStatusId}",
                NewValue = $"ProductId={product.ProductId}, ProductApprovalStatusId={product.ProductApprovalStatusId}",
                UserId = vendorUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(productLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", productLog.TableName, productLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", productLog.TableName, productLog.RecordId);

            _logger.LogInformation("ProductId {ProductId} reviewed successfully by Vendor UserId {VendorUserId}. Status changed from {OldStatus} to {NewStatus}", product.ProductId, vendorUserId, approvalHistory.PreviousStatusId, approvalHistory.NewStatusId);
            await transaction.CommitAsync();

            _logger.LogInformation(
                "ProductId {ProductId} reviewed successfully by Vendor UserId {VendorUserId}. Status changed from {OldStatus} to {NewStatus}",
                product.ProductId,
                vendorUserId,
                approvalHistory.PreviousStatusId,
                approvalHistory.NewStatusId);

            return _mapper.Map<ResponseReviewOfProductDTO>(updatedproduct);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex,
                "Transaction failed while reviewing ProductId {ProductId}, VendorUserId {VendorUserId}",
                requestReviewOfProductDTO.ProductId,
                vendorUserId);

            throw;
        }
    }

    public async Task<ResponseReviewOfProductVariantDTO> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} reviewing ProductVariantId {ProductVariantId} with StatusId {StatusId}", vendorUserId, requestReviewOfProductDTO.ProductVariantId, requestReviewOfProductDTO.ApprovalStatusId);

            if (requestReviewOfProductDTO.ApprovalStatusId != (int)ProductApprovalStatusEnum.Vendor_Approved && requestReviewOfProductDTO.ApprovalStatusId != (int)ProductApprovalStatusEnum.Vendor_Rejected)
            {
                _logger.LogWarning("Invalid approval status {StatusId} provided by Vendor UserId {VendorUserId}", requestReviewOfProductDTO.ApprovalStatusId, vendorUserId);
                throw new InvalidOperationException("Vendor can only approve or reject a product");
            }

            var product = await _productValidation.AdminValidateProductVariant(requestReviewOfProductDTO.ProductVariantId);
            if (product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Vendor_Approved || product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Vendor_Rejected)
            {
                _logger.LogWarning("ProductVariantId {ProductVariantId} already reviewed with StatusId {StatusId}", product.ProductVariantId, product.ProductApprovalStatusId);
                throw new InvalidOperationException("Product already reviewed");
            }

            var vendorUser = await _vendorUserValidation.ValidateProductVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor UserId {VendorUserId} resolved to VendorUserId {VendorUserId2}", vendorUserId, vendorUser.VendorUserId);

            await _productValidation.ValidateProductVariant(product.ProductVariantId, vendorUserId);
            _logger.LogInformation("ProductVariantId {ProductVariantId} validated for Vendor UserId {VendorUserId}", product.ProductVariantId, vendorUserId);

            var vendor = await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);


            int previousStatusId = product.ProductApprovalStatusId;

            ApprovalHistory approvalHistory = new ApprovalHistory();
            approvalHistory.PreviousStatusId = product.ProductApprovalStatusId;
            approvalHistory.EntityType = "Product_Variant";
            approvalHistory.EntityId = product.ProductVariantId;
            approvalHistory.ReviewerType = "Vendor";
            approvalHistory.ReviewerId = vendorUser.VendorUserId;
            approvalHistory.Remarks = requestReviewOfProductDTO.Remarks;
            approvalHistory.NewStatusId = requestReviewOfProductDTO.ApprovalStatusId;
            product.ProductApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
            product.UpdatedAt = DateTime.Now;

            var updated = await _productVariantRepsository.Update(product.ProductVariantId, product);
            if (updated == null)
            {
                _logger.LogError("Failed to update ProductVariantId {ProductVariantId}", product.ProductVariantId);
                throw new DataRegistrationException("Updation of the product variant failed");
            }
            _logger.LogInformation("ProductVariantId {ProductVariantId} updated successfully. ApprovalStatusId {ApprovalStatusId}", updated.ProductVariantId, updated.ProductApprovalStatusId);

            var createdApprovalHistory = await _approvalHistoryRepsository.Create(approvalHistory);
            if (createdApprovalHistory == null)
            {
                _logger.LogError("Failed to create approval history for ProductVariantId {ProductVariantId}", product.ProductVariantId);
                throw new DataRegistrationException("Approval history creation failed.");
            }
            _logger.LogInformation("Approval history created for ProductVariantId {ProductVariantId}. PreviousStatus {PreviousStatus}, NewStatus {NewStatus}", product.ProductVariantId, approvalHistory.PreviousStatusId, approvalHistory.NewStatusId);

            var productVariantLog = new LogChanges
            {
                TableName = "ProductVariant",
                RecordId = product.ProductVariantId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductVariantId={product.ProductVariantId}, ProductApprovalStatusId={previousStatusId}",
                NewValue = $"ProductVariantId={updated.ProductVariantId}, ProductApprovalStatusId={updated.ProductApprovalStatusId}",
                UserId = vendorUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(productVariantLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", productVariantLog.TableName, productVariantLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", productVariantLog.TableName, productVariantLog.RecordId);

            _logger.LogInformation("ProductVariantId {ProductVariantId} reviewed successfully by Vendor UserId {VendorUserId}. Status changed from {OldStatus} to {NewStatus}", product.ProductVariantId, vendorUserId, approvalHistory.PreviousStatusId, approvalHistory.NewStatusId);
            await transaction.CommitAsync();

            _logger.LogInformation(
                "ProductVariantId {ProductVariantId} reviewed successfully by Vendor UserId {VendorUserId}. Status changed from {OldStatus} to {NewStatus}",
                product.ProductVariantId,
                vendorUserId,
                approvalHistory.PreviousStatusId,
                approvalHistory.NewStatusId);

            return _mapper.Map<ResponseReviewOfProductVariantDTO>(product);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex,
                "Transaction failed while reviewing ProductVariantId {ProductVariantId}, VendorUserId {VendorUserId}",
                requestReviewOfProductDTO.ProductVariantId,
                vendorUserId);

            throw;
        }
    }
}