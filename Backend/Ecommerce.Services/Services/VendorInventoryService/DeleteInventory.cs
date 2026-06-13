using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class InventoryService : IInventoryService
{
    public async Task<ResponseUpdateInventoryDTO> DeleteInventory(int inventoryId,int vendorUserId)
    {
        var inventory = await _inventoryValidation.ValidateInventory(inventoryId);
        await _userValidation.ValidateAddress(inventory.AddressId,vendorUserId);
        inventory.IsActive = false;
        await _inventoryRepsository.Update(inventory.InventoryId, inventory);
        return _mapper.Map<ResponseUpdateInventoryDTO>(inventory);
    }
}