using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<ResponseAddProductCategoryDTO> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO, int adminuserid)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Admin UserId {AdminUserId} is adding Product Category {CategoryName}", adminuserid, requestAddProductCategoryDTO.ProductCategoryName);

            // validate admin user so only valid admin user can add the category
            var admin = await _adminUserValidation.ValidateProductAdminUserByUserId(adminuserid);
            // validate the category name already registered or not
            await _productCategoryValidation.ValidateProductCategoryName(requestAddProductCategoryDTO.ProductCategoryName);

            ProductCategory productCategory = new ProductCategory
            {
                ProductCategoryName = requestAddProductCategoryDTO.ProductCategoryName,
                AddedByAdminId = admin.AdminUserId
            };

            // create the category 
            var createdCategory = await _productCategoryRepsository.Create(productCategory);
            if (createdCategory == null)
            {
                _logger.LogError("Failed to create Product Category with CategoryName {CategoryName} by AdminUserId {AdminUserId}", productCategory.ProductCategoryName, admin.AdminUserId);
                throw new DataRegistrationException("Product Category registration failed.");
            }

            _logger.LogInformation("Product Category {CategoryId} - {CategoryName} created successfully", productCategory.ProductCategoryId, productCategory.ProductCategoryName);

            // log service
            var logChanges = new LogChanges
            {
                TableName = nameof(ProductCategory),
                RecordId = createdCategory.ProductCategoryId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"ProductCategoryId={createdCategory.ProductCategoryId}, ProductCategoryName={createdCategory.ProductCategoryName}, IsActive={createdCategory.IsActive}",
                UserId = adminuserid,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for ProductCategoryId {CategoryId}", productCategory.ProductCategoryId);

            // notification service
            var vendorOwnerIds = await _vendorUserRepsository.GetAllProductVendorUserIds();
            _logger.LogInformation("Sending category creation notification to {VendorCount} vendors", vendorOwnerIds.Count);

            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "New Category Added",
                    $"A new product category '{createdCategory.ProductCategoryName}' has been added and is available for use.",
                    notificationTypeId: (int)NotificationTypeEnum.Category,
                    referenceType: "ProductCategory",
                    referenceId: createdCategory.ProductCategoryId);
            }
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAddProductCategoryDTO>(createdCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while creating ProductCategory {CategoryName}", requestAddProductCategoryDTO.ProductCategoryName);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back while creating ProductCategory {CategoryName}", requestAddProductCategoryDTO.ProductCategoryName);
            throw;
        }
    }
    public async Task<ResponseAddProductSubCategoryDTO> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO, int adminuserid)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Admin UserId {AdminUserId} is adding Product SubCategory {SubCategoryName}", adminuserid, requestAddProductSubCategoryDTO.ProductSubCategoryName);

            var admin = await _adminUserValidation.ValidateProductAdminUserByUserId(adminuserid);

            // validate category is active and present
            await _productCategoryValidation.ValidateCategory(requestAddProductSubCategoryDTO.ProductCategoryId);
            //validate sub category name
            await _productCategoryValidation.ValidateProductSubCategoryName(requestAddProductSubCategoryDTO.ProductSubCategoryName);

            ProductSubCategory productSubCategory = new ProductSubCategory
            {
                ProductCategoryId = requestAddProductSubCategoryDTO.ProductCategoryId,
                ProductSubCategoryName = requestAddProductSubCategoryDTO.ProductSubCategoryName,
                CommissionPercentage = requestAddProductSubCategoryDTO.CommissionPercentage,
                AddedByAdminId = admin.AdminUserId
            };

            var createdSubCategory = await _productSubCategoryRepsository.Create(productSubCategory);
            if (createdSubCategory == null)
            {
                _logger.LogError("Failed to create Product SubCategory with SubCategoryName {SubCategoryName} by AdminUserId {AdminUserId}", productSubCategory.ProductSubCategoryName, admin.AdminUserId);
                throw new DataRegistrationException("Product SubCategory registration failed.");
            }

            _logger.LogInformation("Product SubCategory {SubCategoryId} - {SubCategoryName} created successfully", productSubCategory.ProductSubCategoryId, productSubCategory.ProductSubCategoryName);

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductSubCategory),
                RecordId = createdSubCategory.ProductSubCategoryId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"ProductSubCategoryId={createdSubCategory.ProductSubCategoryId}, ProductSubCategoryName={createdSubCategory.ProductSubCategoryName}, ProductCategoryId={createdSubCategory.ProductCategoryId}, CommissionPercentage={createdSubCategory.CommissionPercentage}, IsActive={createdSubCategory.IsActive}",
                UserId = adminuserid,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for ProductSubCategoryId {SubCategoryId}", createdSubCategory.ProductSubCategoryId);

            var vendorOwnerIds = await _vendorUserRepsository.GetAllProductVendorUserIds();

            _logger.LogInformation("Sending subcategory creation notification to {VendorCount} vendors", vendorOwnerIds.Count);

            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "New SubCategory Added",
                    $"A new product subcategory '{createdSubCategory.ProductSubCategoryName}' has been added and is available for use.",
                    notificationTypeId: (int)NotificationTypeEnum.SubCategory,
                    referenceType: "ProductSubCategory",
                    referenceId: createdSubCategory.ProductSubCategoryId);
            }
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAddProductSubCategoryDTO>(createdSubCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while creating ProductSubCategory {SubCategoryName}", requestAddProductSubCategoryDTO.ProductSubCategoryName);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back while creating ProductSubCategory {SubCategoryName}", requestAddProductSubCategoryDTO.ProductSubCategoryName);
            throw;
        }
    }
}