using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IVendorUserRepsository : IRepository<int,VendorUser>
{
    public Task<VendorUser?> GetOwnerVendorUserByVendorId(int vendorId);
    public Task<VendorUser?> GetVendorUserByUserId(int userId);
}