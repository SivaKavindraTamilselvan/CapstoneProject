namespace Ecommerce.Models;

public class ProductSubCategoryAttribute
{
    public int ProductSubCategoryAttributeId { get; set; }
    public int ProductSubCategoryId { get; set; }
    public ProductSubCategory? ProductSubCategory { get; set; }
    public int AttributeMasterId { get; set; }
    public AttributeMaster? AttributeMaster { get; set; }
}