namespace Ecommerce.Models;

public class DisplayOrder
{
    public int DisplayOrderId {get;set;}
    public string DisplayOrderName {get;set;} = string.Empty;
    public ICollection<ProductImage> ProductImages {get;set;} = new List<ProductImage>();
}