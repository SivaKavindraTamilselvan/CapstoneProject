using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class AdminUserValidation : IAdminUserValidation
{
    private readonly ILogger<AdminUserValidation> _logger;
    private readonly IAdminUserRepsository _adminUserRepsository;
    public AdminUserValidation(ILogger<AdminUserValidation> logger, IAdminUserRepsository adminUserRepsository)
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
        if (!adminUser.IsActive)
        {
            _logger.LogError("Admin record  is inactive for UserId {UserId}", adminUserId);
            throw new DataApprovalStatusException("Admin Is not approved to do this work");
        }
        return adminUser;
    }

     public async Task<AdminUser> ValidateOwnerAdminUserByUserId(int adminUserId)
    {
        var adminUser = await ValidateAdminUserByUserId(adminUserId);
        if (adminUser.AdminRoleId != (int)AdminRoleEnum.Overall_Admin)
        {
            _logger.LogWarning("AdminUserId {AdminUserId} is not authorized for owner administration. RoleId: {RoleId}", adminUserId, adminUser.AdminRoleId);
            throw new UnauthorizationException("Admin is not authorized to perform this operation.");
        }
        return adminUser;
    }

    public async Task<AdminUser> ValidateProductAdminUserByUserId(int adminUserId)
    {
        var adminUser = await ValidateAdminUserByUserId(adminUserId);
        if (adminUser.AdminRoleId != (int)AdminRoleEnum.Overall_Admin && adminUser.AdminRoleId != (int)AdminRoleEnum.Product_Admin)
        {
            _logger.LogWarning("AdminUserId {AdminUserId} is not authorized for product administration. RoleId: {RoleId}", adminUserId, adminUser.AdminRoleId);
            throw new UnauthorizationException("Admin is not authorized to perform this operation.");
        }
        return adminUser;
    }
    public async Task<AdminUser> ValidateShipmentAndCouponAdminUserByUserId(int adminUserId)
    {
        var adminUser = await ValidateAdminUserByUserId(adminUserId);
        if (adminUser.AdminRoleId != (int)AdminRoleEnum.Overall_Admin && adminUser.AdminRoleId != (int)AdminRoleEnum.Coupons_Logistic_Admin)
        {
            _logger.LogWarning("AdminUserId {AdminUserId} is not authorized for coupon and shipment administration. RoleId: {RoleId}", adminUserId, adminUser.AdminRoleId);
            throw new UnauthorizationException("Admin is not authorized to perform this operation.");
        }
        return adminUser;
    }

    public async Task<AdminUser> ValidateVendorAdminUserByUserId(int adminUserId)
    {
        var adminUser = await ValidateAdminUserByUserId(adminUserId);
        if (adminUser.AdminRoleId != (int)AdminRoleEnum.Overall_Admin && adminUser.AdminRoleId != (int)AdminRoleEnum.Vendor_Admin)
        {
            _logger.LogWarning("AdminUserId {AdminUserId} is not authorized for vendor administration. RoleId: {RoleId}", adminUserId, adminUser.AdminRoleId);
            throw new UnauthorizationException("Admin is not authorized to perform this operation.");
        }
        return adminUser;
    }

}