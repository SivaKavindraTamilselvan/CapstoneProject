using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IUserRepsository : IRepository<int,User>
{
    public Task<User?> GetUserByEmail(string Email);
}