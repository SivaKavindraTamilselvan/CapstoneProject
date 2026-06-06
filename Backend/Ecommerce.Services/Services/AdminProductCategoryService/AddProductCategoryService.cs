using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class AdminProductCategoryService : IAdminProductCategoryService
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
}