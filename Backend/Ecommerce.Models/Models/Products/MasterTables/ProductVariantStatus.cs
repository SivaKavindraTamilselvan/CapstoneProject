namespace Ecommerce.Models;
public class ProductVariantStatus
{
    public int ProductVariantStatusId {get;set;}
    public string ProductVariantStatusName {get;set;} = string.Empty; // draft,active,temporarily not available,archived
    public ICollection<Product> Products {get;set;} = new List<Product>();
}