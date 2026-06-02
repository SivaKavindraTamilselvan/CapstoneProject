using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminService
{
    public Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId);
}