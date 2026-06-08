using Ecommerce.Models;

namespace Ecommerce.DTOs;
public class ResponseGetVendor
{
    public int VendorId { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public string CompanyPhoneNumber { get; set; } = string.Empty;
    public string VendorCompanyName { get; set; } = string.Empty;
    public string GSTNumber { get; set; } = string.Empty;
    public int ApprovalStatusId { get; set; } 
    public string ApprovalStatusName {get;set;} = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}