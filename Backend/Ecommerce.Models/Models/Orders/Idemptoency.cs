namespace Ecommerce.Models
{
    public class IdempotencyKey
    {
        public int IdempotencyKeyId { get; set; }
        public string Key { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string RequestHash { get; set; } = string.Empty;
        public int? ResponseStatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
    }
}