public class ResponseGetFavoriteDTO
{
    public int FavoritesItemsId { get; set; }
    public int FavoritesId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProductVariantId { get; set; }
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string mainImageUrl {get;set;} = string.Empty;
}