using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestRegisterAdminDTO
{
    public RequestRegisterUserDTO requestRegisterUserDTO { get; set; } = new();

    [Required(ErrorMessage = "Admin Role Cannot Be Empty")]
    [RegularExpression(@"^[2-9]+$", ErrorMessage = "Only can enter the registered Adimin Role Id")]
    public int AdminRoleId { get; set; }
}

public class ResponseRegisterAdminDTO
{
    public int AdminUserId { get; set; }
    public int UserId { get; set; }
    public int? AssignedByAdminUserId { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
}


public class RequestSetPasswordDTO
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ResponseSetPasswordDTO
{
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
