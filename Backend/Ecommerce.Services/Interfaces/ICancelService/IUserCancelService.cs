namespace Ecommerce.Services.Interfaces;

public interface ICancelService
{
    public Task<List<CancelSummaryDto>> GetAllCancelsForAdmin(RequestGetCancelsForAdminDTO request);
    public Task<List<CancelSummaryDto>> GetAllCancelsForVendor(RequestGetCancelsForVendorDTO request, int user);
    public Task<List<CancelSummaryDto>> GetAllCancelsForUser(RequestGetCancelsForVendorDTO request, int user);
    public Task<ResponseCancelDTO> RequestCancel(RequestCancelDTO requestCancelDTO);
}