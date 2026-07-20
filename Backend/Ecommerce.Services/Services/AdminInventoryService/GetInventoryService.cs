using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class AdminInventoryService : IAdminInventoryService
{
    private readonly IAdminUserValidation _adminUserValidation;
    private readonly IInventoryRepsository _inventoryRepsository;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminInventoryService> _logger;

    public AdminInventoryService(IInventoryRepsository inventoryRepsository, IMapper mapper, IAdminUserValidation adminUserValidation, ILogger<AdminInventoryService> logger)
    {
        _inventoryRepsository = inventoryRepsository;
        _adminUserValidation = adminUserValidation;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResponse<ResponseAdminInventoryDTO>> GetInventory(RequestAdminInventoryFilter request, int adminUserId)
    {
        _logger.LogInformation("Inventory list retrieval initiated by AdminUserId {AdminUserId}. PageNumber={PageNumber}, PageSize={PageSize}", adminUserId, request.PageNumber, request.PageSize);

        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        var result = await _inventoryRepsository.GetInventoryForAdmin(request);
        if (result.totalQuantity == 0)
        {
            _logger.LogWarning("Inventory not found");
            throw new DataNotFoundException("No inventory list is found");
        }
        _logger.LogInformation("Inventory list retrieved successfully. TotalRecords={TotalCount}", result.totalQuantity);

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
        _logger.LogInformation("Inventory retrieval initiated for InventoryId {InventoryId} by AdminUserId {AdminUserId}", inventoryid, adminUserId);
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        var result = await _inventoryRepsository.GetInventoryById(inventoryid);
        if (result == null)
        {
            _logger.LogWarning("Inventory not found for InventoryId {InventoryId}", inventoryid);
            throw new DataNotFoundException("Inventory Not Found");
        }
        _logger.LogInformation("Inventory retrieved successfully for InventoryId {InventoryId}", inventoryid);
        return _mapper.Map<ResponseAdminInventoryDTO>(result);
    }
}