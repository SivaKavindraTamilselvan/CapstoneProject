using System.ComponentModel.DataAnnotations;

public class RequestChangePasswordDTO
{

    [Required(ErrorMessage = "Password Cannot Be Empty")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,20}$", ErrorMessage = "Enter Strong Password")]
    public string CurrentPassword { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password Cannot Be Empty")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,20}$", ErrorMessage = "Enter Strong Password")]
    public string NewPassword { get; set; } = string.Empty;
}
