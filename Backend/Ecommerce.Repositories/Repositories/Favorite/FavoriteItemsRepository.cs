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
        var items = await _ecommerceContext.FavoritesItems.Include(c => c.Favorites).Include(f=>f.ProductVariant).ThenInclude(f=>f!.Product).Where(u => u.Favorites != null && u.Favorites.UserId == userId).ToListAsync();
        return items;
    }
    public async Task<FavoritesItems?> GetFavoriteItemsByProductVariantAndCart(int productVariantId,int favoriteId)
    {
        var items = await _ecommerceContext.FavoritesItems.FirstOrDefaultAsync(p=>p.ProductVariantId == productVariantId && p.FavoritesId == favoriteId);
        return items;
    }
    public async Task<List<FavoritesItems>> DeleteFavoriteItemsByUserId(int userId)
    {
        var items = await _ecommerceContext.FavoritesItems.Where(u => u.Favorites != null && u.Favorites.UserId == userId).ToListAsync();
        _ecommerceContext.FavoritesItems.RemoveRange(items);
        await _ecommerceContext.SaveChangesAsync();
        return items;
    }
}