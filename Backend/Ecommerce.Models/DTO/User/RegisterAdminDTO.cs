using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestRegisterAdminDTO
{
    [Required(ErrorMessage = "First name Cannot Be Empty")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name can contain only letters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name Cannot Be Empty")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last name can contain only letters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email Cannot Be Empty")]
    [EmailAddress(ErrorMessage = "You Have Enteres The Wrong Email Format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone Number Cannot Be Empty")]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Admin Role Cannot Be Empty")]
    [RegularExpression(@"^[1-5]+$",ErrorMessage = "Only can enter the registered Adimin Role Id")]
    public int AdminRoleId { get; set; }
    public int? AssignedByAdminUserId { get; set; }
}

public class ResponseRegisterAdminDTO
{
    public int AdminUserId { get; set; }
    public int UserId { get; set; }
    public int? AssignedByAdminUserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}