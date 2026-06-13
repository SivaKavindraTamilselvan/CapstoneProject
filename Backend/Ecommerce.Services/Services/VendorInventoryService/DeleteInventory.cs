using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class InventoryService : IInventoryService
{
    public async Task<ResponseUpdateInventoryDTO> DeleteInventory(int inventoryId,int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested deletion of InventoryId {InventoryId}",vendorUserId,inventoryId);

        var inventory = await _inventoryValidation.ValidateInventory(inventoryId);
        await _userValidation.ValidateAddress(inventory.AddressId,vendorUserId);
        inventory.IsActive = false;
        await _inventoryRepsository.Update(inventory.InventoryId, inventory);
        _logger.LogInformation("InventoryId {InventoryId} deactivated successfully by Vendor UserId {VendorUserId}",inventory.InventoryId,vendorUserId);

        return _mapper.Map<ResponseUpdateInventoryDTO>(inventory);
    }
}