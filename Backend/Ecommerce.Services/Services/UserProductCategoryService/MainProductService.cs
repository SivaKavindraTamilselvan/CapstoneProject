using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserProductService : IUserProductService
{
    private readonly IProductRepsository _productRepository;
    private readonly IMapper _mapper;

    public UserProductService(IProductRepsository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
}