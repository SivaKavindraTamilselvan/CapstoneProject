namespace Ecommerce.DTOs;

public class RequestMakeDefaultImageDTO
{
    public int ProductImageId {get;set;}
}
public class ResponseMakeDefaultImageDTO
{
    public int ProductImageId {get;set;}
    public bool IsMainImage {get;set;}
    public DateTime? UpdatedAt {get;set;}
}