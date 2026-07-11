namespace Ecommerce.DTOs;

public class ResponseAdminGetVendorDTO
{
    public int VendorId { get; set; }
    public string ContactPersonName { get; set; } = string.Empty;
    public string CompanyEmail { get; set; } = string.Empty;
    public string CompanyPhoneNumber { get; set; } = string.Empty;
    public string VendorCompanyName { get; set; } = string.Empty;
    public string GSTNumber { get; set; } = string.Empty;
    public int ApprovalStatusId { get; set; }
    public string ApprovalStatusName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int? ReviewedByAdminId { get; set; }
    public string ReviewAdminName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class RequestAdminVendorFilter : PaginationFilter
{
    public string? ContactPersonName { get; set; }
    public string? CompanyEmail { get; set; }
    public string? CompanyPhoneNumber { get; set; }
    public string? VendorCompanyName { get; set; }
    public string? GSTNumber { get; set; }
    public int? ApprovalStatusId { get; set; }
    public bool? IsActive { get; set; }
    public bool? includeIsDeleted { get; set; }
    public int? ReviewedByAdminId { get; set; }

}

public class RequestAdminVendorUserFilter : PaginationFilter
{
    public int? VendorId { get; set; }
    public bool? IsActive { get; set; }
    public int? VendorRoleId { get; set; }

}