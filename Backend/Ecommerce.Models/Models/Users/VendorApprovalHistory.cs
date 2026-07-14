namespace Ecommerce.Models;

public class VendorApprovalHistory
{
    public int ApprovalHistoryId { get; set; }
    public int EntityId { get; set; }
    public int PreviousStatusId { get; set; }
    public ApprovalStatus? PreviousStatus { get; set; }
    public int NewStatusId { get; set; }
    public ApprovalStatus? NewStatus { get; set; }
    public int ReviewerId { get; set; }
    public AdminUser? AdminUser { get; set; }
    public string? Remarks { get; set; }
    public DateTime ReviewedAt { get; set; } = DateTime.Now;
}