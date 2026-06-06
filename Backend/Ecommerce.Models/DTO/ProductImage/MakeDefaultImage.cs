namespace Ecommerce.DTOs;

public class RequestMakeDefaultImageDTO
{
    public int ProductImageId {get;set;}
}
public class ResponseMakeDefaultImageDTO
{
    public int ProductImageId {get;set;}
    public bool IsDefault {get;set;} = false;
    public DateTime? UpdatedAt {get;set;}
}