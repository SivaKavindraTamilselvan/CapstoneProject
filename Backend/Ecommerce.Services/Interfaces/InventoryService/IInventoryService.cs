using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IInventoryService
{
    public Task<PagedResponse<ResponseVendorInventoryDTO>> GetInventory(RequestVendorInventoryFilter request, int vendorUserId);
    public Task<ResponseVendorInventoryDTO> GetInventoryById(int inventoryid, int vendorUserId);
    public Task<ResponseUpdateInventoryDTO> DeleteInventory(int inventoryId, int vendorUserId);
    public Task<ResponseAddInventoryDTO> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO, int vendorUserId);
    public Task<ResponseUpdateInventoryDTO> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO, int vendorUserId);

}