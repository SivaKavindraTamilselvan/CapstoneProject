public class RequestAddCartItemsDTO
{
    public int ProductVariantId {get;set;}
}
public class ResponseAddCartItemsDTO
{
    public int CartItemsId {get;set;}
    public int CartId {get;set;}
    public int ProductVariantId {get;set;}
    public int Qunatity {get;set;}
}

