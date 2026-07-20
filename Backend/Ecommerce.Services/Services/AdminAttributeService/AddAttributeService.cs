using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<ResponseAddAttributeDTO> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO, int adminuserid)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Attribute creation initiated by UserId {UserId} for AttributeName {AttributeName}", adminuserid, requestAddAttributeDTO.AttributeName);

            var admin = await _adminUserValidation.ValidateProductAdminUserByUserId(adminuserid);
            await _productAttributeValidation.ValidateAttributeName(requestAddAttributeDTO.AttributeName);

            _logger.LogInformation("Validation completed for AttributeName {AttributeName}", requestAddAttributeDTO.AttributeName);

            AttributeMaster createAttribute = new AttributeMaster();
            createAttribute.AttributeName = requestAddAttributeDTO.AttributeName;
            createAttribute.AddedByAdminId = admin.AdminUserId;

            var createdAttribute = await _attributeRepsository.Create(createAttribute);
            if (createdAttribute == null)
            {
                _logger.LogError("Failed to create Attribute with AttributeName {AttributeName} by AdminUserId {AdminUserId}", createAttribute.AttributeName, admin.AdminUserId);
                throw new DataRegistrationException("Attribute registration failed.");
            }
            _logger.LogInformation("Attribute created successfully with AttributeMasterId {AttributeMasterId} by AdminUserId {AdminUserId}", createdAttribute.AttributeMasterId, admin.AdminUserId);


            var logChanges = new LogChanges
            {
                TableName = nameof(AttributeMaster),
                RecordId = createdAttribute.AttributeMasterId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"AttributeMasterId={createdAttribute.AttributeMasterId}, AttributeName={createdAttribute.AttributeName}, AddedByAdminId={createdAttribute.AddedByAdminId}, IsActive={createdAttribute.IsActive}",
                UserId = adminuserid,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for AttributeMasterId {AttributeMasterId}", createdAttribute.AttributeMasterId);
            
            var vendorOwnerIds = await _vendorUserRepsository.GetAllProductVendorUserIds();

            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "New Attribute Added",
                    $"A new attribute {createdAttribute.AttributeName} has been created. Please review your products.",
                    notificationTypeId: (int)NotificationTypeEnum.Attribute,
                    referenceType: "AttributeMaster",
                    referenceId: createdAttribute.AttributeMasterId);
            }

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed for AttributeMasterId {AttributeMasterId}", createdAttribute.AttributeMasterId);

            return _mapper.Map<ResponseAddAttributeDTO>(createdAttribute);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while creating Attribute {AttributeName}", requestAddAttributeDTO.AttributeName);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for AttributeName {AttributeName}", requestAddAttributeDTO.AttributeName);
            throw;
        }
    }
    public async Task<ResponseAddProductSubCategoryAttributeDTO> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO, int adminuserid)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("ProductSubCategoryAttribute creation initiated by UserId {UserId}. AttributeMasterId {AttributeMasterId}, ProductSubCategoryId {ProductSubCategoryId}", adminuserid, requestAddProductSubCategoryAttributeDTO.AttributeMasterId, requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId);

            var admin = await _adminUserValidation.ValidateProductAdminUserByUserId(adminuserid);
            await _productAttributeValidation.ValidateProductSubCategoryAttributeForAdmin(requestAddProductSubCategoryAttributeDTO.AttributeMasterId, requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId);
            var mappedattribute = await _productSubCategoryAttributeRepsository.CheckProductSubCategoryAttribute(requestAddProductSubCategoryAttributeDTO.AttributeMasterId, requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId);
            if (mappedattribute != null)
            {
                throw new DataAlreadyRegisteredException("Data Already mapped with this inputs");
            }
            _logger.LogInformation("Validation completed for AttributeMasterId {AttributeMasterId} and ProductSubCategoryId {ProductSubCategoryId}", requestAddProductSubCategoryAttributeDTO.AttributeMasterId, requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId);

            ProductSubCategoryAttribute productSubCategoryAttribute = new ProductSubCategoryAttribute();
            productSubCategoryAttribute.AttributeMasterId = requestAddProductSubCategoryAttributeDTO.AttributeMasterId;
            productSubCategoryAttribute.ProductSubCategoryId = requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId;
            productSubCategoryAttribute.AddedByAdminId = admin.AdminUserId; var createdProductSubCategoryAttribute = await _productSubCategoryAttributeRepsository.Create(productSubCategoryAttribute);
            if (createdProductSubCategoryAttribute == null)
            {
                _logger.LogError("Failed to create ProductSubCategoryAttribute for AttributeMasterId {AttributeMasterId}, ProductSubCategoryId {ProductSubCategoryId} by AdminUserId {AdminUserId}",
                    productSubCategoryAttribute.AttributeMasterId,
                    productSubCategoryAttribute.ProductSubCategoryId,
                    admin.AdminUserId);

                throw new DataRegistrationException("Product SubCategory Attribute registration failed.");
            }

            _logger.LogInformation("ProductSubCategoryAttribute created successfully with Id {ProductSubCategoryAttributeId} by AdminUserId {AdminUserId}", createdProductSubCategoryAttribute.ProductSubCategoryAttributeId, admin.AdminUserId);

            var logChanges = new LogChanges
            {
                TableName = nameof(ProductSubCategoryAttribute),
                RecordId = createdProductSubCategoryAttribute.ProductSubCategoryAttributeId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"ProductSubCategoryAttributeId={createdProductSubCategoryAttribute.ProductSubCategoryAttributeId}, AttributeMasterId={createdProductSubCategoryAttribute.AttributeMasterId}, ProductSubCategoryId={productSubCategoryAttribute.ProductSubCategoryId}, AddedByAdminId={createdProductSubCategoryAttribute.AddedByAdminId}, IsActive={createdProductSubCategoryAttribute.IsActive}",
                UserId = adminuserid,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", logChanges.TableName, logChanges.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

            _logger.LogInformation("Audit log created for ProductSubCategoryAttributeId {ProductSubCategoryAttributeId}", createdProductSubCategoryAttribute.ProductSubCategoryAttributeId);

            var vendorOwnerIds = await _vendorUserRepsository.GetAllProductVendorUserIds();

            foreach (var vendorUserId in vendorOwnerIds)
            {
                await _notificationService.SendToUser(
                    vendorUserId,
                    "New Mapped Attribute Added",
                    "A new attribute has been mapped to a product subcategory. Please review your products.",
                    notificationTypeId: (int)NotificationTypeEnum.MappedAttribute,
                    referenceType: "AttributeMaster",
                    referenceId: createdProductSubCategoryAttribute.ProductSubCategoryAttributeId);
            }

             await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed for ProductSubCategoryAttributeId {ProductSubCategoryAttributeId}", createdProductSubCategoryAttribute.ProductSubCategoryAttributeId);


            return _mapper.Map<ResponseAddProductSubCategoryAttributeDTO>(createdProductSubCategoryAttribute);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed while creating ProductSubCategoryAttribute");

            await transaction.RollbackAsync();

            _logger.LogInformation("Transaction rolled back for ProductSubCategoryAttribute");

            throw;
        }

    }
}