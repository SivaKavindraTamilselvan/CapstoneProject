using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class InventoryService : IInventoryService
{
    private readonly IProductValidation _productValidation;
    private readonly IUserValidation _userValidation;
    private readonly IInventoryRepsository _inventoryRepsository;
    private readonly IInventoryValidation _inventoryValidation;

    private readonly IMapper _mapper;
    public InventoryService(IProductValidation productValidation, IMapper mapper, IInventoryRepsository inventoryRepsository, IUserValidation userValidation, IInventoryValidation inventoryValidation)
    {
        _productValidation = productValidation;
        _inventoryRepsository = inventoryRepsository;
        _userValidation = userValidation;
        _inventoryValidation = inventoryValidation;
        _mapper = mapper;

    }
    public async Task<ResponseAddInventoryDTO> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO,int vendorUserId)
    {
        await _productValidation.ValidateProductVariant(requestAddInventoryDTO.ProductVariantId,vendorUserId);
        await _userValidation.ValidateAddress(requestAddInventoryDTO.AddressId,vendorUserId);
        var inventory = _mapper.Map<Inventory>(requestAddInventoryDTO);
        await _inventoryRepsository.Create(inventory);
        return _mapper.Map<ResponseAddInventoryDTO>(inventory);
    }

    public async Task<ResponseUpdateInventoryDTO> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO)
    {
        var inventory = await _inventoryValidation.ValidateInventory(requestUpdateInventoryDTO.InventoryId);
        var updateInventory = _mapper.Map<Inventory>(requestUpdateInventoryDTO);
        await _inventoryRepsository.Update(inventory.InventoryId, updateInventory);
        return _mapper.Map<ResponseUpdateInventoryDTO>(updateInventory);
    }
    public async Task<ResponseUpdateInventoryDTO> DeleteInventory(int inventoryId)
    {
        var inventory = await _inventoryValidation.ValidateInventory(inventoryId);
        inventory.IsActive = false;
        await _inventoryRepsository.Update(inventory.InventoryId, inventory);
        return _mapper.Map<ResponseUpdateInventoryDTO>(inventory);
    }
}