using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class InventoryService : IInventoryService
{
    public async Task<ResponseUpdateInventoryDTO> DeleteInventory(int inventoryId, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} requested deletion of InventoryId {InventoryId}", vendorUserId, inventoryId);

            var vendorUser = await _vendorUserValidation.ValidateInventoryVendorUserByUserId(vendorUserId);
            _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendorUser.VendorId);

            await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
            _logger.LogInformation("Vendor {VendorId} is approved and eligible to delete inventory", vendorUser.VendorId);

            var inventory = await _inventoryValidation.ValidateInventory(inventoryId);
            _logger.LogInformation("InventoryId {InventoryId} validated successfully", inventory.InventoryId);

            await _userValidation.ValidateAddress(inventory.AddressId, vendorUserId);
            _logger.LogInformation("AddressId {AddressId} validated successfully", inventory.AddressId);

            await _inventoryValidation.VendorValidateInventory(inventoryId, vendorUser.VendorId);
            _logger.LogInformation("InventoryId {InventoryId} ownership validated for VendorId {VendorId}", inventory.InventoryId, vendorUser.VendorId);

            bool previousIsActive = inventory.IsActive;
            inventory.IsActive = false;

            var updatedInventory = await _inventoryRepsository.Update(inventory.InventoryId, inventory);
            if (updatedInventory == null)
            {
                _logger.LogError("Failed to deactivate InventoryId {InventoryId}", inventory.InventoryId);
                throw new DataRegistrationException("Deletion of the inventory failed");
            }
            _logger.LogInformation("InventoryId {InventoryId} deactivated successfully by Vendor UserId {VendorUserId}", updatedInventory.InventoryId, vendorUserId);

            var inventoryLog = new LogChanges
            {
                TableName = nameof(Inventory),
                RecordId = updatedInventory.InventoryId,
                Actions = (int)AuditAction.Deleted,
                OldValue = $"InventoryId={inventory.InventoryId}, IsActive={previousIsActive}",
                NewValue = $"InventoryId={updatedInventory.InventoryId}, IsActive={updatedInventory.IsActive}",
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
                _logger.LogInformation("Sending inventory deletion notification to owner vendor user of VendorId {VendorId}", vendorUser.VendorId);
                await _notificationService.SendToUser(
                    ownerUser.UserId,
                    "Inventory Deleted",
                    $"Inventory for ProductVariantId '{updatedInventory.ProductVariantId}' at AddressId '{updatedInventory.AddressId}' has been deactivated.",
                    notificationTypeId: (int)NotificationTypeEnum.InventoryDeleted,
                    referenceType: "Inventory",
                    referenceId: updatedInventory.InventoryId);
                _logger.LogInformation("Inventory deletion notification sent to owner UserId {UserId}", ownerUser.UserId);
            }
            else
            {
                _logger.LogWarning("No owner vendor user found for VendorId {VendorId}. Skipping owner notification", vendorUser.VendorId);
            }

            await transaction.CommitAsync();

            return _mapper.Map<ResponseUpdateInventoryDTO>(updatedInventory);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex,
                "Transaction failed while deleting InventoryId {InventoryId}, VendorUserId {VendorUserId}",
                inventoryId,
                vendorUserId);

            throw;
        }
    }
}