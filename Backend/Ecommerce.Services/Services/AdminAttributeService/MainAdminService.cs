using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    private readonly IProductSubCategoryAttributeRepsository _productSubCategoryAttributeRepsository;
    private readonly IAttributeRepsository _attributeRepsository;
    private readonly IProductAttributeValidation _productAttributeValidation;
    private readonly IMapper _mapper;
    public AdminProductAttributeService(IMapper mapper,IAttributeRepsository attributeRepsository, IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository,IProductAttributeValidation productAttributeValidation)
    {
        _productSubCategoryAttributeRepsository = productSubCategoryAttributeRepsository;
        _attributeRepsository = attributeRepsository;
        _productAttributeValidation = productAttributeValidation;
        _mapper = mapper;
    }
}