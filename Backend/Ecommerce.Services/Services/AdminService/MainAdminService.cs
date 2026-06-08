using AutoMapper;
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
    private readonly IMapper _mapper;
    public AdminService(IAuthentication authentication, IMapper mapper, ILogger<AdminService> logger, IAdminUserRepsository adminUserRepsository)
    {
        _authentication = authentication;
        _adminUserRepsository = adminUserRepsository;
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
}