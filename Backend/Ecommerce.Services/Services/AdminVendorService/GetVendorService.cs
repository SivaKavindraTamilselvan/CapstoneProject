using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminVendorService : IAdminVendorService
{
    public async Task<PagedResponse<ResponseAdminGetVendorDTO>> GetVendorsForAdmin(RequestAdminVendorFilter request, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Fetching all vendors");
        var vendor = await _vendorRepsository.GetVendorsForAdmin(request);
        if (vendor.totalCount == 0)
        {
            _logger.LogWarning("No vendors found");
            throw new DataNotFoundException("No Vendors Found");
        }
        _logger.LogInformation("{Count} vendors found", vendor.totalCount);
        return new PagedResponse<ResponseAdminGetVendorDTO>
        {
            Items = _mapper.Map<List<ResponseAdminGetVendorDTO>>(vendor.items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = vendor.totalCount
        };
    }
    public async Task<ResponseAdminGetVendorDTO> GetVendorsByVendorIdForAdmin(int vendorId, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Fetching all vendors");
        var vendor = await _vendorRepsository.GetVendorsByVendorIdForAdmin(vendorId);
        if (vendor == null)
        {
            _logger.LogWarning("No vendors found");
            throw new DataNotFoundException("No Vendors Found");
        }
        return _mapper.Map<ResponseAdminGetVendorDTO>(vendor);
    }

    public async Task<PagedResponse<ResponseGetVendorUserDTO>> GetVendorUserByVendorIdForAdmin(RequestAdminVendorUserFilter request, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Fetching all vendors");
        var vendor = await _vendorUserRepsository.GetVendorsForAdmin(request);
        return new PagedResponse<ResponseGetVendorUserDTO>
        {
            Items = _mapper.Map<List<ResponseGetVendorUserDTO>>(vendor.items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = vendor.totalCount
        };
    }
}