namespace Ecommerce.Models;
public class ProductStatus
{
    public int ProductStatusId {get;set;}
    public string ProductStatusName {get;set;} = string.Empty; // draft,active,temporarily not available,archived
    public ICollection<Product> Products {get;set;} = new List<Product>();
}