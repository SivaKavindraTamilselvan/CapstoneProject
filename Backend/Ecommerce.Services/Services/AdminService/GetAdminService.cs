using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminService : IAdminService
{
    public async Task<PagedResponse<ResponseGetAdminUserDTO>> GetAllAdminUser(RequestAdiminUserFilter request, int logedusedId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(logedusedId);
        _logger.LogInformation("Fetching admin users.");
        var user = await _adminUserRepsository.GetAllAdminUser(request);
        if (user.totalCount == 0)
        {
            _logger.LogWarning("No admin users found.");
            throw new DataNotFoundException("No Admin User Found");
        }
        _logger.LogInformation("{Count} admin users found", user.totalCount);
        return new PagedResponse<ResponseGetAdminUserDTO>
        {
            Items = _mapper.Map<List<ResponseGetAdminUserDTO>>(user.items),
            TotalCount = user.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<ResponseGetAdminUserDTO> GetAdminUserByUserId(int userId, int logedusedId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(logedusedId);
        _logger.LogInformation("Fetching admin user by UserId {UserId}", userId);
        var adminUser = await _adminUserRepsository.GetAdminUserByAdminUserId(userId);
        if (adminUser == null)
        {
            _logger.LogWarning("Admin user not found for UserId {UserId}", userId);
            throw new DataNotFoundException("Admin User Not Found");
        }
        return _mapper.Map<ResponseGetAdminUserDTO>(adminUser);
    }
}