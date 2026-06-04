using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class AddressService : IAddressService
{
    private readonly IAddressRepsository _addressRepsository;
    private readonly IUserValidation _userValidation;
    private readonly IMapper _mapper;
    public AddressService(IAddressRepsository addressRepsository,IUserValidation userValidation,IMapper mapper)
    {
        _addressRepsository = addressRepsository;
        _userValidation = userValidation;
        _mapper = mapper;
    }
    public async Task<ResponseAddAddressDTO> AddAddress(RequestAddAddressDTO requestAddAddressDTO,int UserId)
    {
        var user = await _userValidation.ValidateUser(UserId);
        var address = _mapper.Map<Address>(requestAddAddressDTO);
        address.UserId = UserId;
        await _addressRepsository.Create(address);
        if(address.IsDefault)
        {
            RequestMakeDefaultAddressDTO requestMakeDefaultAddressDTO = new RequestMakeDefaultAddressDTO();
            requestMakeDefaultAddressDTO.AddressId = address.AddressId;
            await MakeAddressDefault(requestMakeDefaultAddressDTO);
        }
        return _mapper.Map<ResponseAddAddressDTO>(address);
    }
    public async Task<ResponseMakeDefaultAddressDTO> MakeAddressDefault(RequestMakeDefaultAddressDTO requestMakeDefaultAddressDTO)
    {
        var selectedAddress = await _addressRepsository.Get(requestMakeDefaultAddressDTO.AddressId);
        if(selectedAddress == null)
        {
            throw new DataNotFoundException("Address Not Found");
        }
        var userAddress = await _addressRepsository.GetAddressByUserId(selectedAddress.UserId);
        foreach(var address in userAddress)
        {
            address.IsDefault = false;
            address.UpdatedAt = DateTime.Now;
            await _addressRepsository.Update(address.AddressId,address);
        }
        selectedAddress.IsDefault = true;
        selectedAddress.UpdatedAt = DateTime.Now;
        await _addressRepsository.Update(selectedAddress.AddressId,selectedAddress);
        return _mapper.Map<ResponseMakeDefaultAddressDTO>(selectedAddress);
    }
}