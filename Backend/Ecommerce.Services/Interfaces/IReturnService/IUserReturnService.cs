using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IUserReturnService
{
    public Task<ResponseAddReturnDTO> AddReturn(RequestAddReturnDTO requestAddReturnDTO);
}