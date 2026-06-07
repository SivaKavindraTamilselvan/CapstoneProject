using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<ResponseAddAttributeDTO> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO)
    {
        await _productAttributeValidation.ValidateAttributeName(requestAddAttributeDTO.AttributeName);
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