using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class CancelService : ICancelService
{
    public async Task<PagedResponse<CancelSummaryDto>> GetAllCancelsForAdmin(RequestAdminCancelFilter request)
    {
        _logger.LogInformation("Admin requested cancel list with filter {@Filter}", request);

        var result = await _cancelRepsository.GetAllCancelsForAdmin(request);
        if (result.data.Count == 0)
        {
            _logger.LogWarning("No cancel requests found for admin filter {@Filter}", request);
            throw new DataNotFoundException("No cancel requests found");
        }
        _logger.LogInformation("Returning {CancelCount} cancel requests for admin", result.data.Count);

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
        _logger.LogInformation("UserId {UserId} requested cancel list with filter {@Filter}", user, request);

        var result = await _cancelRepsository.GetAllCancelsForUser(request, user);
        if (result.data.Count == 0)
        {
            _logger.LogWarning("No cancel requests found for UserId {UserId}", user);
            throw new DataNotFoundException("No cancel requests found");
        }
        _logger.LogInformation("Returning {CancelCount} cancel requests for UserId {UserId}", result.data.Count, user);

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
        _logger.LogInformation("Vendor UserId {UserId} requested cancel list with filter {@Filter}", user, request);

        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(user);
        _logger.LogInformation("Vendor User validated successfully. VendorId: {VendorId}", vendor.VendorId);

        var result = await _cancelRepsository.GetAllCancelsForVendor(request, vendor.VendorId);
        if (result.data.Count == 0)
        {
            _logger.LogWarning("No cancel requests found for VendorId {VendorId}", vendor.VendorId);
            throw new DataNotFoundException("No cancel requests found");
        }
        _logger.LogInformation("Returning {CancelCount} cancel requests for VendorId {VendorId}", result.data.Count, vendor.VendorId);

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
        _logger.LogInformation("UserId {UserId} requested details for CancelId {CancelId}", user, cancelId);

        var cancel = await _cancelRepsository.GetCancelById(cancelId);
        if (cancel == null)
        {
            _logger.LogWarning("CancelId {CancelId} not found", cancelId);
            throw new DataNotFoundException("Cancel Not Found");
        }
        _logger.LogInformation("Returning CancelId {CancelId} details for UserId {UserId}", cancelId, user);

        return _mapper.Map<CancelSummaryDto>(cancel);
    }
}