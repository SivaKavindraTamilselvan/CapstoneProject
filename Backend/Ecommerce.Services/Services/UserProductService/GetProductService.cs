using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class UserProductService :IUserProductService
{
    private readonly IProductCategoryRepsository _productCategoryRepsository;
    private readonly IAttributeRepsository _attributeRepsository;
    private readonly IProductSubCategoryAttributeRepsository _productSubCategoryAttributeRepsository;
    private readonly IProductSubCategoryRepsository _productSubCategoryRepsository;
    private readonly IMapper _mapper;
    public UserProductService(IProductCategoryRepsository productCategoryRepsository,IMapper mapper,IAttributeRepsository attributeRepsository,IProductSubCategoryAttributeRepsository productSubCategoryAttributeRepsository,IProductSubCategoryRepsository productSubCategoryRepsository)
    {
        _productCategoryRepsository = productCategoryRepsository;
        _productSubCategoryAttributeRepsository = productSubCategoryAttributeRepsository;
        _attributeRepsository = attributeRepsository;
        _productSubCategoryRepsository = productSubCategoryRepsository;
        _mapper = mapper;
    }
    public async Task<List<ResponseGetAllProductCategory>> GetAllProductCategory()
    {
        var productCategory  = await _productCategoryRepsository.GetAll();
        return _mapper.Map<List<ResponseGetAllProductCategory>>(productCategory);
    }
    public async Task<List<ResponseGetAllAttributeName>> GetAllAttributeNames()
    {
        var attribute = await _attributeRepsository.GetAll();
        return _mapper.Map<List<ResponseGetAllAttributeName>>(attribute);
    }
    public async Task<List<ResponseGetAllProductSubCategoryAttributeName>> GetAllProductSubCategoryAttributeNames(RequestGetAllProductSubCategoryAttributeName requestGetAllProductSubCategoryAttributeName)
    {
        var productSubcategory = await _productSubCategoryAttributeRepsository.GetAllProductSubCategoryAttribute(requestGetAllProductSubCategoryAttributeName.ProductSubCategoryAttributeId);
        return _mapper.Map<List<ResponseGetAllProductSubCategoryAttributeName>>(productSubcategory);
    }
    public async Task<List<ResponseGetAllProductSubCategoryName>> GetAllProductSubCategoryNames(RequestGetAllProductSubCategoryName requestGetAllProductSubCategoryName)
    {
        var productSubcategory = await _productSubCategoryRepsository.GetAllProductSubCategory(requestGetAllProductSubCategoryName.ProductCategoryId);
        return _mapper.Map<List<ResponseGetAllProductSubCategoryName>>(productSubcategory);
    }
    public async Task<List<ResponseGetAllProductSubCategoryNameVendor>> GetAllProductSubCategoryNamesVendor(RequestGetAllProductSubCategoryName requestGetAllProductSubCategoryName)
    {
        var productSubcategory = await _productSubCategoryRepsository.GetAllProductSubCategory(requestGetAllProductSubCategoryName.ProductCategoryId);
        return _mapper.Map<List<ResponseGetAllProductSubCategoryNameVendor>>(productSubcategory);
    }
}