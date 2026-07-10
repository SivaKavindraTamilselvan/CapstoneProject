namespace Ecommerce.Models;

public class Reviews
{
    public int ReviewId {get;set;}
    public int OrderDetailsId {get;set;}
    public OrderItems? OrderItems {get;set;}
    public int ReviewDescriptionId {get;set;}
    public ReviewDescription? ReviewDescription {get;set;}
    public string? AdditionalReviewDescription {get;set;}
    public int StarId {get;set;}
    public Star? Star {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
}