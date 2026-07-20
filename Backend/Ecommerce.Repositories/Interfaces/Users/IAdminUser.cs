using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IAdminUserRepsository : IRepository<int, AdminUser>
{
    public Task<List<int>> GetOrderAdminUserIds();
    public Task<List<int>> GetShipmentAdminUserIds();
    public Task<List<int>> GetProductAdminUserIds();
    public Task<AdminUser?> GetAdminUserByAdminUserId(int adminUserId);
    public Task<AdminUser?> GetAdminUserByUserId(int userId);
    public Task<(List<AdminUser> items, int totalCount)> GetAllAdminUser(RequestAdiminUserFilter request);
}