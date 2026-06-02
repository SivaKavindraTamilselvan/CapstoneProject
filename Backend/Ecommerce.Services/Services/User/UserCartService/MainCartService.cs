using AutoMapper;
using Ecommerce.Repositories.Interfaces;

public partial class UserCartService : IUserCartService
{
    private readonly ICartRepsository _cartRepsository;
    private readonly ICartItemsRepsository _cartItemsRepsository;
    private readonly IMapper _mapper;

    public UserCartService(ICartItemsRepsository cartItemsRepsository,ICartRepsository cartRepsository,IMapper mapper)
    {
        _cartItemsRepsository = cartItemsRepsository;
        _cartRepsository = cartRepsository;
        _mapper = mapper;
    }
}