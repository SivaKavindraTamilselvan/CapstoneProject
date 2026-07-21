using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class InventoryService : IInventoryService
{
    public async Task<ResponseUpdateInventoryDTO> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO, int vendorUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("Vendor UserId {VendorUserId} requested update for InventoryId {InventoryId}", vendorUserId, requestUpdateInventoryDTO.InventoryId);

            var vendorUser = await _vendorUserValidation.ValidateInventoryVendorUserByUserId(vendorUserId);
            await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);

            var inventory = await _inventoryRepsository.Get(requestUpdateInventoryDTO.InventoryId);
            if(inventory == null)
            {
                throw new DataNotFoundException("Inventory Id is not found");
            }

            int previousAvailableQuantity = inventory.AvailableQuantity;

            if (requestUpdateInventoryDTO.UpdateType == true)
            {
                inventory.AvailableQuantity = requestUpdateInventoryDTO.AvailableQuantity + inventory.AvailableQuantity;
            }
            else
            {
                if (inventory.AvailableQuantity < requestUpdateInventoryDTO.AvailableQuantity)
                {
                    throw new DataApprovalStatusException("Stock cannot be negative");
                }
                inventory.AvailableQuantity = inventory.AvailableQuantity - requestUpdateInventoryDTO.AvailableQuantity;
            }

            var updatedInventory = await _inventoryRepsository.Update(inventory.InventoryId, inventory);
            if (updatedInventory == null)
            {
                _logger.LogError("Failed to update InventoryId {InventoryId}", inventory.InventoryId);
                throw new DataRegistrationException("Updation of the inventory failed");
            }
            _logger.LogInformation("InventoryId {InventoryId} updated successfully by Vendor UserId {VendorUserId}", updatedInventory.InventoryId, vendorUserId);

            var inventoryLog = new LogChanges
            {
                TableName = nameof(Inventory),
                RecordId = updatedInventory.InventoryId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"InventoryId={inventory.InventoryId}, AvailableQuantity={previousAvailableQuantity}",
                NewValue = $"InventoryId={updatedInventory.InventoryId}, AvailableQuantity={updatedInventory.AvailableQuantity}",
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

            await transaction.CommitAsync();

            _logger.LogInformation("Transaction committed successfully for InventoryId {InventoryId}",
                updatedInventory.InventoryId);

            return _mapper.Map<ResponseUpdateInventoryDTO>(updatedInventory);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            _logger.LogError(ex,
                "Transaction failed while updating InventoryId {InventoryId}",
                requestUpdateInventoryDTO.InventoryId);

            _logger.LogInformation(
                "Transaction rolled back for InventoryId {InventoryId}",
                requestUpdateInventoryDTO.InventoryId);

            throw;
        }
    }
}