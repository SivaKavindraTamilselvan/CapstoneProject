using AutoMapper;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class AdminProductService : IAdminProductService
{
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly IProductRepsository _productRepsository;
    private readonly IProductCategoryRepsository _productCategoryRepsository;
    private readonly IProductSubCategoryRepsository _productSubCategoryRepsository;
    private readonly IProductSubCategoryAttributeRepsository _productSubCategoryAttributeRepsository;
    private readonly IAttributeRepsository _attributeRepsository;
    private readonly IMapper _mapper;
    public AdminProductService(IMapper mapper,IAdminUserRepsository adminUserRepsository,IProductRepsository productRepsository,IProductCategoryRepsository productCategoryRepsository,IProductSubCategoryRepsository productSubCategoryRepsository,IAttributeRepsository attributeRepsository,IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository)
    {
        _adminUserRepsository = adminUserRepsository;
        _productRepsository = productRepsository;
        _productCategoryRepsository = productCategoryRepsository;
        _productSubCategoryRepsository = productSubCategoryRepsository;
        _productSubCategoryAttributeRepsository = productSubCategoryAttributeRepsository;
        _attributeRepsository = attributeRepsository;
        _mapper = mapper;
    }
}