using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class CancelService : ICancelService
{
    public async Task<PagedResponse<CancelSummaryDto>> GetAllCancelsForAdmin(RequestAdminCancelFilter request)
    {
        var result = await _cancelRepsository.GetAllCancelsForAdmin(request);
        if (result.data.Count == 0)
        {
            throw new DataNotFoundException("No cancel requests found");
        }
        return new PagedResponse<CancelSummaryDto>
        {
            Items = _mapper.Map<List<CancelSummaryDto>>(result.data),
            TotalCount = result.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResponse<CancelSummaryDto>> GetAllCancelsForUser(RequestUserCancelFilter request, int user)
    {
        var result = await _cancelRepsository.GetAllCancelsForUser(request, user);
        if (result.data.Count == 0)
        {
            throw new DataNotFoundException("No cancel requests found");
        }
        return new PagedResponse<CancelSummaryDto>
        {
            Items = _mapper.Map<List<CancelSummaryDto>>(result.data),
            TotalCount = result.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<PagedResponse<CancelSummaryDto>> GetAllCancelsForVendor(RequestVendorCancelFilter request, int user)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(user);
        var result = await _cancelRepsository.GetAllCancelsForVendor(request, vendor.VendorId);
        if (result.data.Count == 0)
        {
            throw new DataNotFoundException("No cancel requests found");
        }
        return new PagedResponse<CancelSummaryDto>
        {
            Items = _mapper.Map<List<CancelSummaryDto>>(result.data),
            TotalCount = result.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<CancelSummaryDto> GetAllCancel(int cancelId, int user)
    {
        var cancel = await _cancelRepsository.GetCancelById(cancelId);
        return _mapper.Map<CancelSummaryDto>(cancel);
    }


}
