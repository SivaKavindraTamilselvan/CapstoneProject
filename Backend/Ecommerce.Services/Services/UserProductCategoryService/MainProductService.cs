using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserProductService : IUserProductService
{
    private readonly ILogger<UserProductService> _logger;
    private readonly IProductRepsository _productRepository;
    private readonly IMapper _mapper;

    public UserProductService(ILogger<UserProductService> logger,IProductRepsository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }
}