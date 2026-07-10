using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IReturnRepsository : IRepository<int,Return>
{
    public Task<Return?> GetReturnSummaryById(int returnId);
    public Task<(List<Return> data, int totalCount)> GetAllReturnsForUser(RequestUserReturnFilter request,int userId);
    public Task<(List<Return> data, int totalCount)> GetAllReturnsForVendor(RequestVendorReturnFilter request,int vendorId);
    public Task<(List<Return> data, int totalCount)> GetAllReturnsForAdmin( RequestAdminReturnFilter request);
    public Task<Return?> GetTheReturnUserByReturnId(int returnId);
    public Task<Return?> GetTheReturnInventoryByReturnId(int returnId);
    public Task<Return?> GetTheReturnProductByReturnId(int returnId);
}