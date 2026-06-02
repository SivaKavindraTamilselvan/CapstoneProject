using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestReviewOfVendorDTO
{
    public int VendorId { get; set; }
    [Required(ErrorMessage = "Approval Status Cannot Be Empty")]
    [RegularExpression(@"^[1-5]+$",ErrorMessage = "Only can enter the registered Approval Status Id")]
    public int ApprovalStatusId { get; set; }
}

public class ResponseReviewOfVendorDTO
{
    public int VendorId { get; set; }
    public int ApprovalStatusId { get; set; }
    public int? ReviewedByAdminId { get; set; }
    public DateTime? ReviewedAt { get; set; }
}