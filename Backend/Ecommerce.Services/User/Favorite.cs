using AutoMapper;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;

public class UserFavoritesService : IUserFavoriteService
{
    private readonly IFavoriteRepsository _favoriteRepsository;
    private readonly IFavoriteItemsRepsository _favoriteItemsRepsository;
    private readonly IMapper _mapper;

    public UserFavoritesService(IFavoriteRepsository favoriteRepsository,IFavoriteItemsRepsository favoriteItemsRepsository,IMapper mapper)
    {
        _favoriteRepsository = favoriteRepsository;
        _favoriteItemsRepsository = favoriteItemsRepsository;
        _mapper = mapper;
    }
    public async Task<ResponseAddFavoriteItemsDTO> AddFavorite(RequestAddFavoriteItemsDTO requestAddFavoriteItemsDTO,int userId)
    {
        Console.WriteLine(userId);
        var favorite = (await _favoriteRepsository.GetAll()).FirstOrDefault(f=>f.UserId == userId);
        if(favorite == null)
        {
            throw new Exception("Product Already Added To Favorites");
        }
        var product = (await _favoriteItemsRepsository.GetAll()).FirstOrDefault(f=>f.ProductVariantId == requestAddFavoriteItemsDTO.ProductVariantId && f.FavoritesId == favorite.FavoritesId);
        if(product != null)
        {
            throw new Exception("Product Already Added To Favorites");
        }
        FavoritesItems favoritesItems = new FavoritesItems();
        favoritesItems.FavoritesId = favorite.FavoritesId;
        favoritesItems.ProductVariantId = requestAddFavoriteItemsDTO.ProductVariantId;
        await _favoriteItemsRepsository.Create(favoritesItems);
        return _mapper.Map<ResponseAddFavoriteItemsDTO>(favoritesItems);

    }
}