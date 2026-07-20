using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IInventoryValidation
{
        public  Task<Inventory> VendorValidateInventory(int inventoryId,int vendorId);
    public Task<Inventory> ValidateInventory(int inventoryId);
}