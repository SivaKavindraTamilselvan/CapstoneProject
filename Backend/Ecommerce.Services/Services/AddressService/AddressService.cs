using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
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
        var user = _userValidation.ValidateUser(UserId);
        var address = _mapper.Map<Address>(requestAddAddressDTO);
        address.UserId = UserId;
        await _addressRepsository.Create(address);
        return _mapper.Map<ResponseAddAddressDTO>(address);
    }
}