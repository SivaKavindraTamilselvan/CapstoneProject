using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class UserValidation : IUserValidation
{
    private readonly IUserRepsository _userRepsository;
    private readonly IAddressRepsository _addressRepsository;
    public UserValidation(IUserRepsository userRepsository,IAddressRepsository addressRepsository)
    {
        _userRepsository = userRepsository;
        _addressRepsository = addressRepsository;
    }
    public async Task<User> ValidateUser(int UserId)
    {
        var user = await _userRepsository.Get(UserId);
        if(user == null)
        {
            throw new DataNotFoundException("The User Is Not");
        }
        return user;
    }
    public async Task<Address> ValidateAddress(int addressId,int userId)
    {
        var address = await _addressRepsository.Get(addressId);
        if(address == null)
        {
            throw new DataNotFoundException("Address Not Found");
        }
        if(address.UserId !=userId)
        {
            throw new DataApprovalStatusException("You Cannot access other address datas");
        }
        return address;
    }
}