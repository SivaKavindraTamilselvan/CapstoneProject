namespace Ecommerce.DTOs;

public class RequestReviewReturnDTO
{
    public int ReturnId { get; set; }
    public bool Review {get;set;}

}

public class RequestReviewReturnProductDTO
{
    public int ReturnId { get; set; }
    public decimal DamageCost {get;set;}
    public string Remarks {get;set;} = string.Empty;

}
public class ResponseReviewReturnDTO
{
    public int ReturnId { get; set; }
    public int ReturnStatusId { get; set; }
    public DateTime? ReviewedDate { get; set; }
}