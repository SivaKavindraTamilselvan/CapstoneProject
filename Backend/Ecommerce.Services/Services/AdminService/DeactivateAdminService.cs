using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminService : IAdminService
{
    public async Task<ResponseGetAdminUserDTO> DeactivateAdminUser(int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
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
            adminUser = await _adminUserRepsository.Update(adminUser.AdminUserId, adminUser);
            if (adminUser == null)
            {
                _logger.LogError("Failed to deactivate AdminUser {AdminUserId}", adminUserId);
                throw new DataRegistrationException("Failed to deactivate Admin User");
            }
            _logger.LogInformation("AdminUser record deactivated for AdminUserId {AdminUserId}", adminUserId);
            var user = await _userRepsository.Get(adminUser.UserId);
            if (user == null)
            {
                _logger.LogWarning("Linked user not found for AdminUserId {AdminUserId} and UserId {UserId}", adminUser.AdminUserId, adminUser.UserId);
                throw new DataNotFoundException("User Not Found");
            }
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;
            await _userRepsository.Update(user.UserId, user);
            _logger.LogInformation("User account deactivated for UserId {UserId}", user.UserId);
            _logger.LogInformation("Admin User {AdminUserId} deactivated successfully", adminUserId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseGetAdminUserDTO>(adminUser);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,"Error occurred while deactivating Admin User {AdminUserId}",adminUserId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for Admin User {AdminUserId}",adminUserId);
            throw;
        }
    }
}