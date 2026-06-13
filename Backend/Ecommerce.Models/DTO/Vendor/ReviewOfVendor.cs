using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestReviewOfVendorDTO
{
    [Required(ErrorMessage = "Vendor Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Vendor Id Should Be Greater Than 0")]
    public int VendorId { get; set; }

    [Required(ErrorMessage = "Approval Status Cannot Be Empty")]
    [RegularExpression(@"^[2-3]+$",ErrorMessage = "Only can enter the registered Approval Status Id")]
    public int ApprovalStatusId { get; set; }

    public string? Remark {get;set;}
}

public class ResponseReviewOfVendorDTO
{
    public int VendorId { get; set; }
    public int ApprovalStatusId { get; set; }
    public int? ReviewedByAdminId { get; set; }
    public DateTime? ReviewedAt { get; set; }
}