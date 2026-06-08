namespace Ecommerce.Models;
public class ResponseGetAddressDTO
{
    public int AddressId {get;set;}
    public string ContactName {get;set;} = string.Empty; // for deliver 
    public string ContactPhoneNumber {get;set;} = string.Empty; // for delivery
    public string AddressLine {get;set;} = string.Empty;
    public string LandMark {get;set;} = string.Empty;
    public string City {get;set;} = string.Empty;
    public string State {get;set;} = string.Empty;
    public string Country {get;set;} = string.Empty;
    public string PinCode {get;set;} = string.Empty;
    public bool IsDefault {get;set;}
    public bool IsActive {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime? UpdatedAt {get;set;}
}