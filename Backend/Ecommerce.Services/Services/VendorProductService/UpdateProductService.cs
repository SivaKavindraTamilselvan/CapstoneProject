using System.Security.Authentication;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductService : IVendorProductService
{
    public async Task<ResponseUpdateProduct> UpdateProduct(RequestUpdateProductStatus requestUpdateProduct, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} initiated product status update for ProductId {ProductId}", vendorUserId, requestUpdateProduct.ProductId);

            // validate the vendor user is active and role is validated
            var vendorUser = await _vendorUserValidation.ValidateProductVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            // validate the vendor company is approved
            await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
            _logger.LogInformation("Vendor {VendorId} is approved and eligible to update products", vendorUser.VendorId);

            // validate product exists
            var product = await _productValidation.ValidateProduct(requestUpdateProduct.ProductId);
            _logger.LogInformation("Product {ProductId} validated successfully for status update", product.ProductId);

            // validate the product belongs to this vendor
            await _productValidation.VendorValidateProduct(requestUpdateProduct.ProductId, vendorUser.VendorId);
            _logger.LogInformation("ProductId {ProductId} ownership validated for VendorId {VendorId}", product.ProductId, vendorUser.VendorId);

            // deleted product cannot be updated
            if (product.ProductStatusId == (int)ProductStatusEnum.Archived)
            {
                _logger.LogWarning("ProductId {ProductId} update rejected. Product is deleted (ProductStatusId=4)", product.ProductId);
                throw new DataApprovalStatusException("Deleted Product Cannot be updated");
            }

            if (product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Deleted_By_Admin)
            {
                _logger.LogWarning("ProductId {ProductId} update rejected. Product is deleted (ApprovalStatusId=5)", product.ProductId);
                throw new DataApprovalStatusException("Deleted Product Cannot be updated");
            }

            product = _mapper.Map<Product>(product);
            if (product == null)
            {
                _logger.LogError("Failed to map Product entity for ProductId {ProductId}", requestUpdateProduct.ProductId);
                throw new NullReferenceException("Product mapping failed");
            }

            int previousStatusId = product.ProductStatusId;
            product.UpdatedAt = DateTime.Now;
            product.ProductStatusId = requestUpdateProduct.ProductStatusId;
            _logger.LogInformation("Updating ProductId {ProductId} status from {OldStatus} to {NewStatus}", product.ProductId, previousStatusId, product.ProductStatusId);

            var updatedProduct = await _productRepsository.Update(product.ProductId, product);
            if (updatedProduct == null)
            {
                _logger.LogError("Failed to update ProductId {ProductId}", product.ProductId);
                throw new DataRegistrationException("Updation of the product failed");
            }
            _logger.LogInformation("ProductId {ProductId} status updated successfully", updatedProduct.ProductId);

            var productLog = new LogChanges
            {
                TableName = nameof(Product),
                RecordId = updatedProduct.ProductId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductId={product.ProductId}, ProductStatusId={previousStatusId}",
                NewValue = $"ProductId={updatedProduct.ProductId}, ProductStatusId={updatedProduct.ProductStatusId}",
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

            await transaction.CommitAsync();
            return _mapper.Map<ResponseUpdateProduct>(updatedProduct);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while updating product status. ProductId {ProductId}, VendorUserId {VendorUserId}", requestUpdateProduct.ProductId, vendorUserId);
            throw;
        }
    }

    public async Task<ResponseUpdateProduct> UpdateRejectedOrPendingProduct(RequestUpdateProduct requestUpdateProduct, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} initiated product update for ProductId {ProductId}", vendorUserId, requestUpdateProduct.ProductId);

            // validate the vendor user is active and role is validated
            var vendorUser = await _vendorUserValidation.ValidateOwnerVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            // validate the vendor company is approved
            await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
            _logger.LogInformation("Vendor {VendorId} is approved and eligible to update products", vendorUser.VendorId);

            // validate product exists
            var product = await _productValidation.ValidateProduct(requestUpdateProduct.ProductId);
            _logger.LogInformation("Product {ProductId} validated successfully for modification", product.ProductId);

            // validate the product belongs to this vendor
            await _productValidation.VendorValidateProduct(requestUpdateProduct.ProductId, vendorUser.VendorId);
            _logger.LogInformation("Current Product Approval Status: {ApprovalStatusId} for ProductId {ProductId}", product.ProductApprovalStatusId, product.ProductId);

            // already admin-reviewed products (approved or deleted) cannot be edited by the vendor
            if (product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved || product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Deleted_By_Admin)
            {
                _logger.LogWarning("ProductId {ProductId} update rejected. Product already admin-reviewed with ApprovalStatusId {ApprovalStatusId}", product.ProductId, product.ProductApprovalStatusId);
                throw new InvalidCredentialException("You Cannot update th admin approved product datas");
            }

            int previousApprovalStatusId = product.ProductApprovalStatusId;

            _mapper.Map(requestUpdateProduct, product);
            product.UpdatedAt = DateTime.Now;

            product.ProductApprovalStatusId = (int)ProductApprovalStatusEnum.Vendor_Approved;
            _logger.LogInformation("Updating ProductId {ProductId}. Product will be resubmitted for admin review", product.ProductId);

            var updatedProduct = await _productRepsository.Update(product.ProductId, product);
            if (updatedProduct == null)
            {
                _logger.LogError("Failed to update ProductId {ProductId}", product.ProductId);
                throw new DataRegistrationException("Updation of the product failed");
            }
            product = updatedProduct;
            _logger.LogInformation("ProductId {ProductId} updated successfully by Vendor UserId {VendorUserId}", product.ProductId, vendorUserId);

            var productLog = new LogChanges
            {
                TableName = nameof(Product),
                RecordId = product.ProductId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"ProductId={product.ProductId}, ProductApprovalStatusId={previousApprovalStatusId}",
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

            var productAdminUserIds = await _adminUserRepsository.GetProductAdminUserIds();
            _logger.LogInformation("Sending product update notification to {AdminCount} product admins for ProductId {ProductId}", productAdminUserIds.Count, product.ProductId);

            if (productAdminUserIds.Count == 0)
            {
                _logger.LogWarning("No product admin users found to notify for ProductId {ProductId}", product.ProductId);
            }

            foreach (var adminUserId in productAdminUserIds)
            {
                await _notificationService.SendToUser(
                    adminUserId,
                    "Product Updated By Vendor",
                    $"Product '{product.ProductName}' has been modified by the vendor and requires review.",
                    notificationTypeId: (int)NotificationTypeEnum.ProductSubmitted,
                    referenceType: "Product",
                    referenceId: product.ProductId);
                _logger.LogInformation("Product update notification sent to product admin UserId {UserId}", adminUserId);
            }

            _logger.LogInformation("Product review notifications sent successfully for ProductId {ProductId}", product.ProductId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseUpdateProduct>(updatedProduct);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while updating product. ProductId {ProductId}, VendorUserId {VendorUserId}", requestUpdateProduct.ProductId, vendorUserId);
            throw;
        }
    }
}