using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    private readonly IProductCategoryRepsository _productCategoryRepsository;
    private readonly IProductSubCategoryRepsository _productSubCategoryRepsository;
    private readonly IProductCategoryValidation _productCategoryValidation;
    private readonly IMapper _mapper;
    public AdminProductCategoryService(IMapper mapper, IProductCategoryRepsository productCategoryRepsository, IProductSubCategoryRepsository productSubCategoryRepsository, IProductCategoryValidation productCategoryValidation)
    {

        _productCategoryRepsository = productCategoryRepsository;
        _productSubCategoryRepsository = productSubCategoryRepsository;
        _productCategoryValidation = productCategoryValidation;
        _mapper = mapper;
    }
}