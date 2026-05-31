using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestRegisterVendorDTO
{
    public RequestRegisterUserDTO requestRegisterUserDTO { get; set; } = new();
    [Required(ErrorMessage = "Contact Person Name name Cannot Be Empty")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name can contain only letters.")]
    [MaxLength(50)]
    public string ContactPersonName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email Cannot Be Empty")]
    [EmailAddress(ErrorMessage = "You Have Entered The Wrong Email Format")]
    public string CompanyEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone Number Cannot Be Empty")]
    [Phone]
    public string CompanyPhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vendor Company Name Cannot Be Empty")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name can contain only letters.")]
    [MaxLength(50)]
    public string VendorCompanyName { get; set; } = string.Empty;
    
    [Required]
    [RegularExpression( @"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[A-Z0-9]{1}Z[A-Z0-9]{1}$",ErrorMessage = "GST Number For The India Is Not Valid")]
    public string GSTNumber { get; set; } = string.Empty;
}

public class ResponseRegisterVendorDTO
{
    public int VendorId { get; set; }
    public int OwnerUserId { get; set; }
    public string CompanyEmail { get; set; } = string.Empty;
    public string CompanyPhoneNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}