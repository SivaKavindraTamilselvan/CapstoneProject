using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;

public partial class AdminService : IAdminService
{
    private readonly IAuthentication _authentication;
    private readonly IMapper _mapper;
    public AdminService(IAuthentication authentication,IMapper mapper)
    {
        _authentication = authentication;
        _mapper = mapper;
    }
    public async Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO,int adminUserId)
    {
        var result = await _authentication.RegisterAdmin(requestRegisterAdminDTO,adminUserId);
        return result;
    }
}