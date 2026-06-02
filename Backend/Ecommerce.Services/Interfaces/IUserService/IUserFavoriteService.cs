namespace Ecommerce.Services.Interfaces;
public interface IUserFavoriteService
{
    public Task<ResponseFavoriteItemsDTO> AddFavorite(RequestAddFavoriteItemsDTO requestAddFavoriteItemsDTO, int userId);
}