using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class VendorRepsository : AbstractRepository<int, Vendor>, IVendorRepsository
{
    public VendorRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
    // used for registration validation
    public async Task<Vendor?> GetVendorByCompanyName(string companyname)
    {
        return await _ecommerceContext.Vendor.FirstOrDefaultAsync(v => v.VendorCompanyName == companyname);
    }
    // used for registration validation
    public async Task<Vendor?> GetVendorByCompanyEmail(string Email)
    {
        return await _ecommerceContext.Vendor.FirstOrDefaultAsync(v => v.CompanyEmail == Email);
    }
    // used for registration validation
    public async Task<Vendor?> GetVendorByCompanyGSTNumber(string gst)
    {
        return await _ecommerceContext.Vendor.FirstOrDefaultAsync(v => v.GSTNumber == gst);
    }
    // used for registration validation
    public async Task<Vendor?> GetVendorByCompanyPhoneNumber(string phone)
    {
        return await _ecommerceContext.Vendor.FirstOrDefaultAsync(p => p.CompanyPhoneNumber == phone);
    }
    public async Task<(List<Vendor> items, int totalCount)> GetVendorsForAdmin(RequestAdminVendorFilter request)
    {
        var query = _ecommerceContext.Vendor.Include(v => v.ReviwedByAdmin).ThenInclude(a => a!.User).Include(a => a.ApprovalStatus).AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.CompanyEmail))
        {
            query = query.Where(a => a.CompanyEmail.ToLower() == request.CompanyEmail.ToLower());
        }
        if (!string.IsNullOrWhiteSpace(request.CompanyPhoneNumber))
        {
            query = query.Where(a => a.CompanyPhoneNumber == request.CompanyPhoneNumber);
        }
        if (!string.IsNullOrWhiteSpace(request.VendorCompanyName))
        {
            query = query.Where(a => a.VendorCompanyName.ToLower() == request.VendorCompanyName.ToLower());
        }
        if (!string.IsNullOrWhiteSpace(request.ContactPersonName))
        {
            query = query.Where(a => a.ContactPersonName.ToLower() == request.ContactPersonName.ToLower());
        }
        if (!string.IsNullOrWhiteSpace(request.GSTNumber))
        {
            query = query.Where(a => a.GSTNumber.ToLower() == request.GSTNumber.ToLower());
        }
        if (request.ApprovalStatusId.HasValue)
        {
            query = query.Where(a => a.ApprovalStatusId == request.ApprovalStatusId);
        }
        if (request.ReviewedByAdminId.HasValue)
        {
            query = query.Where(a => a.ReviewedByAdminId == request.ReviewedByAdminId);
        }
        if (request.IsActive.HasValue)
        {
            query = query.Where(a => a.IsActive == request.IsActive);
        }
        var totalCount = await query.CountAsync();
        var vendor = await query.OrderByDescending(p => p.CreatedAt).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (vendor, totalCount);
    }
    public async Task<List<int>> GetAllVendorOwnerUserIds()
    {
        return await _ecommerceContext.Vendor.Where(v => v.IsActive && v.ApprovalStatusId == 2).SelectMany(v => v.VendorUsers
        .Where(vu => vu.VendorRoleId == 1 && vu.IsActive).Select(vu => vu.UserId)).Distinct().ToListAsync();
    }

    public async Task<Vendor?> GetVendorsByVendorIdForAdmin(int vendorId)
    {
        var query = _ecommerceContext.Vendor.Include(v => v.ReviwedByAdmin).ThenInclude(a => a!.User).Include(a => a.ApprovalStatus).AsQueryable();
        return await query.Where(v=>v.VendorId == vendorId).FirstOrDefaultAsync();
    }


}