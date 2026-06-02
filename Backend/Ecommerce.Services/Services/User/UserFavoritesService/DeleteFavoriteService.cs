using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<ResponseFavoriteItemsDTO> DeleteFavorite(RequestDeleteFavoriteItemsDTO requestDeleteFavoriteItemsDTO)
    {
        var favoritesItems = (await _favoriteItemsRepsository.GetAll()).FirstOrDefault(c=>c.FavoritesItemsId == requestDeleteFavoriteItemsDTO.FavoritesItemsId);
        if(favoritesItems ==null)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        await _favoriteItemsRepsository.Delete(favoritesItems.FavoritesItemsId);
        return _mapper.Map<ResponseFavoriteItemsDTO>(favoritesItems);
    }
    public async Task<List<ResponseFavoriteItemsDTO>> DeleteAllFavorite()
    {
        var favoritesItems = await _favoriteItemsRepsository.GetAll();
        await _favoriteItemsRepsository.DeleteAll();
        return _mapper.Map<List<ResponseFavoriteItemsDTO>>(favoritesItems);
    }
}