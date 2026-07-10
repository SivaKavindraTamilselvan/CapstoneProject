using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IAdminUserValidation
{
    public Task<AdminUser> ValidateAdminUserByUserId(int adminUserId);
}