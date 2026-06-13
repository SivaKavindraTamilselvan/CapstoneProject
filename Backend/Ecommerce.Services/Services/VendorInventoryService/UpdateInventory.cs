using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class InventoryService : IInventoryService
{
    
    public async Task<ResponseUpdateInventoryDTO> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO,int vendorUserId)
    {
        var inventory = await _inventoryValidation.ValidateInventory(requestUpdateInventoryDTO.InventoryId);
        await _userValidation.ValidateAddress(inventory.AddressId,vendorUserId);
        var updateInventory = _mapper.Map<Inventory>(requestUpdateInventoryDTO);
        await _inventoryRepsository.Update(inventory.InventoryId, updateInventory);
        return _mapper.Map<ResponseUpdateInventoryDTO>(updateInventory);
    }
}