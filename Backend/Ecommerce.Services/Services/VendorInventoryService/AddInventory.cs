using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class InventoryService : IInventoryService
{
   
    public async Task<ResponseAddInventoryDTO> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO,int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} adding inventory for ProductVariantId {ProductVariantId} at AddressId {AddressId}",vendorUserId,requestAddInventoryDTO.ProductVariantId,requestAddInventoryDTO.AddressId);
        await _productValidation.ValidateProductVariant(requestAddInventoryDTO.ProductVariantId,vendorUserId);
        await _userValidation.ValidateAddress(requestAddInventoryDTO.AddressId,vendorUserId);
        var inventory = _mapper.Map<Inventory>(requestAddInventoryDTO);
        await _inventoryRepsository.Create(inventory);
        _logger.LogInformation("Inventory created successfully. InventoryId {InventoryId}, ProductVariantId {ProductVariantId}, AddressId {AddressId}",inventory.InventoryId,inventory.ProductVariantId,inventory.AddressId);
        return _mapper.Map<ResponseAddInventoryDTO>(inventory);
    }
}