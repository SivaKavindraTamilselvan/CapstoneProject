namespace Ecommerce.Models;

public class CartItems
{
    public int CartItemsId {get;set;}
    public int CartId {get;set;}
    public Cart? Cart {get;set;}
    public int ProductVariantId {get;set;}
    public ProductVariant? ProductVariant {get;set;}
    public int Qunatity {get;set;} = 1;
   
}