using System.Security.Authentication;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorService : IVendorService
{
    public async Task<PagedResponse<ResponseGetVendorUserListDTO>> GetAllVendorUser(RequestVendorUserFilter request, int logedusedId)
    {
        var vendor = await _vendorUserValidation.ValidateOwnerVendorUserByUserId(logedusedId);
        _logger.LogInformation("Fetching vendor users.");
        var user = await _vendorUserRepsository.GetVendorsForVendor(request, vendor.VendorId);
        if (user.totalCount == 0)
        {
            _logger.LogWarning("No vendor users found.");
            throw new DataNotFoundException("No Vendor User Found");
        }
        _logger.LogInformation("{Count} admin users found", user.totalCount);
        return new PagedResponse<ResponseGetVendorUserListDTO>
        {
            Items = _mapper.Map<List<ResponseGetVendorUserListDTO>>(user.items),
            TotalCount = user.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<ResponseGetVendorUserDTO> GetVendorUserByUserId(int userId, int logedusedId)
    {
        var owner = await _vendorUserValidation.ValidateOwnerVendorUserByUserId(logedusedId);
        _logger.LogInformation("Fetching admin user by UserId {UserId}", userId);
        var adminUser = await _vendorUserRepsository.GetVendorUserByVendorUserId(userId);
        if (adminUser == null)
        {
            _logger.LogWarning("Vendor user not found for UserId {UserId}", userId);
            throw new DataNotFoundException("Vendor User Not Found");
        }
        if (owner.VendorId != adminUser.VendorId)
        {
            throw new UnauthorizationException("Cannot Access other vendor users");
        }
        return _mapper.Map<ResponseGetVendorUserDTO>(adminUser);
    }
}