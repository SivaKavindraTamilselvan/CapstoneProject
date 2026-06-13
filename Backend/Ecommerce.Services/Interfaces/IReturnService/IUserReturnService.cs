using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IUserReturnService
{
    public Task<PagedResponse<ReturnSummaryDto>> GetAllReturnsForUser(RequestUserReturnFilter request, int UserId);
    public Task<ResponseAddReturnDTO> AddReturn(RequestAddReturnDTO requestAddReturnDTO);
}