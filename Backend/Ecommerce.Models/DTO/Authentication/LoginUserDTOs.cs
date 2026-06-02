using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestLoginUserDTO
{
    [Required(ErrorMessage = "Email Cannot Be Empty")]
    [EmailAddress(ErrorMessage = "You Have Enteres The Wrong Email Format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password Cannot Be Empty")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,20}$", ErrorMessage = "Enter Strong Password")]
    public string Password { get; set; } = string.Empty;
}

public class ResponseLoginUserDTO
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int RoleId { get; set; }
    public string Token { get; set; } = string.Empty;
}