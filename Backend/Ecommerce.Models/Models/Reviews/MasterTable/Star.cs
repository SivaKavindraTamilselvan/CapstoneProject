namespace Ecommerce.Models;
public class Star
{
    public int StarId {get;set;}
    public string StarName {get;set;} = string.Empty;
    public string StarDescription {get;set;} = string.Empty;
    public ICollection<Reviews> Reviews {get;set;} = new List<Reviews>();
}