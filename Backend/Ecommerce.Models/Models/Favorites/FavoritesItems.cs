namespace Ecommerce.Models;

public class FavoritesItems
{
    public int FavoritesItemsId {get;set;}
    public int FavoritesId {get;set;}
    public Favorites? Favorites {get;set;}
    public int ProductVariantId {get;set;}
    public ProductVariant? ProductVariant {get;set;}
}