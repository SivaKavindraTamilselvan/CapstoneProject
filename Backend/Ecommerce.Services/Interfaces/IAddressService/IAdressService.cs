using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAddressService
{
    public Task<ResponseMakeDefaultAddressDTO> MakeAddressDefault(RequestMakeDefaultAddressDTO requestMakeDefaultAddressDTO);
    public Task<ResponseAddAddressDTO> AddAddress(RequestAddAddressDTO requestAddAddressDTO,int UserId);
}