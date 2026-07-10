public class ResponseGetCartDTO
{
    public int CartItemsId { get; set; }
    public int CartId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProductVariantId { get; set; }
    public int Qunatity { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }

}