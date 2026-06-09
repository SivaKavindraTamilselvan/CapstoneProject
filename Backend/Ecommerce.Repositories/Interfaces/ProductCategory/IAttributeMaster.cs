using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IAttributeRepsository : IRepository<int, AttributeMaster>
{
    public Task<AttributeMaster?> GetTheAttributeByName(string attributeName);
    public Task<(List<AttributeMaster>, int totalCount)> GetAllAttributeAdmin(bool? status, int pageNumber, int pageSize);

}