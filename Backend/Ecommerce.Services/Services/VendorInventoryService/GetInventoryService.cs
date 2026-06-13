using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Ocsp;

public partial class InventoryService : IInventoryService
{

    public async Task<PagedResponse<ResponseVendorInventoryDTO>> GetInventory(RequestVendorInventoryFilter request, int vendorUserId)
    {
        _logger.LogInformation(
        "Vendor UserId {VendorUserId} requested inventory list with filter {@Filter}",
        vendorUserId,
        request);

        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var result = await _inventoryRepsository.GetInventoryForVendor(request, vendor.VendorId);
        _logger.LogInformation(
       "Returning {InventoryCount} inventory records for VendorId {VendorId}",
       result.items.Count,
       vendor.VendorId);

        return new PagedResponse<ResponseVendorInventoryDTO>
        {
            Items = _mapper.Map<List<ResponseVendorInventoryDTO>>(result.items),
            TotalCount = result.totalQuantity,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<ResponseVendorInventoryDTO> GetInventoryById(int inventoryid, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested InventoryId {InventoryId}", vendorUserId, inventoryid);
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var inventory = await _inventoryRepsository.GetInventoryByVendorId(vendor.VendorId, inventoryid);
        if (inventory == null)
        {
            _logger.LogWarning("Vendor UserId {VendorUserId} attempted to access InventoryId {InventoryId} not belonging to VendorId {VendorId}", vendorUserId, inventoryid, vendor.VendorId);
            throw new DataNotFoundException("You Cannot access other vendor inventory");
        }
        var result = await _inventoryRepsository.GetInventoryById(inventoryid);
        _logger.LogInformation("InventoryId {InventoryId} returned successfully for Vendor UserId {VendorUserId}", inventoryid, vendorUserId);

        return _mapper.Map<ResponseVendorInventoryDTO>(result);
    }
}