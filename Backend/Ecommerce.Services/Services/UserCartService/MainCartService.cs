using AutoMapper;
using Ecommerce.Data;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserCartService : IUserCartService
{
    private readonly IUserValidation _userValidation;
    private readonly EcommerceContext _ecommerceContext;
    private readonly ILogChanges _logChanges;
    private readonly ICartRepsository _cartRepsository;
    private readonly IProductValidation _productValidation;
    private readonly ICartItemsRepsository _cartItemsRepsository;
    private readonly ICartValidation _cartValidation;
    private readonly IProductImageRepsository _productImageRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserCartService> _logger;

    public UserCartService(IUserValidation userValidation,EcommerceContext ecommerceContext,ILogChanges logChanges,ILogger<UserCartService> logger,ICartRepsository cartRepsository,IProductVariantRepsository productVariantRepsository,IProductImageRepsository productImageRepsository,IProductValidation productValidation,ICartItemsRepsository cartItemsRepsository,IMapper mapper,ICartValidation cartValidation)
    {
        _userValidation = userValidation;
        _ecommerceContext = ecommerceContext;
        _cartRepsository = cartRepsository;
        _productVariantRepsository = productVariantRepsository;
        _productImageRepsository = productImageRepsository;
        _productValidation = productValidation;
        _cartItemsRepsository = cartItemsRepsository;
        _cartValidation = cartValidation;
        _mapper = mapper;
        _logger = logger;
        _logChanges = logChanges;
    }
}