using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class FavoriteValidation : IFavoriteValidation
{
    private readonly IFavoriteRepsository _favoriteRepsository;
    private readonly IFavoriteItemsRepsository _favoriteItemsRepsository;
    public FavoriteValidation(IFavoriteItemsRepsository favoriteItemsRepsository,IFavoriteRepsository favoriteRepsository)
    {
        _favoriteItemsRepsository = favoriteItemsRepsository;
        _favoriteRepsository = favoriteRepsository;
    }
    public async Task<Favorites> ValidateFavoriteByUserId(int userId)
    {
        var favorites = await _favoriteRepsository.GetFavoriteByUserId(userId);
        if (favorites == null)
        {
            throw new DataNotFoundException("Favorite Not Found");
        }
        return favorites;
    }
    public async Task<FavoritesItems> ValidateFavoriteItems(int favoriteItemId)
    {
        var favoriteItems = await _favoriteItemsRepsository.Get(favoriteItemId);
        if(favoriteItems ==null)
        {
            throw new DataNotFoundException("Favorite Items Is Not Found");
        }
        return favoriteItems;
    }
    public async Task ValidateFavoriteItemsByProductAndUser(int productVariantId, int favoriteId)
    {
        var favoriteItem = await _favoriteItemsRepsository.GetFavoriteItemsByProductVariantAndCart(productVariantId, favoriteId);
        if (favoriteItem != null)
        {
            throw new DataNotFoundException("Product Already Added To Favorite");
        }
    }
    public async Task<List<FavoritesItems>> ValidateFavoriteItemsByUserId(int userId)
    {
        var favoriteItems = await _favoriteItemsRepsository.DeleteFavoriteItemsByUserId(userId);
        if(favoriteItems.Count == 0)
        {
            throw new DataNotFoundException("Favorite Is Empty");
        }
        return favoriteItems;
    }
    public async Task<List<FavoritesItems>> ValidateGetFavoriteItemsByUserId(int userId)
    {
        var favoriteItems = await _favoriteItemsRepsository.GetFavoriteItemsByUserId(userId);
        if(favoriteItems.Count == 0)
        {
            throw new DataNotFoundException("Favorite Items Is Not Found");
        }
        return favoriteItems;
    }
}