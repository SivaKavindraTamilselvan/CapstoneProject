namespace Ecommerce.DTOs;

public class ResponseMakeDefaultAddressDTO
{
    public int AddressId { get; set; }
    public bool IsDefault { get; set; } = false;
    public DateTime? UpdatedAt { get; set; }
}