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

    // validate user status and data
    public async Task<User> ValidateUser(int UserId)
    {
        var user = await _userRepsository.Get(UserId);
        if(user == null)
        {
            throw new DataNotFoundException("The User Is Not Found");
        }
        if(!user.IsActive)
        {
            throw new UnauthorizationException("User Is Deactivated");
        }
        return user;
    }

    // validate address and that address belong to that user
    public async Task<Address> ValidateAddress(int addressId,int userId)
    {
        await ValidateUser(userId);
        var address = await _addressRepsository.Get(addressId);
        if(address == null)
        {
            throw new DataNotFoundException("Address Not Found");
        }
        if(address.UserId !=userId)
        {
            throw new UnauthorizationException("You Cannot access other address datas");
        }
        return address;
    }
}