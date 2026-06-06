using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class FavoriteRepsository : AbstractRepository<int, Favorites> ,IFavoriteRepsository
{
    public FavoriteRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    public async Task<Favorites?> GetFavoriteByUserId(int userId)
    {
        var favorites = await _ecommerceContext.Favorites.FirstOrDefaultAsync(c=>c.UserId == userId);
        return favorites;
    }
}