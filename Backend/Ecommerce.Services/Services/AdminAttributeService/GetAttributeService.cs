using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<PagedResponse<ResponseAdminGetAttribute>> GetAllAttributeAdmin(RequestAttributeFilter request,int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        var (attribute,totalCount) = await _attributeRepsository.GetAllAttributeAdmin(request);
        if(totalCount == 0)
        {
            throw new DataNotFoundException("No Attribute is found");
        }
        return new PagedResponse<ResponseAdminGetAttribute>
        {
            Items = _mapper.Map<List<ResponseAdminGetAttribute>>(attribute),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResponse<ResponseAdminGetCategoryAttribute>> GetAdminCategoryAttribute(RequestSubCategoryAttributeFilter request,int adminUserId)
    {
       await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        var productAttribute = await _productSubCategoryAttributeRepsository.GetAdminCategoryAttribute(request);
        if(productAttribute.totalCount == 0)
        {
            throw new DataNotFoundException("No Mapped attribute found");
        }
        return new PagedResponse<ResponseAdminGetCategoryAttribute>
        {
            Items = _mapper.Map<List<ResponseAdminGetCategoryAttribute>>(productAttribute.items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = productAttribute.totalCount
        };
    }
}