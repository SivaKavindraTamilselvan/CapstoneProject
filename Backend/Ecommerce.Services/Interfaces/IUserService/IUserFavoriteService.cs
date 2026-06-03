namespace Ecommerce.Services.Interfaces;

public interface IUserFavoriteService
{
    public Task<ResponseFavoriteItemsDTO> AddFavorite(RequestAddFavoriteItemsDTO requestAddFavoriteItemsDTO, int userId);
    public Task<ResponseFavoriteItemsDTO> DeleteFavorite(RequestDeleteFavoriteItemsDTO requestDeleteFavoriteItemsDTO);
    public Task<ResponseFavoriteItemsDTO> GetFavoriteByUserId(int userId);
}