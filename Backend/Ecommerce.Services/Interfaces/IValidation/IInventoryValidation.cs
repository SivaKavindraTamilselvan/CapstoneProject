using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IInventoryValidation
{
    public Task<Inventory> ValidateInventory(int inventoryId);
}