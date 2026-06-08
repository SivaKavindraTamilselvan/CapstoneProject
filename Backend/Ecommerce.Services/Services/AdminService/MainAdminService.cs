using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminService : IAdminService
{
    private readonly ILogger<AdminService> _logger;
    private readonly IAuthentication _authentication;
    private readonly IMapper _mapper;
    public AdminService(IAuthentication authentication,IMapper mapper,ILogger<AdminService> logger)
    {
        _authentication = authentication;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO,int adminUserId)
    {
        _logger.LogInformation("Admin registration initiated by AdminUserId {AdminUserId} for Email {Email}",adminUserId,requestRegisterAdminDTO.requestRegisterUserDTO.Email);
        var result = await _authentication.RegisterAdmin(requestRegisterAdminDTO,adminUserId);
         _logger.LogInformation("Admin registration completed successfully. New UserId {UserId}",result.UserId);
        return result;
    }
}