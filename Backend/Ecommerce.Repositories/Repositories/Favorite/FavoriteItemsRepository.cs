using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class FavoriteItemsRepsository : AbstractRepository<int, FavoritesItems>, IFavoriteItemsRepsository
{
    public FavoriteItemsRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<List<FavoritesItems>> GetFavoriteItemsByUserId(int userId)
    {
        var items = await _ecommerceContext.FavoritesItems.Include(c => c.Favorites).Where(u => u.Favorites != null && u.Favorites.UserId == userId).ToListAsync();
        if (items.Count == 0)
        {
            return new List<FavoritesItems>();
        }
        return items;
    }
}