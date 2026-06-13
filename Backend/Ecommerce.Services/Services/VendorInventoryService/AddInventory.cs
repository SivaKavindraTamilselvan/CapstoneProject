using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class InventoryService : IInventoryService
{
   
    public async Task<ResponseAddInventoryDTO> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO,int vendorUserId)
    {
        await _productValidation.ValidateProductVariant(requestAddInventoryDTO.ProductVariantId,vendorUserId);
        await _userValidation.ValidateAddress(requestAddInventoryDTO.AddressId,vendorUserId);
        var inventory = _mapper.Map<Inventory>(requestAddInventoryDTO);
        await _inventoryRepsository.Create(inventory);
        return _mapper.Map<ResponseAddInventoryDTO>(inventory);
    }
}