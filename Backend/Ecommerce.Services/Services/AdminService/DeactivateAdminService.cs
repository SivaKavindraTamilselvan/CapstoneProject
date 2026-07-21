using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminService : IAdminService
{
    public async Task<ResponseGetAdminUserDTO> DeactivateAdminUser(int adminUserId, int logedusedId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _adminUserValidation.ValidateOwnerAdminUserByUserId(logedusedId);
            _logger.LogInformation("Deactivating Admin User {AdminUserId}", adminUserId);
            var adminUser = await _adminUserRepsository.GetAdminUserByAdminUserId(adminUserId);
            if (adminUser == null)
            {
                _logger.LogWarning("Admin User not found for AdminUserId {AdminUserId}", adminUserId);
                throw new DataNotFoundException("Admin User Not Found");
            }
            if (!adminUser.IsActive)
            {
                _logger.LogWarning("Admin User {AdminUserId} is already deactivated", adminUserId);
                throw new DataApprovalStatusException("Admin User is already deactivated");
            }
            adminUser.IsActive = false;

            var updatedAdminUser = await _adminUserRepsository.Update(adminUser.AdminUserId, adminUser);
            if (updatedAdminUser == null)
            {
                _logger.LogError("Failed to deactivate AdminUser {AdminUserId}", adminUserId);
                throw new DataRegistrationException("Failed to deactivate Admin User");
            }
            adminUser = updatedAdminUser;
            _logger.LogInformation("AdminUser record deactivated for AdminUserId {AdminUserId}", adminUserId);

            var user = await _userRepsository.Get(adminUser.UserId);
            if (user == null)
            {
                _logger.LogWarning("Linked user not found for AdminUserId {AdminUserId} and UserId {UserId}", adminUser.AdminUserId, adminUser.UserId);
                throw new DataNotFoundException("User Not Found");
            }
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;

            var updatedUser = await _userRepsository.Update(user.UserId, user);
            if (updatedUser == null)
            {
                _logger.LogError("Failed to deactivate UserId {UserId} for AdminUserId {AdminUserId}", user.UserId, adminUser.AdminUserId);
                throw new DataRegistrationException("Failed to deactivate User");
            }

            var adminUserLog = new LogChanges
            {
                TableName = nameof(AdminUser),
                RecordId = adminUser.AdminUserId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"AdminUserId={adminUser.AdminUserId}, UserId={adminUser.UserId}, IsActive=True",
                NewValue = $"AdminUserId={adminUser.AdminUserId}, UserId={adminUser.UserId}, IsActive=False",
                UserId = logedusedId,
                ChangedAt = DateTime.Now
            };

            var createdAdminUserLog = await _logChanges.Create(adminUserLog);
            if (createdAdminUserLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", adminUserLog.TableName, adminUserLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for AdminUserId {AdminUserId}", adminUser.AdminUserId);

            var userLog = new LogChanges
            {
                TableName = nameof(User),
                RecordId = updatedUser.UserId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"UserId={user.UserId}, IsActive=True",
                NewValue = $"UserId={updatedUser.UserId}, IsActive=False",
                UserId = logedusedId,
                ChangedAt = DateTime.Now
            };

            var createdUserLog = await _logChanges.Create(userLog);
            if (createdUserLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", userLog.TableName, userLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }
            _logger.LogInformation("Audit log created for UserId {UserId}", updatedUser.UserId);

            _logger.LogInformation("User account deactivated for UserId {UserId}", updatedUser.UserId);
            _logger.LogInformation("Admin User {AdminUserId} deactivated successfully", adminUserId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseGetAdminUserDTO>(adminUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deactivating Admin User {AdminUserId}", adminUserId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for Admin User {AdminUserId}", adminUserId);
            throw;
        }
    }
}