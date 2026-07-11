using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserFavoritesService : IUserFavoriteService
{
    private readonly IProductImageRepsository _productImageRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IFavoriteItemsRepsository _favoriteItemsRepsository;
    private readonly IFavoriteValidation _favoriteValidation;
    private readonly IMapper _mapper;

    public UserFavoritesService(IProductImageRepsository productImageRepsository,IProductVariantRepsository productVariantRepsository,IFavoriteItemsRepsository favoriteItemsRepsository,IMapper mapper,IFavoriteValidation favoriteValidation)
    {
        _productImageRepsository = productImageRepsository;
        _productVariantRepsository = productVariantRepsository;
        _favoriteItemsRepsository = favoriteItemsRepsository;
        _favoriteValidation = favoriteValidation;
        _mapper = mapper;
    }
}