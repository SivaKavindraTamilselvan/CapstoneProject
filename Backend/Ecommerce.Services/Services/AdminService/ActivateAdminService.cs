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
    public async Task<ResponseGetAdminUserDTO> ActivateAdminUser(int adminUserId, int logedusedId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            await _adminUserValidation.ValidateOwnerAdminUserByUserId(logedusedId);
            
            _logger.LogInformation("Activating Admin User {AdminUserId}", adminUserId);
            var adminUser = await _adminUserRepsository.GetAdminUserByAdminUserId(adminUserId);
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
            adminUser = await _adminUserRepsository.Update(adminUser.AdminUserId, adminUser);
            if (adminUser == null)
            {
                _logger.LogError("Failed to activate AdminUser {AdminUserId}", adminUserId);
                throw new DataRegistrationException("Failed to activate Admin User");
            }
            _logger.LogInformation("AdminUser record activated for AdminUserId {AdminUserId}", adminUserId);
            var user = await _userRepsository.Get(adminUser.UserId);
            if (user == null)
            {
                _logger.LogWarning("Linked user not found for AdminUserId {AdminUserId} and UserId {UserId}", adminUser.AdminUserId, adminUser.UserId);
                throw new DataNotFoundException("User Not Found");
            }
            user.IsActive = true;
            user.UpdatedAt = DateTime.Now;


            await _userRepsository.Update(user.UserId, user);

            var logChanges = new LogChanges
            {
                TableName = nameof(AdminUser),
                RecordId = adminUser.AdminUserId,
                Actions = (int)AuditAction.Updated,
                OldValue = $"AdminUserId={adminUser.AdminUserId}, UserId={adminUser.UserId}, IsActive=False",
                NewValue = $"AdminUserId={adminUser.AdminUserId}, UserId={adminUser.UserId}, IsActive=True",
                UserId = logedusedId,
                ChangedAt = DateTime.Now
            };

            await _logChanges.Create(logChanges);
            _logger.LogInformation("Audit log created for AdminUserId {AdminUserId}",adminUser.AdminUserId);
            _logger.LogInformation("User account activated for UserId {UserId}", user.UserId);
            _logger.LogInformation("Admin User {AdminUserId} activated successfully", adminUserId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseGetAdminUserDTO>(adminUser);
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