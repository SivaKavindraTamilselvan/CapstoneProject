using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class InventoryService : IInventoryService
{
    
    public async Task<ResponseUpdateInventoryDTO> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO,int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested update for InventoryId {InventoryId}",vendorUserId,requestUpdateInventoryDTO.InventoryId);
        var inventory = await _inventoryValidation.ValidateInventory(requestUpdateInventoryDTO.InventoryId);
        await _userValidation.ValidateAddress(inventory.AddressId,vendorUserId);
        var updateInventory = _mapper.Map<Inventory>(requestUpdateInventoryDTO);
        await _inventoryRepsository.Update(inventory.InventoryId, updateInventory);
        _logger.LogInformation("InventoryId {InventoryId} updated successfully by Vendor UserId {VendorUserId}",inventory.InventoryId,vendorUserId);
        return _mapper.Map<ResponseUpdateInventoryDTO>(updateInventory);
    }
}