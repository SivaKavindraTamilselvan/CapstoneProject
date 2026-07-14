namespace Ecommerce.Models;

public class ApprovalStatus
{
    public int ApprovalStatusId { get; set; }
    public string ApprovalStatusName { get; set; } = string.Empty;
    public ICollection<Vendor> Vendors { get; set; } = new List<Vendor>();
    public ICollection<VendorApprovalHistory> PreviousApprovalHistories { get; set; } = new List<VendorApprovalHistory>();
    public ICollection<VendorApprovalHistory> NewApprovalHistories { get; set; } = new List<VendorApprovalHistory>();
}