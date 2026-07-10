using Ecommerce.Services.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<ResponseGetFavoriteDTO> GetFavoriteByUserId(int userId)
    {
        var favoriteItems = await _favoriteValidation.ValidateGetFavoriteItemsByUserId(userId);
        return _mapper.Map<ResponseGetFavoriteDTO>(favoriteItems);
    }
}