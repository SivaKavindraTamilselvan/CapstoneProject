using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserFavoritesService : IUserFavoriteService
{
    private readonly IUserValidation _userValidation;
    private readonly EcommerceContext _ecommerceContext;
    private readonly ILogChanges _logChanges;
    private readonly IProductImageRepsository _productImageRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IFavoriteItemsRepsository _favoriteItemsRepsository;
    private readonly IFavoriteValidation _favoriteValidation;
    private readonly ILogger<UserFavoritesService> _logger;
    private readonly IMapper _mapper;

    public UserFavoritesService(IUserValidation userValidation,EcommerceContext ecommerceContext,ILogChanges logChanges,ILogger<UserFavoritesService> logger,IProductImageRepsository productImageRepsository,IProductVariantRepsository productVariantRepsository,IFavoriteItemsRepsository favoriteItemsRepsository,IMapper mapper,IFavoriteValidation favoriteValidation)
    {
        _userValidation = userValidation;
        _ecommerceContext = ecommerceContext;
        _logChanges = logChanges;
        _productImageRepsository = productImageRepsository;
        _productVariantRepsository = productVariantRepsository;
        _favoriteItemsRepsository = favoriteItemsRepsository;
        _favoriteValidation = favoriteValidation;
        _mapper = mapper;
        _logger = logger;
    }
}