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
    public AdminService(IAuthentication authentication,IMapper mapper,ILogger<AdminService> logger,IAdminUserRepsository adminUserRepsository)
    {
        _authentication = authentication;
        _adminUserRepsository = adminUserRepsository;
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
    public async Task<List<ResponseGetAdminUserDTO>> GetAllAdminUser(int? role,bool? status,int pageNumber,int pageSize)
    {
        var user = await _adminUserRepsository.GetAllAdminUser(role,status,pageNumber,pageSize);
        if(user.Count == 0)
        {
            throw new DataNotFoundException("No Admin User Found");
        }
        return _mapper.Map<List<ResponseGetAdminUserDTO>>(user);
    }
}