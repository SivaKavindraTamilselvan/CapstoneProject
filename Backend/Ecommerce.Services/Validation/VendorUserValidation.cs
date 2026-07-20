using System.Security.Authentication;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class VendorUserValidation : IVendorUserValidation
{
    private readonly ILogger<VendorUserValidation> _logger;
    private readonly IVendorUserRepsository _vendorUserRepsository;
    public VendorUserValidation(ILogger<VendorUserValidation> logger,IVendorUserRepsository vendorUserRepsository)
    {
        _logger = logger;
        _vendorUserRepsository = vendorUserRepsository;
    }
    public async Task<VendorUser> ValidateVendorUserByUserId(int vendorUserId)
    {
        var vendorUser = await _vendorUserRepsository.GetVendorUserByUserId(vendorUserId);
        if (vendorUser == null)
        {
            throw new DataNotFoundException("Vendor User Not Found");
        }
        if (!vendorUser.IsActive)
        {
            throw new UnauthorizationException("Vendor User Is Removed");
        }
        if (vendorUser.Vendor!.ApprovalStatusId != (int)ApprovalStatusEnum.Accepted)
        {
            throw new UnauthorizationException("Vendor Is Not Approved");
        }
        return vendorUser;
    }
    public async Task<VendorUser> ValidateProductVendorUserByUserId(int vendorUserId)
    {
        var vendorUser = await ValidateVendorUserByUserId(vendorUserId);
        if (vendorUser.VendorRoleId != (int)VendorRoleEnum.Owner && vendorUser.VendorRoleId != (int)VendorRoleEnum.ProductManager)
        {
            _logger.LogWarning("AdminUserId {AdminUserId} is not authorized for product administration. RoleId: {RoleId}", vendorUser, vendorUser.VendorRoleId);
            throw new UnauthorizationException("Admin is not authorized to perform this operation.");
        }
        return vendorUser;
    }

    public async Task<VendorUser> ValidateOwnerVendorUserByUserId(int vendorUserId)
    {
        var vendorUser = await ValidateVendorUserByUserId(vendorUserId);
        if (vendorUser.VendorRoleId != (int)VendorRoleEnum.Owner)
        {
            _logger.LogWarning("AdminUserId {AdminUserId} is not authorized for owner administration. RoleId: {RoleId}", vendorUser, vendorUser.VendorRoleId);
            throw new UnauthorizationException("Admin is not authorized to perform this operation.");
        }
        return vendorUser;
    }

    public async Task<VendorUser> ValidateInventoryVendorUserByUserId(int vendorUserId)
    {
        var vendorUser = await ValidateVendorUserByUserId(vendorUserId);
        if (vendorUser.VendorRoleId != (int)VendorRoleEnum.Owner && vendorUser.VendorRoleId !=(int)VendorRoleEnum.InventoryManager)
        {
            _logger.LogWarning("AdminUserId {AdminUserId} is not authorized for inventory and administration. RoleId: {RoleId}", vendorUser, vendorUser.VendorRoleId);
            throw new UnauthorizationException("Admin is not authorized to perform this operation.");
        }
        return vendorUser;
    }
}