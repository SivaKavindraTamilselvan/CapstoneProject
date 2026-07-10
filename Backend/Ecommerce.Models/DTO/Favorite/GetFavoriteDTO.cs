public class ResponseGetFavoriteDTO
{
    public int FavoriteItemsId { get; set; }
    public int FavoriteId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProductVariantId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
}