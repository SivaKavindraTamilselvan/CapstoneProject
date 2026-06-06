using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class InventoryValidation : IInventoryValidation
{
    private readonly IInventoryRepsository _inventoryRepsository;
    public InventoryValidation(InventoryRepsository inventoryRepsository,IUserValidation userValidation,IProductValidation productValidation)
    {
        _inventoryRepsository = inventoryRepsository;
    }
    public async Task<Inventory> ValidateInventory(int inventoryId)
    {
        var inventory = await _inventoryRepsository.Get(inventoryId);
        if(inventory == null)
        {
            throw new DataNotFoundException("Inventory Is Not Found");
        }
        return inventory;
    }
}