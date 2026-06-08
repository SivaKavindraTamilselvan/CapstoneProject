using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<ResponseAddProductCategoryDTO> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO,int adminuserid)
    {
        var admin = await _adminUserValidation.ValidateAdminUserByUserId(adminuserid);
        await _productCategoryValidation.ValidateProductCategoryName(requestAddProductCategoryDTO.ProductCategoryName);
        ProductCategory productCategory = new ProductCategory();
        productCategory.ProductCategoryName = requestAddProductCategoryDTO.ProductCategoryName;
        productCategory.AddedByAdminId = admin.AdminUserId;
        await _productCategoryRepsository.Create(productCategory);
        return _mapper.Map<ResponseAddProductCategoryDTO>(productCategory);
    }
    public async Task<ResponseAddProductSubCategoryDTO> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO,int adminuserid)
    {
        var admin = await _adminUserValidation.ValidateAdminUserByUserId(adminuserid);
        await _productCategoryValidation.ValidateCategory(requestAddProductSubCategoryDTO.ProductCategoryId);
        await _productCategoryValidation.ValidateProductSubCategoryName(requestAddProductSubCategoryDTO.ProductSubCategoryName);
        ProductSubCategory productSubCategory = new ProductSubCategory();
        productSubCategory.ProductCategoryId = requestAddProductSubCategoryDTO.ProductCategoryId;
        productSubCategory.ProductSubCategoryName = requestAddProductSubCategoryDTO.ProductSubCategoryName;
        productSubCategory.AddedByAdminId = admin.AdminUserId;
        await _productSubCategoryRepsository.Create(productSubCategory);
        return _mapper.Map<ResponseAddProductSubCategoryDTO>(productSubCategory);
    }
}