namespace Ecommerce.Services.Interfaces;
public interface IUserFavoriteService
{
    public Task<ResponseAddFavoriteItemsDTO> AddFavorite(RequestAddFavoriteItemsDTO requestAddFavoriteItemsDTO, int userId);
}