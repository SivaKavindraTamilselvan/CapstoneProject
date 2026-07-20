using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductVariantService : IVendorProductVariantService
{
    public async Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} started adding product variant for ProductId {ProductId}", vendorUserId, requestAddProductVariantDTO.ProductId);

            var vendorUser = await _vendorUserValidation.ValidateProductVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
            _logger.LogInformation("Vendor {VendorId} is approved and eligible to add product variants", vendorUser.VendorId);

            var product = await _productValidation.ValidateProductIfApproved(requestAddProductVariantDTO.ProductId);
            await _productValidation.VendorValidateProduct(product.ProductId, vendorUser.VendorId);
            _logger.LogInformation("ProductId {ProductId} validated and ownership confirmed for VendorId {VendorId}", product.ProductId, vendorUser.VendorId);

            if (requestAddProductVariantDTO.ProductVariantAttribute == null || !requestAddProductVariantDTO.ProductVariantAttribute.Any())
            {
                _logger.LogWarning("Variant creation failed for ProductId {ProductId}: attributes are missing", requestAddProductVariantDTO.ProductId);
                throw new InvalidDataException("Variant attributes are required");
            }

            if (!requestAddProductVariantDTO.ProductVariantAttribute.Any(x => x.ProductSubCategoryAttributeId == product.MainProductSubCategoryAttributeId))
            {
                _logger.LogWarning("Variant creation failed for ProductId {ProductId}: main attribute missing", product.ProductId);
                throw new InvalidDataException("Main attribute value is required");
            }

            await _productAttributeValidation.ValidateProductSubCategoryAttribute(product.MainProductSubCategoryAttributeId, product.ProductSubCategoryId);
            _logger.LogInformation("Main subcategory attribute {AttributeId} validated for ProductId {ProductId}", product.MainProductSubCategoryAttributeId, product.ProductId);

            var duplicateAttribute = requestAddProductVariantDTO.ProductVariantAttribute.GroupBy(x => x.ProductSubCategoryAttributeId).FirstOrDefault(g => g.Count() > 1);
            if (duplicateAttribute != null)
            {
                _logger.LogWarning("Duplicate variant attribute found for ProductId {ProductId}, ProductSubCategoryAttributeId {AttributeId}", product.ProductId, duplicateAttribute.Key);
                throw new InvalidDataException("Duplicate variant attributes are not allowed");
            }

            var productVariant = _mapper.Map<ProductVariant>(requestAddProductVariantDTO);
            if (productVariant == null)
            {
                _logger.LogError("Failed to map RequestAddProductVariantDTO to ProductVariant entity for ProductId {ProductId}", requestAddProductVariantDTO.ProductId);
                throw new NullReferenceException("Product variant mapping failed");
            }
            productVariant.AddedByVendorUserId = vendorUser.VendorUserId;
            productVariant.SKU = await GenerateSku(product.ProductId);

            bool isOwner = vendorUser.VendorRoleId == (int)VendorRoleEnum.Owner;
            if (isOwner)
            {
                _logger.LogInformation("Vendor UserId {VendorUserId} is Owner. Auto-approving new variant SKU {SKU}", vendorUserId, productVariant.SKU);
                productVariant.ProductApprovalStatusId = (int)ProductApprovalStatusEnum.Vendor_Approved;
            }

            var createdVariant = await _productVariantRepsository.Create(productVariant);
            if (createdVariant == null)
            {
                _logger.LogError("Failed to create Product Variant for ProductId {ProductId}, SKU {SKU}", product.ProductId, productVariant.SKU);
                throw new DataRegistrationException("Product variant creation failed");
            }
            _logger.LogInformation("Product Variant {ProductVariantId} created for ProductId {ProductId} with SKU {SKU}", createdVariant.ProductVariantId, product.ProductId, createdVariant.SKU);

            var variantLog = new LogChanges
            {
                TableName = nameof(ProductVariant),
                RecordId = createdVariant.ProductVariantId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"ProductVariantId={createdVariant.ProductVariantId}, ProductId={createdVariant.ProductId}, SKU={createdVariant.SKU}, ProductApprovalStatusId={createdVariant.ProductApprovalStatusId}",
                UserId = vendorUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(variantLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", variantLog.TableName, variantLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", variantLog.TableName, variantLog.RecordId);

            foreach (var list in requestAddProductVariantDTO.ProductVariantAttribute)
            {
                RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO = new RequestAddProductVariantAttributeDTO();
                requestAddProductVariantAttributeDTO.AttributeValue = list.AttributeValue;
                requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId = list.ProductSubCategoryAttributeId;
                requestAddProductVariantAttributeDTO.ProductVariantId = createdVariant.ProductVariantId;

                await AddProductVariantAttribute(requestAddProductVariantAttributeDTO, false, vendorUserId);
            }
            _logger.LogInformation("All {Count} variant attributes processed for ProductVariantId {ProductVariantId}", requestAddProductVariantDTO.ProductVariantAttribute.Count(), createdVariant.ProductVariantId);

            // Case 1: non-owner vendor user added the variant -> notify the vendor owner for internal review
            if (!isOwner)
            {
                _logger.LogInformation("Sending product variant review notification to owner vendor user of VendorId {VendorId}", vendorUser.VendorId);
                var ownerUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendorUser.VendorId);
                if (ownerUser != null)
                {
                    await _notificationService.SendToUser(
                        ownerUser.UserId,
                        "New Product Variant Submitted",
                        $"A new variant '{createdVariant.SKU}' for product '{product.ProductName}' has been submitted and is waiting for approval.",
                        notificationTypeId: (int)NotificationTypeEnum.ProductAdded,
                        referenceType: "ProductVariant",
                        referenceId: createdVariant.ProductVariantId);
                    _logger.LogInformation("Product variant review notification sent to owner UserId {UserId}", ownerUser.UserId);
                }
                else
                {
                    _logger.LogWarning("No owner vendor user found for VendorId {VendorId}. Skipping owner notification", vendorUser.VendorId);
                }
            }
            // Case 2: owner added the variant directly (already vendor-approved) -> notify all product admins for platform review
            else
            {
                var productAdminUserIds = await _adminUserRepsository.GetProductAdminUserIds();
                _logger.LogInformation("Sending new product variant notification to {AdminCount} product admins for ProductVariantId {ProductVariantId}", productAdminUserIds.Count, createdVariant.ProductVariantId);

                if (productAdminUserIds.Count == 0)
                {
                    _logger.LogWarning("No product admin users found to notify for ProductVariantId {ProductVariantId}", createdVariant.ProductVariantId);
                }

                foreach (var adminUserId in productAdminUserIds)
                {
                    await _notificationService.SendToUser(
                        adminUserId,
                        "New Product Variant Submitted",
                        $"A new variant '{createdVariant.SKU}' for product '{product.ProductName}' has been submitted and is waiting for approval.",
                        notificationTypeId: (int)NotificationTypeEnum.ProductSubmitted,
                        referenceType: "ProductVariant",
                        referenceId: createdVariant.ProductVariantId);
                    _logger.LogInformation("Product variant notification sent to product admin UserId {UserId}", adminUserId);
                }

                _logger.LogInformation("Product variant notifications sent successfully for ProductVariantId {ProductVariantId}", createdVariant.ProductVariantId);
            }
            await transaction.CommitAsync();
            return _mapper.Map<ResponseAddProductVariantDTO>(createdVariant);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction failed while adding product variant. ProductId {ProductId}, VendorUserId {VendorUserId}", requestAddProductVariantDTO.ProductId, vendorUserId);
            throw;
        }
    }

    public async Task<ResponseAddProductVariantAttributeDTO> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO, bool updation, int userId)
    {
        _logger.LogInformation("Vendor UserId {UserId} started processing attribute for ProductVariantId {ProductVariantId}", userId, requestAddProductVariantAttributeDTO.ProductVariantId);

        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(userId);
        var productVariant = await _productValidation.ValidateProductVariant(requestAddProductVariantAttributeDTO.ProductVariantId, userId);
        var product = await _productValidation.ValidateProduct(productVariant.ProductId);
        _logger.LogInformation("ProductVariantId {ProductVariantId} and ProductId {ProductId} validated", productVariant.ProductVariantId, product.ProductId);

        var productVariantAttribute = _mapper.Map<ProductVariantAttribute>(requestAddProductVariantAttributeDTO);
        if (productVariantAttribute == null)
        {
            _logger.LogError("Failed to map RequestAddProductVariantAttributeDTO for ProductVariantId {ProductVariantId}", requestAddProductVariantAttributeDTO.ProductVariantId);
            throw new NullReferenceException("Product variant attribute mapping failed");
        }

        await _productAttributeValidation.ValidateProductSubCategoryAttribute(requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId, product.ProductSubCategoryId);
        _logger.LogInformation("ProductSubCategoryAttributeId {AttributeId} validated for ProductSubCategoryId {SubCategoryId}", requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId, product.ProductSubCategoryId);

        if (updation)
        {
            var result = await _productVariantAttributeRepsository.CheckAttributeAlreadyAdded(requestAddProductVariantAttributeDTO.ProductVariantId, requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId);
            if (result != null)
            {
                _logger.LogWarning("Attribute {AttributeId} already added for ProductVariantId {ProductVariantId}", requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId, requestAddProductVariantAttributeDTO.ProductVariantId);
                throw new DataAlreadyRegisteredException("Product Attribute Already Added");
            }
            productVariantAttribute.UpdatedAt = DateTime.Now;
        }

        productVariantAttribute.AddedByVendorUserId = vendorUser.VendorUserId;

        var createdAttribute = await _productVariantAttributeRepsository.Create(productVariantAttribute);
        if (createdAttribute == null)
        {
            _logger.LogError("Failed to create ProductVariantAttribute for ProductVariantId {ProductVariantId}", requestAddProductVariantAttributeDTO.ProductVariantId);
            throw new DataRegistrationException("Product variant attribute creation failed");
        }
        _logger.LogInformation("ProductVariantAttribute {AttributeId} created for ProductVariantId {ProductVariantId}", createdAttribute.ProductVariantAttributeId, createdAttribute.ProductVariantId);

        var attributeLog = new LogChanges
        {
            TableName = nameof(ProductVariantAttribute),
            RecordId = createdAttribute.ProductVariantAttributeId,
            Actions = updation ? (int)AuditAction.Updated : (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"ProductVariantAttributeId={createdAttribute.ProductVariantAttributeId}, ProductVariantId={createdAttribute.ProductVariantId}, ProductSubCategoryAttributeId={createdAttribute.ProductSubCategoryAttributeId}, AttributeValue={createdAttribute.AttributeValue}",
            UserId = userId,
            ChangedAt = DateTime.Now
        };

        var createdLog = await _logChanges.Create(attributeLog);
        if (createdLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", attributeLog.TableName, attributeLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
        }
        _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", attributeLog.TableName, attributeLog.RecordId);

        var ownerUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendorUser.VendorId);
        if (ownerUser != null)
        {
            await _notificationService.SendToUser(
                ownerUser.UserId,
                "New Product Variant Attribute Submitted",
                $"A new product variant attribute for '{product.ProductName}' has been submitted and is waiting for approval.",
                notificationTypeId: (int)NotificationTypeEnum.ProductAdded,
                referenceType: "Product",
                referenceId: product.ProductId);
            _logger.LogInformation("Product variant attribute notification sent to owner UserId {UserId}", ownerUser.UserId);
        }
        else
        {
            _logger.LogWarning("No owner vendor user found for VendorId {VendorId}. Skipping owner notification", vendorUser.VendorId);
        }

        return _mapper.Map<ResponseAddProductVariantAttributeDTO>(createdAttribute);
    }

    private async Task<string> GenerateSku(int productId)
    {
        string sku;
        do
        {
            var randomCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            sku = $"PV-{productId:D6}-{randomCode}";

        } while ((await _productVariantRepsository.GetAll()).Any(v => v.SKU == sku));

        return sku;
    }
}