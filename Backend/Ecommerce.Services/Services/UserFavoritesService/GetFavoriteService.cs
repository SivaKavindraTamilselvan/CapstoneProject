using Ecommerce.Services.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<ResponseFavoriteItemsDTO> GetFavoriteByUserId(int userId)
    {
        var cartItems = await _favoriteValidation.ValidateGetFavoriteItemsByUserId(userId);
        return _mapper.Map<ResponseFavoriteItemsDTO>(cartItems);
    }
}