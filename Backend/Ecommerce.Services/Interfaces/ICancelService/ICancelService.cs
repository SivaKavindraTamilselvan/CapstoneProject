using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface ICancelService
{
    
    public Task<PagedResponse<CancelSummaryDto>> GetAllCancelsForAdmin(RequestAdminCancelFilter request);
    public Task<PagedResponse<CancelSummaryDto>> GetAllCancelsForVendor(RequestVendorCancelFilter request, int user);
    public Task<PagedResponse<CancelSummaryDto>> GetAllCancelsForUser(RequestUserCancelFilter request, int user);
    public Task<ResponseCancelDTO> RequestCancel(RequestCancelDTO requestCancelDTO);
}