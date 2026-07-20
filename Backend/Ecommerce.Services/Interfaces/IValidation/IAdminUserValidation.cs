using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IAdminUserValidation
{
    public Task<AdminUser> ValidateOwnerAdminUserByUserId(int adminUserId);
    public Task<AdminUser> ValidateVendorAdminUserByUserId(int adminUserId);
    public Task<AdminUser> ValidateShipmentAndCouponAdminUserByUserId(int adminUserId);
    public Task<AdminUser> ValidateProductAdminUserByUserId(int adminUserId);
    public Task<AdminUser> ValidateAdminUserByUserId(int adminUserId);
}