using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserProductCategoryService :IUserProductCategoryService
{
    private readonly IProductCategoryRepsository _productCategoryRepsository;
    private readonly IAttributeRepsository _attributeRepsository;
    private readonly IProductSubCategoryAttributeRepsository _productSubCategoryAttributeRepsository;
    private readonly IProductSubCategoryRepsository _productSubCategoryRepsository;
    private readonly IMapper _mapper;
    public UserProductCategoryService(IProductCategoryRepsository productCategoryRepsository,IMapper mapper,IAttributeRepsository attributeRepsository,IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository,IProductSubCategoryRepsository productSubCategoryRepsository)
    {
        _productCategoryRepsository = productCategoryRepsository;
        _productSubCategoryAttributeRepsository = productSubCategoryAttributeRepsository;
        _attributeRepsository = attributeRepsository;
        _productSubCategoryRepsository = productSubCategoryRepsository;
        _mapper = mapper;
    }
}