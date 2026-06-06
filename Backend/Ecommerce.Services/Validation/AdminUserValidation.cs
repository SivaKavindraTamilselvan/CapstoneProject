using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class AdminUserValidation : IAdminUserValidation
{
    private readonly IAdminUserRepsository _adminUserRepsository;
    public AdminUserValidation(IAdminUserRepsository adminUserRepsository)
    {
        _adminUserRepsository = adminUserRepsository;
    }
    public async Task<AdminUser> ValidateAdminUserByUserId(int adminUserId)
    {
        var adminUser = await _adminUserRepsository.GetAdminUserByUserId(adminUserId);
        if (adminUser == null)
        {
            throw new DataNotFoundException("Admin User Not Found");
        }
        return adminUser;
    }

}