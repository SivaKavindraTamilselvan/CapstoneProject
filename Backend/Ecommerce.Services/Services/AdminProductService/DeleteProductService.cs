using System.Linq.Expressions;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductService : IAdminProductService
{
    public async Task<ResponseReviewOfProductDTO> DeleteProduct(RequestDeleteProductDTO requestDeleteProductDTO, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Admin UserId {AdminUserId} initiated deletion of ProductId {ProductId}", adminUserId, requestDeleteProductDTO.ProductId);

            // validate if product is found or not
            var product = await _productValidation.ValidateProduct(requestDeleteProductDTO.ProductId);
            // to check if the product is already deleted. prevent from deleting already deleted product
            if (product.ProductApprovalStatusId == (int)ApprovalStatusEnum.Deleted_By_Admin)
            {
                throw new DataApprovalStatusException("Product Already Deleted By Admin");
            }
            // validate admin user based on the product admin and check if the admin is active or not
            var adminUser = await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);
            _logger.LogInformation("Product {ProductId} - {ProductName} found. Current Approval Status: {StatusId}", product.ProductId, product.ProductName, product.ProductApprovalStatusId);

            // approval history for the product
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

            int previousStatus = product.ProductApprovalStatusId;

            product.ProductApprovalStatusId = (int)ProductApprovalStatusEnum.Deleted_By_Admin;
            product.UpdatedAt = DateTime.Now;

            approvalHistory.PreviousStatusId = previousStatus;
            approvalHistory.NewStatusId = (int)ProductApprovalStatusEnum.Deleted_By_Admin;

            var updatedProduct = await _productRepsository.Update(product.ProductId, product);

            if (updatedProduct == null)
            {
                throw new DataRegistrationException("Product not deleted");
            }
            var createdHistory = await _approvalHistoryRepsository.Create(approvalHistory);

            if (createdHistory == null)
            {
                throw new DataRegistrationException("Approval History not created for the deletion of product by admin");
            }

            var logChanges = new LogChanges
            {
                TableName = nameof(Product),
                RecordId = updatedProduct.ProductId,
                Actions = (int)AuditAction.Deleted,
                OldValue = $"ProductId={product.ProductId}, ProductApprovalStatusId={previousStatus}",
                NewValue = $"ProductId={updatedProduct.ProductId}, ProductApprovalStatusId={(int)ProductApprovalStatusEnum.Deleted_By_Admin}",
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

            await transaction.CommitAsync();

            var ownerVendor = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(updatedProduct.VendorId);
            if (ownerVendor != null)
            {
                await _notificationService.SendToUser(
                    ownerVendor.UserId,
                    "Product Deleted By Admin",
                    $"Your product '{updatedProduct.ProductName}' has been deleted by admin. Reason: {requestDeleteProductDTO.Remarks}",
                    notificationTypeId: (int)NotificationTypeEnum.ProductDeleted,
                    referenceType: "Product",
                    referenceId: updatedProduct.ProductId);
                _logger.LogInformation("Product delete notification sent to Vendor Owner UserId {UserId} for ProductId {ProductId}", ownerVendor.UserId, product.ProductId);
            }
            return _mapper.Map<ResponseReviewOfProductDTO>(updatedProduct);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while deleting ProductId {ProductId}", requestDeleteProductDTO.ProductId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for ProductId {ProductId}", requestDeleteProductDTO.ProductId);
            throw;
        }
    }
    public async Task<ResponseReviewOfProductVariantDTO> DeleteProductVaraint(RequestDeleteVariantDTO requestDeleteProductDTO, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Admin UserId {AdminUserId} initiated deletion of ProductId {ProductId}", adminUserId, requestDeleteProductDTO.ProductVariantId);
            var product = await _productValidation.AdminValidateProductVariant(requestDeleteProductDTO.ProductVariantId);
            // to check if the product is already deleted. prevent from deleting already deleted product
            if (product.ProductApprovalStatusId == (int)ApprovalStatusEnum.Deleted_By_Admin)
            {
                throw new DataApprovalStatusException("Product Already Deleted By Admin");
            }
            // validate admin user based on the product admin and check if the admin is active or not
            var adminUser = await _adminUserValidation.ValidateProductAdminUserByUserId(adminUserId);
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

            int previousStatus = product.ProductApprovalStatusId;

            product.ProductApprovalStatusId = (int)ProductApprovalStatusEnum.Deleted_By_Admin;
            product.UpdatedAt = DateTime.UtcNow;

            approvalHistory.PreviousStatusId = previousStatus;
            approvalHistory.NewStatusId = (int)ProductApprovalStatusEnum.Deleted_By_Admin;

            var updatedVariant = await _productVariantRepsository.Update(product.ProductVariantId, product);

            if (updatedVariant == null)
            {
                throw new DataRegistrationException("Product Variant is not updated");
            }
            var createdHistory = await _approvalHistoryRepsository.Create(approvalHistory);

            if (createdHistory == null)
            {
                throw new DataRegistrationException("Approval History not created for the deletion of product variant by admin");
            }

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductVariant),
                RecordId = updatedVariant.ProductVariantId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductVariantId={product.ProductVariantId}, ProductApprovalStatusId={previousStatus}",
                NewValue = $"ProductVariantId={updatedVariant.ProductVariantId}, ProductApprovalStatusId={(int)ProductApprovalStatusEnum.Deleted_By_Admin}",
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

            await transaction.CommitAsync();

            var productDetails = await _productRepsository.Get(product.ProductId);
            var ownerVendor = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(productDetails!.VendorId);
            if (ownerVendor != null)
            {
                await _notificationService.SendToUser(
                    ownerVendor.UserId,
                    "Product Deleted By Admin",
                    $"Your product '{product.SKU}' has been deleted by admin. Reason: {requestDeleteProductDTO.Remarks}",
                    notificationTypeId: (int)NotificationTypeEnum.ProductDeleted,
                    referenceType: "Product_Variant",
                    referenceId: product.ProductVariantId);
                _logger.LogInformation("Product delete notification sent to Vendor Owner UserId {UserId} for ProductId {ProductId}", ownerVendor.UserId, product.ProductId);
            }
            return _mapper.Map<ResponseReviewOfProductVariantDTO>(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Transaction failed while deleting ProductVariantId {ProductVariantId}",
                requestDeleteProductDTO.ProductVariantId);

            await transaction.RollbackAsync();

            _logger.LogInformation("Transaction rolled back for ProductVariantId {ProductVariantId}",
                requestDeleteProductDTO.ProductVariantId);

            throw;
        }
    }
}