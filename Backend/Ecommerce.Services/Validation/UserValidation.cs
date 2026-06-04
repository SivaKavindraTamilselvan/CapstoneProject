using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class UserValidation : IUserValidation
{
    private readonly IUserRepsository _userRepsository;
    public UserValidation(IUserRepsository userRepsository)
    {
        _userRepsository = userRepsository;
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
}