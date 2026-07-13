using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IAddressService
{
    public Task<ResponseGetAddressDTO> GetAddress(int userId, int addressId);
    public Task<ResponseGetAddressDTO> DeleteInventoryAddress(int addressId, int userId);
    public Task<ResponseGetAddressDTO> DeleteUserAddress(int addressId, int userId);
    public Task<PagedResponse<ResponseGetAddressDTO>> GetAllTheVendorAddress(int UserId, AddressRequestFilter request);
    public Task<List<ResponseGetAddressDTO>> GetAllActiveUserAddress(int userId);
    public Task<ResponseMakeDefaultAddressDTO> MakeAddressDefault(int addressId, int userId);
    public Task<ResponseAddAddressDTO> AddAddress(RequestAddAddressDTO requestAddAddressDTO, int UserId);
}