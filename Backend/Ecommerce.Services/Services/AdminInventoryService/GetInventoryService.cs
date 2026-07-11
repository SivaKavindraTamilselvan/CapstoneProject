using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class AdminInventoryService : IAdminInventoryService
{
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly IInventoryRepsository _inventoryRepsository;
    private readonly IMapper _mapper;
    public AdminInventoryService(IInventoryRepsository inventoryRepsository, IMapper mapper, IAdminUserValidation adminUserValidation)
    {
        _inventoryRepsository = inventoryRepsository;
        _adminUserValidation = adminUserValidation;
        _mapper = mapper;
    }
    public async Task<PagedResponse<ResponseAdminInventoryDTO>> GetInventory(RequestAdminInventoryFilter request, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        var result = await _inventoryRepsository.GetInventoryForAdmin(request);
        return new PagedResponse<ResponseAdminInventoryDTO>
        {
            Items = _mapper.Map<List<ResponseAdminInventoryDTO>>(result.items),
            TotalCount = result.totalQuantity,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<ResponseAdminInventoryDTO> GetInventoryById(int inventoryid, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        var result = await _inventoryRepsository.GetInventoryById(inventoryid);
        if (result == null)
        {
            throw new DataNotFoundException("Inventory Not Found");
        }

        return _mapper.Map<ResponseAdminInventoryDTO>(result);
    }
}