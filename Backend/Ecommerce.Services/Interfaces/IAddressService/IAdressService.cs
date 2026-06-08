using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IAddressService
{
    public Task<List<ResponseGetAddressDTO>> GetAllTheVendorAddress(int vendorUserId);
    public Task<List<ResponseGetAddressDTO>> GetAllActiveAddress(int userId);
    public Task<ResponseMakeDefaultAddressDTO> MakeAddressDefault(RequestMakeDefaultAddressDTO requestMakeDefaultAddressDTO,int userId);
    public Task<ResponseAddAddressDTO> AddAddress(RequestAddAddressDTO requestAddAddressDTO,int UserId);
}