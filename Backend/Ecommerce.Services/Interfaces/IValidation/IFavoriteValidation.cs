using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IFavoriteValidation
{
    public Task<Favorites> ValidateFavoriteByUserId(int userId);
    public Task<FavoritesItems> ValidateFavoriteItems(int favoriteItemId);
    public Task ValidateFavoriteItemsByProductAndUser(int productVariantId, int favoriteId);
    public Task<List<FavoritesItems>> ValidateFavoriteItemsByUserId(int userId);
    public Task<List<FavoritesItems>> ValidateGetFavoriteItemsByUserId(int userId);

}