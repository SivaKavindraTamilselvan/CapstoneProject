namespace Ecommerce.DTOs;

public class RequestAddAddressDTO
{
    public int UserId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhoneNumber { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string LandMark { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
}

public class ResponseAddAddressDTO
{
    public int AddressId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}