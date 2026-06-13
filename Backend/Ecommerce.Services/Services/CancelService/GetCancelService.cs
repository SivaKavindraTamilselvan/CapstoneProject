using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class CancelService : ICancelService
{
    public async Task<List<CancelSummaryDto>> GetAllCancelsForAdmin(RequestGetCancelsForAdminDTO request)
    {

        var result = await _cancelRepsository.GetAllCancelsForAdmin(request);

        if (!result.data.Any())
        {
            throw new DataNotFoundException("No cancel requests found");
        }

        var cancels = _mapper.Map<List<CancelSummaryDto>>(result.data);
        return cancels;
    }

    public async Task<List<CancelSummaryDto>> GetAllCancelsForUser(RequestGetCancelsForVendorDTO request, int user)
    {

        var result = await _cancelRepsository.GetAllCancelsForUser(request, user);
        if (!result.data.Any())
        {
            throw new DataNotFoundException("No cancel requests found");
        }

        var cancels = _mapper.Map<List<CancelSummaryDto>>(result.data);
        return cancels;
    }
    public async Task<List<CancelSummaryDto>> GetAllCancelsForVendor(RequestGetCancelsForVendorDTO request, int user)
    {

        var result = await _cancelRepsository.GetAllCancelsForUser(request, user);
        if (!result.data.Any())
        {
            throw new DataNotFoundException("No cancel requests found");
        }

        var cancels = _mapper.Map<List<CancelSummaryDto>>(result.data);
        return cancels;
    }
}
