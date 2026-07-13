namespace Ecommerce.Models;

public class ApprovalHistory
{
    public int ApprovalHistoryId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public int PreviousStatusId { get; set; }
    public ProductApprovalStatus? PreviousStatus { get; set; }
    public int NewStatusId { get; set; }
    public ProductApprovalStatus? NewStatus { get; set; }
    public string ReviewerType { get; set; } = string.Empty;
    public int ReviewerId { get; set; }
    public string? Remarks { get; set; }
    public DateTime ReviewedAt { get; set; } = DateTime.Now;
}