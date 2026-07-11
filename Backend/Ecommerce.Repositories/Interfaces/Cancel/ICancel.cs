using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface ICancelRepsository : IRepository<int, Cancel>
{
    public Task<Cancel?> GetCancelById(int cancelId);
    public Task<(List<Cancel> data, int totalCount)> GetAllCancelsForUser(RequestUserCancelFilter request, int userId);
    public Task<(List<Cancel> data, int totalCount)> GetAllCancelsForAdmin(RequestAdminCancelFilter request);
    public Task<(List<Cancel> data, int totalCount)> GetAllCancelsForVendor(RequestVendorCancelFilter request, int vendorId);
}