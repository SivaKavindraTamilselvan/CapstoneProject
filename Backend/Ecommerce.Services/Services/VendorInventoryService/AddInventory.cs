using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class InventoryService
{
    private readonly IProductValidation _productValidation;
    private readonly IAddressRepsository _addressRepsository;
    private readonly IInventoryRepsository _inventoryRepsository;
    private readonly IVendorValidation _vendorValidation;

    private readonly IMapper _mapper;
    public InventoryService(IProductValidation productValidation,IAddressRepsository addressRepsository,IMapper mapper,IInventoryRepsository inventoryRepsository,IVendorValidation vendorValidation)
    {
        _productValidation = productValidation;
        _addressRepsository = addressRepsository;
        _mapper = mapper;
        _inventoryRepsository = inventoryRepsository;
        _vendorValidation = vendorValidation;
    }
    public async Task<ResponseAddInventoryDTO> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO)
    {
        var product = await _productValidation.ValidateProductVariant(requestAddInventoryDTO.ProductVariantId);
        var address = await _addressRepsository.Get(requestAddInventoryDTO.AddressId);
        if(address == null)
        {
            throw new DataNotFoundException("Address is Not Found For This User");
        }
        var inventory = _mapper.Map<Inventory>(requestAddInventoryDTO);
        await _inventoryRepsository.Create(inventory);
        return _mapper.Map<ResponseAddInventoryDTO>(inventory);
    }

    public async Task<ResponseUpdateInventoryDTO> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO)
    {
        var inventory = await _vendorValidation.ValidateInventory(requestUpdateInventoryDTO.InventoryId);
        var updateInventory = _mapper.Map<Inventory>(requestUpdateInventoryDTO);
        await _inventoryRepsository.Update(inventory.InventoryId,updateInventory);
        return _mapper.Map<ResponseUpdateInventoryDTO>(updateInventory);
    }
}