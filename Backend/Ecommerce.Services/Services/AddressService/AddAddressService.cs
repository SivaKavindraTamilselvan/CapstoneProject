using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AddressService : IAddressService
{
    public async Task<ResponseAddAddressDTO> AddAddress(RequestAddAddressDTO requestAddAddressDTO, int UserId)
    {
        _logger.LogInformation("Adding address for UserId {UserId}", UserId);
        await _userValidation.ValidateUser(UserId);
        var address = _mapper.Map<Address>(requestAddAddressDTO);
        address.UserId = UserId;
        var createdAddress = await _addressRepsository.Create(address);
        if (createdAddress == null)
        {
            _logger.LogError("Address creation failed for UserId {UserId}. City: {City}, Pincode: {Pincode}", UserId, address.City, address.PinCode);
            throw new DataRegistrationException("Failed to create address");
        }

        if (createdAddress.IsDefault)
        {
            _logger.LogInformation("Setting AddressId {AddressId} as default for UserId {UserId}", createdAddress.AddressId, UserId);
            await MakeAddressDefault(createdAddress.AddressId, UserId);
        }
        _logger.LogInformation("Address created successfully with AddressId {AddressId} for UserId {UserId}", createdAddress.AddressId, UserId);
        return _mapper.Map<ResponseAddAddressDTO>(createdAddress);
    }
    public async Task<ResponseMakeDefaultAddressDTO> MakeAddressDefault(int addressId, int userId)
    {
        _logger.LogInformation("Making AddressId {AddressId} default", addressId);
        var selectedAddress = await _userValidation.ValidateAddress(addressId, userId);
        var userAddress = await _addressRepsository.GetAllAddressByUserId(selectedAddress.UserId);
        foreach (var address in userAddress)
        {
            address.IsDefault = false;
            address.UpdatedAt = DateTime.Now;
            var updatedAddress = await _addressRepsository.Update(address.AddressId, address);
            if (updatedAddress == null)
            {
                _logger.LogError("Failed to update AddressId {AddressId}", address.AddressId);
                throw new DataRegistrationException("Failed to update address");
            }
        }
        selectedAddress.IsDefault = true;
        selectedAddress.UpdatedAt = DateTime.Now;
        await _addressRepsository.Update(selectedAddress.AddressId, selectedAddress);
        _logger.LogInformation("AddressId {AddressId} set as default successfully", selectedAddress.AddressId);
        return _mapper.Map<ResponseMakeDefaultAddressDTO>(selectedAddress);
    }
}