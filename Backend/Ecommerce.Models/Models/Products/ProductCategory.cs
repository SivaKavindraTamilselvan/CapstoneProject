namespace Ecommerce.Models;

public class ProductCategory
{
    public int ProductCategoryId { get; set; }
    public string ProductCategoryName { get; set; } = string.Empty;
    public ICollection<ProductSubCategory> ProductSubCategories { get; set; } = new List<ProductSubCategory>();
}