using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserCartService : IUserCartService
{
    private readonly ICartItemsRepsository _cartItemsRepsository;
    private readonly ICartValidation _cartValidation;
    private readonly IMapper _mapper;

    public UserCartService(ICartItemsRepsository cartItemsRepsository,IMapper mapper,ICartValidation cartValidation)
    {
        _cartItemsRepsository = cartItemsRepsository;
        _cartValidation = cartValidation;
        _mapper = mapper;
    }
}