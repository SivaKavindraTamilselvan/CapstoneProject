namespace Ecommerce.Models;
public class AttributeMaster
{
    public int AttributeMasterId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public ICollection<ProductVariantAttribute> ProductVariantAttributes {get;set;} = new List<ProductVariantAttribute>();
}