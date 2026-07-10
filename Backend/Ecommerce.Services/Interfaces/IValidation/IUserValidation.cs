using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IUserValidation
{
    public Task<User> ValidateUser(int UserId);
    public Task<Address> ValidateAddress(int addressId,int userId);

}