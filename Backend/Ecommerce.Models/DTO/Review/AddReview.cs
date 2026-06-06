namespace Ecommerce.DTOs;
public class RequestAddReviewDTO
{
    public int OrderDetailsId {get;set;}
    public int ReviewDescriptionId {get;set;}
    public string? AdditionalReviewDescription {get;set;}
    public int StarId {get;set;}
}

public class ResponseAddReviewDTO
{
    public int ReviewId {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
}