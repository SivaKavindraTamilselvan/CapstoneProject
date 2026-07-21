using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorService : IVendorService
{
    public async Task<ResponseRegisterVendorUserDTO> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO, int vendorUserId)
    {
        await _vendorUserValidation.ValidateOwnerVendorUserByUserId(vendorUserId);
        _logger.LogInformation("Vendor UserId {VendorUserId} is registering a new vendor user with Email {Email} and VendorRoleId {VendorRoleId}",
            vendorUserId, requestRegisterVendorUserDTO.requestRegisterUserDTO.Email, requestRegisterVendorUserDTO.VendorRoleId);

        var result = await _authentication.RegisterVendorUser(requestRegisterVendorUserDTO, vendorUserId);

        _logger.LogInformation("Vendor user registration completed successfully by Vendor UserId {VendorUserId}. New UserId {UserId}", vendorUserId, result.UserId);

        return result;
    }
    public async Task<ResponseGetVendorUserDTO> DeactivateAdminUser(int adminUserId, int logedusedId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var vendorUser = await _vendorUserValidation.ValidateOwnerVendorUserByUserId(logedusedId);
            var vendor = await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
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
            var logChanges = new LogChanges
            {
                TableName = nameof(VendorUser),
                RecordId = adminUser.VendorUserId,
                Actions = (int)AuditAction.Updated,
                OldValue = "IsActive=True",
                NewValue = "IsActive=False",
                UserId = logedusedId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);

            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for VendorUserId {VendorUserId}", adminUser.VendorUserId);
                throw new DataRegistrationException("Failed to create audit log");
            }

            _logger.LogInformation(
                "Audit log created for table {TableName}, RecordId {RecordId}, Action {Action}",
                logChanges.TableName,
                logChanges.RecordId,
                nameof(AuditAction.Updated));

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
            var vendorUser = await _vendorUserValidation.ValidateOwnerVendorUserByUserId(logedusedId);
            var vendor = await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
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

            var logChanges = new LogChanges
            {
                TableName = nameof(VendorUser),
                RecordId = adminUser.VendorUserId,
                Actions = (int)AuditAction.Updated,
                OldValue = "IsActive=False",
                NewValue = "IsActive=True",
                UserId = logedusedId,
                ChangedAt = DateTime.Now
            };

            var createdLog = await _logChanges.Create(logChanges);

            if (createdLog == null)
            {
                _logger.LogError("Failed to create audit log for VendorUserId {VendorUserId}", adminUser.VendorUserId);
                throw new DataRegistrationException("Failed to create audit log");
            }

            _logger.LogInformation(
                "Audit log created for table {TableName}, RecordId {RecordId}, Action {Action}",
                logChanges.TableName,
                logChanges.RecordId,
                nameof(AuditAction.Updated));

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