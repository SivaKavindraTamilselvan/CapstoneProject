using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IFavoriteRepsository : IRepository<int, Favorites>
{
    public Task<Favorites?> GetFavoriteByUserId(int userId);
}