namespace Ecommerce.Models;

public class ProductApprovalStatus
{
    public int ProductApprovalStatusId { get; set; }
    public string ProductApprovalStatusName { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

}