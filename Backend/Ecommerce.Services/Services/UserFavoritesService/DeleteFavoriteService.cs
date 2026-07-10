using Ecommerce.Services.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<ResponseFavoriteItemsDTO> DeleteFavorite(RequestDeleteFavoriteItemsDTO requestDeleteFavoriteItemsDTO)
    {
        var favoritesItems = await _favoriteValidation.ValidateFavoriteItems(requestDeleteFavoriteItemsDTO.FavoritesItemsId);
        await _favoriteItemsRepsository.Delete(favoritesItems.FavoritesItemsId);
        return _mapper.Map<ResponseFavoriteItemsDTO>(favoritesItems);
    }
}