using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminVendorService : IAdminVendorService
{
    public async Task<List<ResponseAdminGetVendorDTO>> GetVendorsForAdmin(RequestAdminVendorFilter request)
    {
        _logger.LogInformation("Fetching all vendors");
        var vendor = await _vendorRepsository.GetVendorsForAdmin(request);
        if (vendor.totalCount == 0)
        {
            _logger.LogWarning("No vendors found");
            throw new DataNotFoundException("No Vendors Found");
        }
        _logger.LogInformation("{Count} vendors found", vendor.totalCount);
        return _mapper.Map<List<ResponseAdminGetVendorDTO>>(vendor.items);
    }
    public async Task<ResponseAdminGetVendorDTO> GetVendorsByVendorIdForAdmin(int vendorId)
    {
        _logger.LogInformation("Fetching all vendors");
        var vendor = await _vendorRepsository.GetVendorsByVendorIdForAdmin(vendorId);
        if (vendor == null)
        {
            _logger.LogWarning("No vendors found");
            throw new DataNotFoundException("No Vendors Found");
        }
        return _mapper.Map<ResponseAdminGetVendorDTO>(vendor);
    }
}