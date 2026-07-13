using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class InventoryRepsository : AbstractRepository<int, Inventory>, IInventoryRepsository
{
    public InventoryRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    private IQueryable<Inventory> BaseQuery()
    {
        return _ecommerceContext.Inventory.Include(a => a.Address)
        .Include(i => i.ProductVariant).ThenInclude(i => i!.Product).ThenInclude(i => i!.Vendor).AsNoTracking();
    }
    public async Task<(List<Inventory>items,int totalQuantity)> GetInventoryForVendor(RequestVendorInventoryFilter request, int vendorId)
    {
        var query = BaseQuery().Where(i => i.ProductVariant!.Product!.VendorId == vendorId && i.Address!.IsActive == true);
        if (request.AddressId.HasValue)
        {
            query = query.Where(a => a.AddressId == request.AddressId.Value);
        }
        if (request.ProductVariantId.HasValue)
        {
            query = query.Where(a => a.ProductVariantId == request.ProductVariantId.Value);
        }
        if (request.Status.HasValue)
        {
            query = query.Where(a => a.IsActive == request.Status.Value);
        }
        if (request.MaximumAvailableQuantity.HasValue)
        {
            query = query.Where(a => a.AvailableQuantity <= request.MaximumAvailableQuantity.Value);
        }
        if (request.MinimumAvailableQuantity.HasValue)
        {
            query = query.Where(a => a.AvailableQuantity >= request.MinimumAvailableQuantity.Value);
        }
        if (request.MaximumReservedQuantity.HasValue)
        {
            query = query.Where(a => a.ReservedQuantity <= request.MaximumReservedQuantity.Value);
        }
        if (request.MinimumReservedQuantity.HasValue)
        {
            query = query.Where(a => a.ReservedQuantity <= request.MinimumReservedQuantity.Value);
        }
        var totalQuantity = await query.CountAsync();
        var items = await query.OrderByDescending(a=>a.UpdatedAt).Skip((request.PageNumber - 1)*request.PageSize).Take(request.PageSize).ToListAsync();
        return (items,totalQuantity);
    }
    public async Task<(List<Inventory>items,int totalQuantity)> GetInventoryForAdmin(RequestAdminInventoryFilter request)
    {
        var query = BaseQuery();
        if (request.VendorId.HasValue)
        {
            query = query.Where(a => a.ProductVariant!.Product!.VendorId == request.VendorId.Value);
        }
        if (request.AddressId.HasValue)
        {
            query = query.Where(a => a.AddressId == request.AddressId.Value);
        }
        if (request.ProductVariantId.HasValue)
        {
            query = query.Where(a => a.ProductVariantId == request.ProductVariantId.Value);
        }
        if (request.Status.HasValue)
        {
            query = query.Where(a => a.IsActive == request.Status.Value);
        }
        if (request.MaximumAvailableQuantity.HasValue)
        {
            query = query.Where(a => a.AvailableQuantity <= request.MaximumAvailableQuantity.Value);
        }
        if (request.MinimumAvailableQuantity.HasValue)
        {
            query = query.Where(a => a.AvailableQuantity >= request.MinimumAvailableQuantity.Value);
        }
        if (request.MaximumReservedQuantity.HasValue)
        {
            query = query.Where(a => a.ReservedQuantity <= request.MaximumReservedQuantity.Value);
        }
        if (request.MinimumReservedQuantity.HasValue)
        {
            query = query.Where(a => a.ReservedQuantity <= request.MinimumReservedQuantity.Value);
        }
        var totalQuantity = await query.CountAsync();
        var items = await query.OrderByDescending(a=>a.UpdatedAt).Skip((request.PageNumber - 1)*request.PageSize).Take(request.PageSize).ToListAsync();
        return (items,totalQuantity);
    }

    public async Task<Inventory?> GetInventoryById(int inventoryid)
    {
        return await BaseQuery().Where(i=>i.InventoryId == inventoryid).FirstOrDefaultAsync();
    }

    public async Task<Inventory?> GetInventoryByVendorId(int vendorId,int inventoryid)
    {
        return await BaseQuery().Where(i=>i.ProductVariant!.Product!.VendorId == vendorId && i.InventoryId == inventoryid).FirstOrDefaultAsync();
    }
}