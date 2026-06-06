using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
{
    private readonly IFavoriteRepsository _favoriteRepsository;
    private readonly IFavoriteItemsRepsository _favoriteItemsRepsository;
    private readonly IFavoriteValidation _favoriteValidation;
    private readonly IMapper _mapper;

    public UserFavoritesService(IFavoriteRepsository favoriteRepsository,IFavoriteItemsRepsository favoriteItemsRepsository,IMapper mapper,IFavoriteValidation favoriteValidation)
    {
        _favoriteRepsository = favoriteRepsository;
        _favoriteItemsRepsository = favoriteItemsRepsository;
        _favoriteValidation = favoriteValidation;
        _mapper = mapper;
    }
}