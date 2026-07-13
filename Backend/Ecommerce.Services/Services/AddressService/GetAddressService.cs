using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AddressService : IAddressService
{
    public async Task<List<ResponseGetAddressDTO>> GetAllActiveUserAddress(int userId)
    {
        _logger.LogInformation("Fetching active addresses for UserId {UserId}", userId);
        await _userValidation.ValidateUser(userId);
        var address = await _addressRepsository.GetAllActiveUserAddress(userId);
        if (address.Count == 0)
        {
            _logger.LogWarning("No active addresses found for UserId {UserId}", userId);
            throw new DataNotFoundException("No Address Is Found");
        }
        _logger.LogInformation("{Count} active addresses found for UserId {UserId}", address.Count, userId);
        return _mapper.Map<List<ResponseGetAddressDTO>>(address);
    }
    public async Task<ResponseGetAddressDTO> GetAddress(int userId, int addressId)
    {
        _logger.LogInformation("Fetching active addresses for UserId {UserId}", userId);
        await _userValidation.ValidateUser(userId);
        await _userValidation.ValidateAddress(addressId, userId);
        var address = await _addressRepsository.Get(addressId);
        if (address == null)
        {
            _logger.LogWarning("No active addresses found for UserId {UserId}", userId);
            throw new DataNotFoundException("No Address Is Found");
        }
        _logger.LogInformation("active addresses found for UserId {UserId}", userId);
        return _mapper.Map<ResponseGetAddressDTO>(address);
    }
    public async Task<PagedResponse<ResponseGetAddressDTO>> GetAllTheVendorAddress(int UserId, AddressRequestFilter request)
    {
        _logger.LogInformation("Fetching addresses for Vendor Owner UserId {VendorUserId}", UserId);
        await _userValidation.ValidateUser(UserId);
        var address = await _addressRepsository.GetAddressByVendorOwnerId(request, UserId);
        if (address.Items.Count == 0)
        {
            _logger.LogWarning("No addresses found for Vendor UserId {VendorUserId}", UserId);
            throw new DataNotFoundException("No Address Is Found");
        }
        _logger.LogInformation("{Count} addresses found for Vendor Owner UserId {VendorUserId}", address.Items.Count, UserId);
        return new PagedResponse<ResponseGetAddressDTO>
        {
            Items = _mapper.Map<List<ResponseGetAddressDTO>>(address.Items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = address.totalCount
        };
    }

}