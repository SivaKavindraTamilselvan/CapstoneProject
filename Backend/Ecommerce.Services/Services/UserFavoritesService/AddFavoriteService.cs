using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<ResponseFavoriteItemsDTO> AddFavorite(RequestAddFavoriteItemsDTO requestAddFavoriteItemsDTO,int userId)
    {
        var favorite = await _favoriteValidation.ValidateFavoriteByUserId(userId);
        await _favoriteValidation.ValidateFavoriteItemsByProductAndUser(requestAddFavoriteItemsDTO.ProductVariantId,favorite.FavoritesId);
        
        var productVariant = await _productVariantRepsository.Get(requestAddFavoriteItemsDTO.ProductVariantId);
        if(productVariant == null)
            throw new Exception("Product Variant Not Found");

        FavoritesItems favoritesItems = new FavoritesItems();
        favoritesItems.FavoritesId = favorite.FavoritesId;
        favoritesItems.ProductVariantId = requestAddFavoriteItemsDTO.ProductVariantId;
        favoritesItems.LastNotifiedPrice = productVariant.Price;
        
        await _favoriteItemsRepsository.Create(favoritesItems);
        return _mapper.Map<ResponseFavoriteItemsDTO>(favoritesItems);
    }
}