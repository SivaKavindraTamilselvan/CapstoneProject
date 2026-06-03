using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
{
    public async Task<ResponseFavoriteItemsDTO> GetFavoriteByUserId(int userId)
    {
        var cartItems = await 
        if(cartItems.Count == 0)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        return _mapper.Map<ResponseFavoriteItemsDTO>(cartItems);
    }
}