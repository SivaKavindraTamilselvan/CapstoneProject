namespace Ecommerce.Models;

public class ApprovalHistory
{
    public int ApprovalHistoryId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public int PreviousStatusId { get; set; }
    public ApprovalStatus? PreviousStatus {get;set;}
    public int NewStatusId { get; set; }
    public ApprovalStatus? NewStatus {get;set;}
    public int ReviewedByAdminId { get; set; }
    public AdminUser? ReviewedByAdmin {get;set;}
    public string? Remarks { get; set; }

    public DateTime ReviewedAt { get; set; } = DateTime.Now;
}