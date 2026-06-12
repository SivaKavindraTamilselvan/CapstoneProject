using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IAdminUserRepsository : IRepository<int, AdminUser>
{
    public Task<List<int>> GetProductAdminUserIds();
    public Task<AdminUser?> GetAdminUserByAdminUserId(int adminUserId);
    public Task<AdminUser?> GetAdminUserByUserId(int userId);
    public Task<List<AdminUser>> GetAllAdminUser(int? role, bool? status, int pageNumber, int pageSize);
}