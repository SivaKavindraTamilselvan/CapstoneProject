using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminService : IAdminService
{
    private readonly ILogger<AdminService> _logger;
    private readonly IAuthentication _authentication;
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly IUserRepsository _userRepsository;
    private readonly EcommerceContext _ecommerceContext;
    private readonly IMapper _mapper;
    public AdminService(EcommerceContext ecommerceContext, IUserRepsository userRepsository, IAuthentication authentication, IMapper mapper, ILogger<AdminService> logger, IAdminUserRepsository adminUserRepsository)
    {
        _authentication = authentication;
        _ecommerceContext = ecommerceContext;
        _adminUserRepsository = adminUserRepsository;
        _userRepsository = userRepsository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId)
    {
        _logger.LogInformation("Admin registration initiated by AdminUserId {AdminUserId} for Email {Email}", adminUserId, requestRegisterAdminDTO.requestRegisterUserDTO.Email);
        var result = await _authentication.RegisterAdmin(requestRegisterAdminDTO, adminUserId);
        _logger.LogInformation("Admin registration completed successfully. New UserId {UserId}", result.UserId);
        return result;
    }
    public async Task<List<ResponseGetAdminUserDTO>> GetAllAdminUser(int? role, bool? status, int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching admin users. Role {Role}, Status {Status}, PageNumber {PageNumber}, PageSize {PageSize}", role, status, pageNumber, pageSize);
        var user = await _adminUserRepsository.GetAllAdminUser(role, status, pageNumber, pageSize);
        if (user.Count == 0)
        {
            _logger.LogWarning("No admin users found. Role {Role}, Status {Status}", role, status);
            throw new DataNotFoundException("No Admin User Found");
        }
        _logger.LogInformation("{Count} admin users found", user.Count);

        return _mapper.Map<List<ResponseGetAdminUserDTO>>(user);
    }
    public async Task<ResponseGetAdminUserDTO> GetAdminUserByUserId(int userId)
    {
        _logger.LogInformation("Fetching admin user by UserId {UserId}", userId);
        var adminUser = await _adminUserRepsository.GetAdminUserByAdminUserId(userId);
        if (adminUser == null)
        {
            _logger.LogWarning("Admin user not found for UserId {UserId}", userId);
            throw new DataNotFoundException("Admin User Not Found");
        }
        return _mapper.Map<ResponseGetAdminUserDTO>(adminUser);
    }
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
    public async Task<ResponseGetAdminUserDTO> ActivateAdminUser(int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
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
            _logger.LogInformation("User account activated for UserId {UserId}", user.UserId);
            _logger.LogInformation("Admin User {AdminUserId} activated successfully", adminUserId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseGetAdminUserDTO>(adminUser);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,"Error occurred while activating Admin User {AdminUserId}",adminUserId);
            await transaction.RollbackAsync();
            _logger.LogInformation("Transaction rolled back for Admin User {AdminUserId}",adminUserId);
            throw;
        }
    }
}