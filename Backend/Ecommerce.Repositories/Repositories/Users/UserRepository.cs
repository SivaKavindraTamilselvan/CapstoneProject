using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class UserRepsository : AbstractRepository<int, User> ,IUserRepsository
{
    public UserRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {
        
    }
    public async Task<User?> GetUserByEmail(string Email)
    {
        var result = await _ecommerceContext.User.FirstOrDefaultAsync(u=>u.Email == Email);
        return result;
    } 

}