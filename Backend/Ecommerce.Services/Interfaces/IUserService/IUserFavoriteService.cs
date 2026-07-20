namespace Ecommerce.Services.Interfaces;

public interface IUserFavoriteService
{
    public Task<ResponseFavoriteItemsDTO> AddFavorite(RequestAddFavoriteItemsDTO requestAddFavoriteItemsDTO, int userId);
    public Task<ResponseFavoriteItemsDTO> DeleteFavorite(RequestDeleteFavoriteItemsDTO requestDeleteFavoriteItemsDTO,int userId);
    public  Task<List<ResponseGetFavoriteDTO>>  GetFavoriteByUserId(int userId);
}