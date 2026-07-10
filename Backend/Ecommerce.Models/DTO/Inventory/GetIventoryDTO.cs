using Ecommerce.Models;

namespace Ecommerce.DTOs;

public class RequestVendorInventoryFilter : PaginationFilter
{
    public int? ProductVariantId { get; set; }
    public int? AddressId { get; set; }
    public int? MinimumAvailableQuantity { get; set; }
    public int? MinimumReservedQuantity { get; set; }
    public int? MaximumAvailableQuantity { get; set; }
    public int? MaximumReservedQuantity { get; set; }
    public bool? Status {get;set;}
}

public class RequestAdminInventoryFilter : PaginationFilter
{
    public int? VendorId {get;set;}
    public int? ProductVariantId { get; set; }
    public int? AddressId { get; set; }
    public int? MinimumAvailableQuantity { get; set; }
    public int? MinimumReservedQuantity { get; set; }
    public int? MaximumAvailableQuantity { get; set; }
    public int? MaximumReservedQuantity { get; set; }
    public bool? Status {get;set;}
}

public class ResponseVendorInventoryDTO
{

    public int InventoryId { get; set; }
    public int AddressId {get;set;}
    public int ProductVariantId {get;set;}
    public string? SKU {get;set;}
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public string? ContactPhoneNumber { get; set; }
    public string? AddressLine {get;set;}
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; } 
    public string? PinCode { get; set; }
    public bool? IsActive { get; set; }
}

public class ResponseAdminInventoryDTO
{
    public int InventoryId { get; set; }
    public int VendorId {get;set;}
    public string? VendorName {get;set;}
    public int AddressId {get;set;}
    public int ProductVariantId {get;set;}
    public string? SKU {get;set;}
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public string? ContactPhoneNumber { get; set; }
    public string? AddressLine {get;set;}
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; } 
    public string? PinCode { get; set; }
    public bool? IsActive { get; set; }
}