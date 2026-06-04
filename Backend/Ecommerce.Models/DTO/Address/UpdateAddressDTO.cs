namespace Ecommerce.DTOs;

public class RequestMakeDefaultAddressDTO
{
    public int AddressId {get;set;}
}
public class ResponseMakeDefaultAddressDTO
{
     public int AddressId {get;set;}
    public bool IsDefault {get;set;} = false;
    public DateTime? UpdatedAt {get;set;}
}