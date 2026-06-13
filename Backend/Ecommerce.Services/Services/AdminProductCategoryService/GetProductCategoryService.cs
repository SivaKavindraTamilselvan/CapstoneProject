using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductCategoryService : IAdminProductCategoryService
{
    public async Task<PagedResponse<ResponseAdminGetAllCategory>> GetAllProductCategoryForAdmin(RequestProductCategoryFilter request,int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Fetching Product Categories. PageNumber: {PageNumber}, PageSize: {PageSize}, CategoryId: {CategoryId}, Status: {Status}", request.PageNumber, request.PageSize, request.ProductCategoryId, request.status);
        var (product, totalCount) = await _productCategoryRepsository.GetAllProductCategoryForAdmin(request);
        if (totalCount == 0)
        {
            _logger.LogWarning("No Product Categories found for the given filter criteria");
            throw new DataNotFoundException("No Product Category Found");
        }
        _logger.LogInformation("Retrieved {TotalCount} Product Categories", totalCount);
        return new PagedResponse<ResponseAdminGetAllCategory>
        {
            Items = _mapper.Map<List<ResponseAdminGetAllCategory>>(product),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<PagedResponse<ResponseAdminGetAllSubCategory>> GetAllSubProductCategoryForAdmin(RequestProductSubCategoryFilter request,int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Fetching Product SubCategories. PageNumber: {PageNumber}, PageSize: {PageSize}, SubCategoryId: {SubCategoryId}, CategoryId: {CategoryId}, Status: {Status}", request.PageNumber, request.PageSize, request.ProductSubCategoryId, request.ProductCategoryId, request.status);

        var (product, totalCount) = await _productSubCategoryRepsository.GetAllSubProductCategoryForAdmin(request);
        if (totalCount == 0)
        {
            _logger.LogWarning("No Product SubCategories found for the given filter criteria");
            throw new DataNotFoundException("No Product Sub Category Found");
        }
        _logger.LogInformation("Retrieved {TotalCount} Product SubCategories", totalCount);
        return new PagedResponse<ResponseAdminGetAllSubCategory>
        {
            Items = _mapper.Map<List<ResponseAdminGetAllSubCategory>>(product),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}