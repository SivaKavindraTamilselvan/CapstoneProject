using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IAttributeRepsository : IRepository<int, AttributeMaster>
{
    public Task<(List<AttributeMaster> items, int totalCount)> GetAllAttributeVendor();
    public Task<AttributeMaster?> GetTheAttributeByName(string attributeName);
    public Task<(List<AttributeMaster> items, int totalCount)> GetAllAttributeAdmin(RequestAttributeFilter request);
}