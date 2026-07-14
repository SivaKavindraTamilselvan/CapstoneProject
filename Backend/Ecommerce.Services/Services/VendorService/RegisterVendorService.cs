using System.Security.Authentication;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorService : IVendorService
{
    public async Task<ResponseRegisterVendorUserDTO> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} is registering a new vendor user with Email {Email} and VendorRoleId {VendorRoleId}", vendorUserId, requestRegisterVendorUserDTO.requestRegisterUserDTO.Email, requestRegisterVendorUserDTO.VendorRoleId);
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var user = await _authentication.RegisterUser(requestRegisterVendorUserDTO.requestRegisterUserDTO, (int)RoleEnum.Vendor);

            _logger.LogInformation("User created successfully. UserId {UserId}", user.UserId);
            var OwnerVendor = (await _vendorUserRepsository.GetAll()).FirstOrDefault(u => u.UserId == vendorUserId);
            if (OwnerVendor == null)
            {
                _logger.LogWarning("Vendor not found for UserId {VendorUserId}", vendorUserId);
                throw new DataNotFoundException("Vendor Not Found");
            }
            VendorUser vendorUser = new VendorUser();
            vendorUser.VendorId = OwnerVendor.VendorId;
            vendorUser.UserId = user.UserId;
            vendorUser.VendorRoleId = requestRegisterVendorUserDTO.VendorRoleId;
            vendorUser.AddedByVendorUserId = OwnerVendor.VendorUserId;
            await _vendorUserRepsository.Create(vendorUser);
            _logger.LogInformation("VendorUser created successfully. VendorUserId {VendorUserId}, VendorId {VendorId}, UserId {UserId}",
            vendorUser.VendorUserId,
            vendorUser.VendorId,
            vendorUser.UserId);
            await transaction.CommitAsync();
            _logger.LogInformation("Vendor user registration completed successfully by Vendor UserId {VendorUserId}", vendorUserId);
            return _mapper.Map<ResponseRegisterVendorUserDTO>(vendorUser); // authentication mapper
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while registering vendor user by Vendor UserId {VendorUserId}", vendorUserId);
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<PagedResponse<ResponseGetVendorUserDTO>> GetAllVendorUser(RequestVendorUserFilter request, int logedusedId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(logedusedId);
        _logger.LogInformation("Fetching admin users.");
        var user = await _vendorUserRepsository.GetVendorsForVendor(request, vendor.VendorId);
        if (user.totalCount == 0)
        {
            _logger.LogWarning("No admin users found.");
            throw new DataNotFoundException("No Admin User Found");
        }
        _logger.LogInformation("{Count} admin users found", user.totalCount);
        return new PagedResponse<ResponseGetVendorUserDTO>
        {
            Items = _mapper.Map<List<ResponseGetVendorUserDTO>>(user.items),
            TotalCount = user.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<ResponseGetVendorUserDTO> GetVendorUserByUserId(int userId, int logedusedId)
    {
        var owner = await _vendorUserValidation.ValidateVendorUserByUserId(logedusedId);
        if (owner.VendorRoleId != 1)
        {
            throw new InvalidCredentialException("Only Vendor Owner Can access");
        }
        _logger.LogInformation("Fetching admin user by UserId {UserId}", userId);
        var adminUser = await _vendorUserRepsository.GetVendorUserByVendorUserId(userId);
        if (adminUser == null)
        {
            _logger.LogWarning("Admin user not found for UserId {UserId}", userId);
            throw new DataNotFoundException("Admin User Not Found");
        }
        if (owner.VendorId != adminUser.VendorId)
        {
            throw new InvalidCredentialException("Cannot Access other vendor users");
        }
        return _mapper.Map<ResponseGetVendorUserDTO>(adminUser);
    }
    public async Task<ResponseGetVendorUserDTO> DeactivateAdminUser(int adminUserId, int logedusedId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _vendorUserValidation.ValidateVendorUserByUserId(logedusedId);
            _logger.LogInformation("Deactivating Admin User {AdminUserId}", adminUserId);
            var adminUser = await _vendorUserRepsository.GetVendorUserByVendorUserId(adminUserId);
            if (adminUser == null)
            {
                _logger.LogWarning("Vendor User not found for AdminUserId {AdminUserId}", adminUserId);
                throw new DataNotFoundException("Admin User Not Found");
            }
            if (!adminUser.IsActive)
            {
                _logger.LogWarning("Admin User {AdminUserId} is already deactivated", adminUserId);
                throw new DataApprovalStatusException("Admin User is already deactivated");
            }
            adminUser.IsActive = false;
            adminUser = await _vendorUserRepsository.Update(adminUser.VendorUserId, adminUser);
            if (adminUser == null)
            {
                _logger.LogError("Failed to deactivate AdminUser {AdminUserId}", adminUserId);
                throw new DataRegistrationException("Failed to deactivate Admin User");
            }
            _logger.LogInformation("AdminUser record deactivated for AdminUserId {AdminUserId}", adminUserId);
            var user = await _userRepsository.Get(adminUser.UserId);
            if (user == null)
            {
                _logger.LogWarning("Linked user not found for AdminUserId {AdminUserId} and UserId {UserId}", adminUser.VendorUserId, adminUser.UserId);
                throw new DataNotFoundException("User Not Found");
            }
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;
            await _userRepsository.Update(user.UserId, user);
            _logger.LogInformation("User account deactivated for UserId {UserId}", user.UserId);
            _logger.LogInformation("Admin User {AdminUserId} deactivated successfully", adminUserId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseGetVendorUserDTO>(adminUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deactivating Admin User {AdminUserId}", adminUserId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for Admin User {AdminUserId}", adminUserId);
            throw;
        }
    }
    public async Task<ResponseGetVendorUserDTO> ActivateAdminUser(int adminUserId, int logedusedId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _vendorUserValidation.ValidateVendorUserByUserId(logedusedId);
            _logger.LogInformation("Activating Admin User {AdminUserId}", adminUserId);
            var adminUser = await _vendorUserRepsository.GetVendorUserByVendorUserId(adminUserId);
            if (adminUser == null)
            {
                _logger.LogWarning("Admin User not found for AdminUserId {AdminUserId}", adminUserId);
                throw new DataNotFoundException("Admin User Not Found");
            }
            if (adminUser.IsActive)
            {
                _logger.LogWarning("Admin User {AdminUserId} is already Activated", adminUserId);
                throw new DataApprovalStatusException("Admin User is already activated");
            }
            adminUser.IsActive = true;
            adminUser = await _vendorUserRepsository.Update(adminUser.VendorUserId, adminUser);
            if (adminUser == null)
            {
                _logger.LogError("Failed to activate AdminUser {AdminUserId}", adminUserId);
                throw new DataRegistrationException("Failed to activate Admin User");
            }
            _logger.LogInformation("AdminUser record activated for AdminUserId {AdminUserId}", adminUserId);
            var user = await _userRepsository.Get(adminUser.UserId);
            if (user == null)
            {
                _logger.LogWarning("Linked user not found for AdminUserId {AdminUserId} and UserId {UserId}", adminUser.VendorUserId, adminUser.UserId);
                throw new DataNotFoundException("User Not Found");
            }
            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;
            await _userRepsository.Update(user.UserId, user);
            _logger.LogInformation("User account activated for UserId {UserId}", user.UserId);
            _logger.LogInformation("Admin User {AdminUserId} activated successfully", adminUserId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseGetVendorUserDTO>(adminUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while activating Admin User {AdminUserId}", adminUserId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for Admin User {AdminUserId}", adminUserId);
            throw;
        }
    }
}