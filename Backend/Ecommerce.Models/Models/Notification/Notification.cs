using Ecommerce.Models;

public class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public int NotificationTypeId { get; set; }
    public NotificationType? NotificationType { get; set; }

    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }

    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ReadAt { get; set; }
}