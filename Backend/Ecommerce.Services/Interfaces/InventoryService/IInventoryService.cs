using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IInventoryService
{
    public Task<ResponseAddInventoryDTO> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO,int vendorUserId);
    public Task<ResponseUpdateInventoryDTO> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO);

}