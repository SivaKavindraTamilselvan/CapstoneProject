using AutoMapper;
using Ecommerce.Repositories.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
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
}