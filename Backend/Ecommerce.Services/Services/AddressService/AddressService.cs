using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public class AddressService : IAddressService
{
    private readonly ILogger<AddressService> _logger;
    private readonly IAddressRepsository _addressRepsository;
    private readonly IUserValidation _userValidation;
    private readonly IMapper _mapper;
    public AddressService(IAddressRepsository addressRepsository, IUserValidation userValidation, IMapper mapper, ILogger<AddressService> logger)
    {
        _addressRepsository = addressRepsository;
        _userValidation = userValidation;
        _mapper = mapper;
        _logger = logger;
    }
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
            RequestMakeDefaultAddressDTO requestMakeDefaultAddressDTO = new RequestMakeDefaultAddressDTO();
            requestMakeDefaultAddressDTO.AddressId = createdAddress.AddressId;
            await MakeAddressDefault(requestMakeDefaultAddressDTO, UserId);
        }
        _logger.LogInformation("Address created successfully with AddressId {AddressId} for UserId {UserId}", createdAddress.AddressId, UserId);
        return _mapper.Map<ResponseAddAddressDTO>(createdAddress);
    }
    public async Task<ResponseMakeDefaultAddressDTO> MakeAddressDefault(RequestMakeDefaultAddressDTO requestMakeDefaultAddressDTO, int userId)
    {
        _logger.LogInformation("Making AddressId {AddressId} default", requestMakeDefaultAddressDTO.AddressId);
        var selectedAddress = await _userValidation.ValidateAddress(requestMakeDefaultAddressDTO.AddressId, userId);
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
    public async Task<List<ResponseGetAddressDTO>> GetAllActiveAddress(int userId,int pageNumber,int pageSize)
    {
        _logger.LogInformation("Fetching active addresses for UserId {UserId}", userId);
        await _userValidation.ValidateUser(userId);
        var address = await _addressRepsository.GetActiveAddressByUserId(userId,pageNumber,pageSize);
        if (address.Count == 0)
        {
            _logger.LogWarning("No active addresses found for UserId {UserId}", userId);
            throw new DataNotFoundException("No Address Is Found");
        }
        _logger.LogInformation("{Count} active addresses found for UserId {UserId}", address.Count, userId);
        return _mapper.Map<List<ResponseGetAddressDTO>>(address);
    }
    public async Task<List<ResponseGetAddressDTO>> GetAllTheVendorAddress(int vendorUserId, bool? status, int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching addresses for Vendor UserId {VendorUserId}", vendorUserId);
        await _userValidation.ValidateUser(vendorUserId);
        var address = await _addressRepsository.GetAddressByUserId(vendorUserId, status, pageNumber, pageSize);
        if (address.Count == 0)
        {
            _logger.LogWarning("No addresses found for Vendor UserId {VendorUserId}", vendorUserId);
            throw new DataNotFoundException("No Address Is Found");
        }
        _logger.LogInformation("{Count} addresses found for Vendor UserId {VendorUserId}", address.Count, vendorUserId);
        return _mapper.Map<List<ResponseGetAddressDTO>>(address);
    }
}