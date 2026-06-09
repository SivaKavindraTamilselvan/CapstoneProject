using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductAttributeService : IAdminProductAttributeService
{
    public async Task<List<ResponseAdminGetAttribute>> GetAllAttributeAdmin(bool? status, int pageNumber, int pageSize)
    {
        var attribute = await _attributeRepsository.GetAllAttributeAdmin(status,pageNumber,pageSize);
        if(attribute.totalCount == 0)
        {
            throw new DataNotFoundException("No Attribute is found");
        }
        return _mapper.Map<List<ResponseAdminGetAttribute>>(attribute);
    }

    public async Task<List<ResponseAdminGetCategoryAttribute>> GetAdminCategoryAttribute(bool? status,int? subcategoryid,int pageNumber,int pageSize)
    {
        var productAttribute = await _productSubCategoryAttributeRepsository.GetAdminCategoryAttribute(status,subcategoryid,pageNumber,pageSize);
        if(productAttribute.Count == 0)
        {
            throw new DataNotFoundException("No Mapped attribute found");
        }
        return _mapper.Map<List<ResponseAdminGetCategoryAttribute>>(productAttribute);
    }
}