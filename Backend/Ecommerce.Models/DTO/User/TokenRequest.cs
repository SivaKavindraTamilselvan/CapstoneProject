namespace Ecommerce.DTOs;

public class TokenRequest
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? AdminRoleId {get;set;}
    public int? VendorRoleId {get;set;}
}

