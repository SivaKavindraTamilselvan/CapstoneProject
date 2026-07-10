namespace Ecommerce.Models;

public class ProductApprovalStatus
{
    public int ProductApprovalStatusId { get; set; }
    public string ProductApprovalStatusName { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public ICollection<ApprovalHistory> PreviousApprovalHistories { get; set; } = new List<ApprovalHistory>();
    public ICollection<ApprovalHistory> NewApprovalHistories { get; set; } = new List<ApprovalHistory>();
}