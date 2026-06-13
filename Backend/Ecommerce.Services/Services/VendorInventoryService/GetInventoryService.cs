using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Org.BouncyCastle.Ocsp;

public partial class InventoryService : IInventoryService
{
    
    public async Task<PagedResponse<ResponseVendorInventoryDTO>> GetInventory(RequestVendorInventoryFilter request,int vendorUserId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var result = await _inventoryRepsository.GetInventoryForVendor(request,vendor.VendorId);
        return new PagedResponse<ResponseVendorInventoryDTO>
        {
            Items = _mapper.Map<List<ResponseVendorInventoryDTO>>(result.items),
            TotalCount = result.totalQuantity,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<ResponseVendorInventoryDTO> GetInventoryById(int inventoryid,int vendorUserId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var inventory = await _inventoryRepsository.GetInventoryByVendorId(vendor.VendorId,inventoryid);
        if(inventory == null)
        {
            throw new DataNotFoundException("You Cannot access other vendor inventory");
        }
        var result = await _inventoryRepsository.GetInventoryById(inventoryid);
        return _mapper.Map<ResponseVendorInventoryDTO>(result);
    }
}