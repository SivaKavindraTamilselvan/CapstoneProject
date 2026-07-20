using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IVendorUserRepsository : IRepository<int, VendorUser>
{
         public  Task<List<VendorUser>> GetOrderVendorUserByVendorId(int vendorId);
    public Task<List<int>> GetAllProductVendorUserIds();
    public Task<VendorUser?> CheckUserIsVendorOwner(int userId);
    public Task<VendorUser?> GetVendorUserByVendorUserId(int userId);
    public Task<(List<VendorUser> items, int totalCount)> GetVendorsForVendor(RequestVendorUserFilter request, int vendorId);
    public Task<(List<VendorUser> items, int totalCount)> GetVendorsForAdmin(RequestAdminVendorUserFilter request);
    public Task<VendorUser?> GetOwnerVendorUserByVendorId(int vendorId);
    public Task<VendorUser?> GetVendorUserByUserId(int userId);
}