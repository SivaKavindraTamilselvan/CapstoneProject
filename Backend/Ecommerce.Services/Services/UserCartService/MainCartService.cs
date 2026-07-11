using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserCartService : IUserCartService
{
    private readonly IProductValidation _productValidation;
    private readonly ICartItemsRepsository _cartItemsRepsository;
    private readonly ICartValidation _cartValidation;
    private readonly IProductImageRepsository _productImageRepsository;
    private readonly IProductVariantRepsository _productVariantRepsository;
    private readonly IMapper _mapper;

    public UserCartService(IProductVariantRepsository productVariantRepsository,IProductImageRepsository productImageRepsository,IProductValidation productValidation,ICartItemsRepsository cartItemsRepsository,IMapper mapper,ICartValidation cartValidation)
    {
        _productVariantRepsository = productVariantRepsository;
        _productImageRepsository = productImageRepsository;
        _productValidation = productValidation;
        _cartItemsRepsository = cartItemsRepsository;
        _cartValidation = cartValidation;
        _mapper = mapper;
    }
}