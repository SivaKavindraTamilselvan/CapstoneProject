using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IVendorRepsository : IRepository<int,Vendor>
{
    public Task<List<Vendor>> GetAllVednorNotPendingApproval();
}