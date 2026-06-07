using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestReviewOfProductDTO
{
    [Required(ErrorMessage = "Product Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Id Should Be Greater Than 0")]
    public int ProductId {get;set;}

    [Required(ErrorMessage = "Approval Status Cannot Be Empty")]
    [RegularExpression(@"^[1-5]+$",ErrorMessage = "Only can enter the registered Approval Status Id")] // here used this alone for learning that regulr expression can be used
    public int ApprovalStatusId { get; set; }

    public string Remarks {get;set;} = string.Empty;
}

public class ResponseReviewOfProductDTO
{
    public int ProductId { get; set; }
    public int ProductApprovalStatusId { get; set; }
    public int ProductStatusId {get;set;}
    public DateTime? UpdatedAt {get;set;}
}