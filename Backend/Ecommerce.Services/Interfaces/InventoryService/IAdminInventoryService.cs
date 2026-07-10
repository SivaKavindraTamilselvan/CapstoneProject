using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminInventoryService
{
    public Task<PagedResponse<ResponseAdminInventoryDTO>> GetInventory(RequestAdminInventoryFilter request, int adminUserId);
    public Task<ResponseVendorInventoryDTO> GetInventoryById(int inventoryid, int adminUserId);

}