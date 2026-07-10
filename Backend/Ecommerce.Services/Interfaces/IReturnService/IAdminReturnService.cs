using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminReturnService
{
    public Task<PagedResponse<ReturnSummaryDto>> GetAllReturnsForAdmin(RequestAdminReturnFilter request);
    public Task<ResponseCreateReturnShipmentDTO> CreateReturnShipment(int returnId);
}