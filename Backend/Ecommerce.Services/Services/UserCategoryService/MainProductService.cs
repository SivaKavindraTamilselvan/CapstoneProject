using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserProductCategoryService :IUserProductCategoryService
{
    private readonly IProductCategoryRepsository _productCategoryRepsository;
    private readonly IProductSubCategoryRepsository _productSubCategoryRepsository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserProductCategoryService> _logger;
    public UserProductCategoryService(ILogger<UserProductCategoryService> logger,IProductCategoryRepsository productCategoryRepsository,IMapper mapper,IProductSubCategoryRepsository productSubCategoryRepsository)
    {
        _productCategoryRepsository = productCategoryRepsository;
        _productSubCategoryRepsository = productSubCategoryRepsository;
        _mapper = mapper;
        _logger = logger;
    }
}