using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAddressService
{
    public Task<ResponseAddAddressDTO> AddAddress(RequestAddAddressDTO requestAddAddressDTO,int UserId);
}