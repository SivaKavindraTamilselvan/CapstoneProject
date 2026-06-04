using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<ResponseFavoriteItemsDTO> AddFavorite(RequestAddFavoriteItemsDTO requestAddFavoriteItemsDTO,int userId)
    {
        var favorite = (await _favoriteRepsository.GetAll()).FirstOrDefault(f=>f.UserId == userId);
        if(favorite == null)
        {
            throw new Exception("Product Already Added To Favorites");
        }
        var product = (await _favoriteItemsRepsository.GetAll()).FirstOrDefault(f=>f.ProductVariantId == requestAddFavoriteItemsDTO.ProductVariantId && f.FavoritesId == favorite.FavoritesId);
        if(product != null)
        {
            throw new Exception("Product Already Added To Favorites");
        }
        FavoritesItems favoritesItems = new FavoritesItems();
        favoritesItems.FavoritesId = favorite.FavoritesId;
        favoritesItems.ProductVariantId = requestAddFavoriteItemsDTO.ProductVariantId;
        await _favoriteItemsRepsository.Create(favoritesItems);
        return _mapper.Map<ResponseFavoriteItemsDTO>(favoritesItems);

    }
}