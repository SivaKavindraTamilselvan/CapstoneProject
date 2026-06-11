using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class AdminUserValidation : IAdminUserValidation
{
     private readonly ILogger<AdminUserValidation> _logger;
    private readonly IAdminUserRepsository _adminUserRepsository;
    public AdminUserValidation(ILogger<AdminUserValidation> logger,IAdminUserRepsository adminUserRepsository)
    {
        _logger = logger;
        _adminUserRepsository = adminUserRepsository;
    }
    public async Task<AdminUser> ValidateAdminUserByUserId(int adminUserId)
    {
        var adminUser = await _adminUserRepsository.GetAdminUserByUserId(adminUserId);
        if (adminUser == null)
        {
            _logger.LogError("Admin record not found for UserId {UserId}", adminUserId);
            throw new DataNotFoundException("Admin Not Found");
        }
        if(!adminUser.IsActive)
        {
            _logger.LogError("Admin record  is inactive for UserId {UserId}", adminUserId);
            throw new DataApprovalStatusException("Admin Is not approved to do this work");
        }
        return adminUser;
    }

}