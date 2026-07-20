using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductService : IVendorProductService
{
    public async Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            // validate the vendor user if active and role
            _logger.LogInformation("Vendor UserId {VendorUserId} initiated product creation. ProductName: {ProductName}", vendorUserId, requestAddProduct.ProductName);
            var vendorUser = await _vendorUserValidation.ValidateProductVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            // validate vendor company is active
            var vendor = await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
            _logger.LogInformation("Vendor {VendorId} is approved and eligible to create products", vendor.VendorId);

            // validate the category and subcategory of the product if valid and active
            await _productCategoryValidation.ValidateSubCategory(requestAddProduct.ProductSubCategoryId);
            _logger.LogInformation("Product SubCategory {SubCategoryId} validated successfully", requestAddProduct.ProductSubCategoryId);

            var product = _mapper.Map<Product>(requestAddProduct);
            if (product == null)
            {
                _logger.LogError("Failed to map RequestAddProduct to Product entity. ProductName: {ProductName}", requestAddProduct.ProductName);
                throw new NullReferenceException("Product mapping failed");
            }
            product.VendorId = vendor.VendorId;
            product.AddedByVendorUserId = vendorUser.VendorUserId;

            // if vendor user is owner then directly approved by vendor
            bool isOwner = vendorUser.VendorRoleId == (int)VendorRoleEnum.Owner;
            if (isOwner)
            {
                _logger.LogInformation("Vendor UserId {VendorUserId} is Owner. Auto-approving new product {ProductName}", vendorUserId, product.ProductName);
                product.ProductApprovalStatusId = (int)ProductApprovalStatusEnum.Vendor_Approved;
            }

            var createdProduct = await _productRepsository.Create(product);
            if (createdProduct == null)
            {
                _logger.LogError("Failed to create Product. ProductName: {ProductName}, VendorId: {VendorId}", product.ProductName, vendor.VendorId);
                throw new DataRegistrationException("Product creation failed");
            }
            _logger.LogInformation("Product created successfully. ProductId: {ProductId}, ProductName: {ProductName}, VendorId: {VendorId}", createdProduct.ProductId, createdProduct.ProductName, vendor.VendorId);

            var productLog = new LogChanges
            {
                TableName = nameof(Product),
                RecordId = createdProduct.ProductId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"ProductId={createdProduct.ProductId}, ProductName={createdProduct.ProductName}, VendorId={createdProduct.VendorId}, ProductApprovalStatusId={createdProduct.ProductApprovalStatusId}",
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

            if (!isOwner)
            {
                _logger.LogInformation("Sending product review notification to owner vendor user of VendorId {VendorId}", vendor.VendorId);
                var ownerUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendor.VendorId);
                if (ownerUser != null)
                {
                    await _notificationService.SendToUser(
                        ownerUser.UserId,
                        "New Product Submitted",
                        $"A new product '{createdProduct.ProductName}' has been submitted and is waiting for approval.",
                        notificationTypeId: (int)NotificationTypeEnum.ProductAdded,
                        referenceType: "Product",
                        referenceId: createdProduct.ProductId);
                    _logger.LogInformation("Product review notification sent to owner UserId {UserId}", ownerUser.UserId);
                }
                else
                {
                    _logger.LogWarning("No owner vendor user found for VendorId {VendorId}. Skipping owner notification", vendor.VendorId);
                }
            }
            else
            {
                var productAdminUserIds = await _adminUserRepsository.GetProductAdminUserIds();
                _logger.LogInformation("Sending product review notification to {AdminCount} product admins for ProductId {ProductId}", productAdminUserIds.Count, createdProduct.ProductId);

                if (productAdminUserIds.Count == 0)
                {
                    _logger.LogWarning("No product admin users found to notify for ProductId {ProductId}", createdProduct.ProductId);
                }

                foreach (var adminUserId in productAdminUserIds)
                {
                    await _notificationService.SendToUser(
                        adminUserId,
                        "New Product Submitted",
                        $"A new product '{createdProduct.ProductName}' has been submitted and is waiting for approval.",
                        notificationTypeId: (int)NotificationTypeEnum.ProductSubmitted,
                        referenceType: "Product",
                        referenceId: createdProduct.ProductId);
                    _logger.LogInformation("Product review notification sent to product admin UserId {UserId}", adminUserId);
                }

                _logger.LogInformation("Product review notifications sent successfully for ProductId {ProductId}", createdProduct.ProductId);
            }
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAddProduct>(createdProduct);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while adding product. ProductName: {ProductName}, VendorUserId: {VendorUserId}", requestAddProduct.ProductName, vendorUserId);
            _logger.LogInformation("Transaction rolled back while adding product. ProductName: {ProductName}, VendorUserId: {VendorUserId}", requestAddProduct.ProductName, vendorUserId);
            throw;
        }
    }
}