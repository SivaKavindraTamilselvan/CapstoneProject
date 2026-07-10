using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface ICartRepsository : IRepository<int,Cart>
{
    public Task<Cart?> GetCartByUserId(int userId);
}