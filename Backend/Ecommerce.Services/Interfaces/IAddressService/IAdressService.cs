using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IAddressService
{
    public Task<ResponseGetAddressDTO> DeleteInventoryAddress(int addressId, int userId);
    public Task<ResponseGetAddressDTO> DeleteUserAddress(int addressId, int userId);
    public Task<List<ResponseGetAddressDTO>> GetAllTheVendorAddress(int vendorUserId, bool? status, int pageNumber, int pageSize);
    public Task<List<ResponseGetAddressDTO>> GetAllActiveAddress(int userId, int pageNumber, int pageSize);
    public Task<ResponseMakeDefaultAddressDTO> MakeAddressDefault(RequestMakeDefaultAddressDTO requestMakeDefaultAddressDTO, int userId);
    public Task<ResponseAddAddressDTO> AddAddress(RequestAddAddressDTO requestAddAddressDTO, int UserId);
}