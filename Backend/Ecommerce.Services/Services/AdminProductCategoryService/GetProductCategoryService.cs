using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<List<ResponseAdminGetAllCategory>> GetAllProductCategoryForAdmin(bool? status,int pageNumber,int pageSize)
    {
        var product = await _productCategoryRepsository.GetAllProductCategoryForAdmin(status,pageNumber,pageSize);
        if(product.totalCount == 0)
        {
            throw new DataNotFoundException("No Product Category Found");
        }
        return _mapper.Map<List<ResponseAdminGetAllCategory>>(product);
    }
    public async Task<List<ResponseAdminGetAllSubCategory>> GetAllProductSubCategoryForAdminGetAllSubProductCategoryForAdmin(bool? status,int? categoryId,int pageNumber,int pageSize)
    {
        var product = await _productSubCategoryRepsository.GetAllSubProductCategoryForAdmin(status,categoryId,pageNumber,pageSize);
        if(product.totalCount == 0)
        {
            throw new DataNotFoundException("No Product Sub Category Found");
        }
        return _mapper.Map<List<ResponseAdminGetAllSubCategory>>(product);
    }
}