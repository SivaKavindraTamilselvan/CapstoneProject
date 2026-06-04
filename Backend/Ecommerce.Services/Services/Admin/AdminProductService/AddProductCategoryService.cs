using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductService : IAdminProductService
{
    public async Task<ResponseAddProductCategoryDTO> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO)
    {
        var product = await _productCategoryValidation.ValidateProductCategoryName(requestAddProductCategoryDTO.ProductCategoryName);
        ProductCategory productCategory = new ProductCategory();
        productCategory.ProductCategoryName = requestAddProductCategoryDTO.ProductCategoryName;
        await _productCategoryRepsository.Create(productCategory);
        return _mapper.Map<ResponseAddProductCategoryDTO>(productCategory);
    }
    public async Task<ResponseAddProductSubCategoryDTO> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO)
    {
        var product = await _productCategoryValidation.ValidateProductSubCategoryName(requestAddProductSubCategoryDTO.ProductSubCategoryName);
        ProductSubCategory productSubCategory = new ProductSubCategory();
        productSubCategory.ProductCategoryId = requestAddProductSubCategoryDTO.ProductCategoryId;
        productSubCategory.ProductSubCategoryName = requestAddProductSubCategoryDTO.ProductSubCategoryName;
        await _productSubCategoryRepsository.Create(productSubCategory);
        return _mapper.Map<ResponseAddProductSubCategoryDTO>(productSubCategory);
    }
    public async Task<ResponseAddAttributeDTO> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO)
    {
        var attribute = await _productCategoryValidation.ValidateAttributeName(requestAddAttributeDTO.AttributeName);
        AttributeMaster createAttribute = new AttributeMaster();
        createAttribute.AttributeName = requestAddAttributeDTO.AttributeName;
        await _attributeRepsository.Create(createAttribute);
        return _mapper.Map<ResponseAddAttributeDTO>(createAttribute);
    }
    public async Task<ResponseAddProductSubCategoryAttributeDTO> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO)
    {
        var product = await _productSubCategoryAttributeRepsository.CheckProductSubCategoryAttribute(requestAddProductSubCategoryAttributeDTO.AttributeMasterId,requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId);
        ProductSubCategoryAttribute productSubCategoryAttribute = new ProductSubCategoryAttribute();
        productSubCategoryAttribute.AttributeMasterId = requestAddProductSubCategoryAttributeDTO.AttributeMasterId;
        productSubCategoryAttribute.ProductSubCategoryId = requestAddProductSubCategoryAttributeDTO.ProductSubCategoryId;
        await _productSubCategoryAttributeRepsository.Create(productSubCategoryAttribute);
        return _mapper.Map<ResponseAddProductSubCategoryAttributeDTO>(productSubCategoryAttribute);
    }
}