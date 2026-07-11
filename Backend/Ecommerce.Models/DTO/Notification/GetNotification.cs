
using Ecommerce.DTOs;

public class ResponseNotification
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int NotificationTypeId { get; set; } 
    public string NotificationTypeName { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ReadAt { get; set; }
}

public class NotificationFilter : PaginationFilter
{
    public int? NotificationTypeId { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? MinCreatedAt { get; set; }
    public DateTime? MaxCreatedAt { get; set; }
}