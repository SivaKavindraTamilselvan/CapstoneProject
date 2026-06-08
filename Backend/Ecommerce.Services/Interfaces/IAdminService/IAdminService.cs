using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminService
{
    public Task<List<ResponseGetAdminUserDTO>> GetAllAdminUser(int? role,bool? status,int pageNumber,int pageSize);
    public Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId);
}