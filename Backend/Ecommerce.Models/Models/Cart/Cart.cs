namespace Ecommerce.Models;

public class Cart
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public User? Users { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool NotifiedAbandoned { get; set; }
    public ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();
}