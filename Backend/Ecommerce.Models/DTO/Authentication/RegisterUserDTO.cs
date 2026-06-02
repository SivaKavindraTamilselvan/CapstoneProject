using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestRegisterUserDTO
{
    [Required(ErrorMessage = "First name Cannot Be Empty")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name can contain only letters.")]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Last name Cannot Be Empty")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last name can contain only letters.")]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email Cannot Be Empty")]
    [EmailAddress(ErrorMessage = "You Have Entered The Wrong Email Format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone Number Cannot Be Empty")]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required(ErrorMessage ="Password Cannot Be Empty")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,20}$",ErrorMessage = "Enter Strong Password")]
    public string Password { get; set; } = string.Empty;
}

public class ResponseRegisterUserDTO
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}