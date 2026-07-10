using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IInventoryRepsository : IRepository<int, Inventory>
{
    public Task<Inventory?> GetInventoryByVendorId(int vendorId, int inventoryid);
    public Task<Inventory?> GetInventoryById(int inventoryid);
    public Task<(List<Inventory> items, int totalQuantity)> GetInventoryForAdmin(RequestAdminInventoryFilter request);
    public Task<(List<Inventory> items, int totalQuantity)> GetInventoryForVendor(RequestVendorInventoryFilter request, int vendorId);
}