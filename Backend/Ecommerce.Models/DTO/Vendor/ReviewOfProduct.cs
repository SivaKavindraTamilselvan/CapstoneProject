using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestReviewOfProductDTO
{
    public int ProductId {get;set;}
    [Required(ErrorMessage = "Approval Status Cannot Be Empty")]
    [RegularExpression(@"^[1-5]+$",ErrorMessage = "Only can enter the registered Approval Status Id")]
    public int ApprovalStatusId { get; set; }
}

public class ResponseReviewOfProductDTO
{
    public int ProductId { get; set; }
    public int ApprovalStatusId { get; set; }
    public int? ReviewedByAdminId { get; set; }
    public int ProductStatusId {get;set;}
    public DateTime? ApprovedAt {get;set;}
}