using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IInventoryService
{
    public Task<ResponseUpdateInventoryDTO> DeleteInventory(int inventoryId);
    public Task<ResponseAddInventoryDTO> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO,int vendorUserId);
    public Task<ResponseUpdateInventoryDTO> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO);

}