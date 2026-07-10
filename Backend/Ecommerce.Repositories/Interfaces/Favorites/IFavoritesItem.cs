using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IFavoriteItemsRepsository : IRepository<int, FavoritesItems>
{
    public Task<List<FavoritesItems>> GetFavoriteItemsByUserId(int userId);
    public Task<FavoritesItems?> GetFavoriteItemsByProductVariantAndCart(int productVariantId,int favoriteId);
    public Task<List<FavoritesItems>> DeleteFavoriteItemsByUserId(int userId);

}