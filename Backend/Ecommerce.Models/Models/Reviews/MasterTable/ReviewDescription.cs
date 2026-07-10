namespace Ecommerce.Models;

public class ReviewDescription
{
    public int ReviewDescriptionId {get;set;}
    public string ReviewDescriptionName {get;set;} = string.Empty;
    public ICollection<Reviews> Reviews {get;set;} = new List<Reviews>();
}