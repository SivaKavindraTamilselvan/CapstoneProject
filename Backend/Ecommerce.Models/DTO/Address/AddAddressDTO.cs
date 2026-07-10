using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestAddAddressDTO
{
    [Required(ErrorMessage = "Contact Person Name is Needed")]
    [MaxLength(50)]
    public string ContactName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone Number Cannot Be Empty")]
    [Phone]
    public string ContactPhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address Is Needed")]
    [MaxLength(300, ErrorMessage = "Address Line Maximum character is 300")]
    public string AddressLine { get; set; } = string.Empty;

    [Required(ErrorMessage = "Land Mark Is Needed For The Shipment Purpose")]
    [MaxLength(150, ErrorMessage = "Maximum character allowed for LandMark is 150")]
    public string LandMark { get; set; } = string.Empty;

    [Required(ErrorMessage = "City Is Needed For The Shipment Purpose")]
    [MaxLength(100, ErrorMessage = "Maximum character allowed for the City is 100")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Stats Is Needed For The Shipment Purpose")]
    [MaxLength(100, ErrorMessage = "Maximum character allowed for the State is 100")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "PinCode Is Needed For The Shipment Purpose")]
    [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "Enter a valid 6-digit Indian PIN code")]
    public string PinCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Is Default Is Needed")]
    public bool IsDefault { get; set; } = false;
}

public class ResponseAddAddressDTO
{
    public int AddressId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}