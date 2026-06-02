using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface ICartItemsRepsository : IRepository<int, CartItems>
{
    public Task<List<CartItems>> DeleteCartItemsByUserId(int userId);

}