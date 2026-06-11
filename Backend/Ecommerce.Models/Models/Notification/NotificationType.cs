public class NotificationType
{
    public int NotificationTypeId { get; set; }

    public string TypeName { get; set; } = string.Empty;
    // OrderPlaced, OrderShipped, VendorApproved, ProductRejected

    public bool IsActive { get; set; } = true;

    public ICollection<Notification>? Notifications { get; set; }
}