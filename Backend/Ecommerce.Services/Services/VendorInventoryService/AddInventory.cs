using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class InventoryService : IInventoryService
{
    public async Task<ResponseAddInventoryDTO> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} adding inventory for ProductVariantId {ProductVariantId} at AddressId {AddressId}", vendorUserId, requestAddInventoryDTO.ProductVariantId, requestAddInventoryDTO.AddressId);

            var vendorUser = await _vendorUserValidation.ValidateInventoryVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
            _logger.LogInformation("Vendor {VendorId} is approved and eligible to add inventory", vendorUser.VendorId);

            await _productValidation.ValidateProductVariant(requestAddInventoryDTO.ProductVariantId, vendorUserId);
            _logger.LogInformation("ProductVariantId {ProductVariantId} validated successfully", requestAddInventoryDTO.ProductVariantId);

            await _userValidation.ValidateAddress(requestAddInventoryDTO.AddressId, vendorUserId);
            _logger.LogInformation("AddressId {AddressId} validated successfully", requestAddInventoryDTO.AddressId);

            var inventory = _mapper.Map<Inventory>(requestAddInventoryDTO);
            if (inventory == null)
            {
                _logger.LogError("Failed to map RequestAddInventoryDTO to Inventory entity for ProductVariantId {ProductVariantId}", requestAddInventoryDTO.ProductVariantId);
                throw new NullReferenceException("Inventory mapping failed");
            }

            var createdInventory = await _inventoryRepsository.Create(inventory);
            if (createdInventory == null)
            {
                _logger.LogError("Failed to create Inventory for ProductVariantId {ProductVariantId}, AddressId {AddressId}", requestAddInventoryDTO.ProductVariantId, requestAddInventoryDTO.AddressId);
                throw new DataRegistrationException("Inventory creation failed");
            }
            _logger.LogInformation("Inventory created successfully. InventoryId {InventoryId}, ProductVariantId {ProductVariantId}, AddressId {AddressId}", createdInventory.InventoryId, createdInventory.ProductVariantId, createdInventory.AddressId);

            var inventoryLog = new LogChanges
            {
                TableName = nameof(Inventory),
                RecordId = createdInventory.InventoryId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"InventoryId={createdInventory.InventoryId}, ProductVariantId={createdInventory.ProductVariantId}, AddressId={createdInventory.AddressId}, Quantity={createdInventory.AvailableQuantity}",
                UserId = vendorUserId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(inventoryLog);
            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", inventoryLog.TableName, inventoryLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for TableName {TableName}, RecordId {RecordId}", inventoryLog.TableName, inventoryLog.RecordId);

            var ownerUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendorUser.VendorId);
            if (ownerUser != null)
            {
                _logger.LogInformation("Sending inventory added notification to owner vendor user of VendorId {VendorId}", vendorUser.VendorId);
                await _notificationService.SendToUser(
                    ownerUser.UserId,
                    "New Inventory Added",
                    $"New inventory has been added for ProductVariantId '{createdInventory.ProductVariantId}' at AddressId '{createdInventory.AddressId}'.",
                    notificationTypeId: (int)NotificationTypeEnum.InventoryAdded,
                    referenceType: "Inventory",
                    referenceId: createdInventory.InventoryId);
                _logger.LogInformation("Inventory added notification sent to owner UserId {UserId}", ownerUser.UserId);
            }
            else
            {
                _logger.LogWarning("No owner vendor user found for VendorId {VendorId}. Skipping owner notification", vendorUser.VendorId);
            }

            await transaction.CommitAsync();

            return _mapper.Map<ResponseAddInventoryDTO>(createdInventory);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex,
                "Transaction failed while adding inventory for ProductVariantId {ProductVariantId}, AddressId {AddressId}, VendorUserId {VendorUserId}",
                requestAddInventoryDTO.ProductVariantId,
                requestAddInventoryDTO.AddressId,
                vendorUserId);

            throw;
        }
    }
}