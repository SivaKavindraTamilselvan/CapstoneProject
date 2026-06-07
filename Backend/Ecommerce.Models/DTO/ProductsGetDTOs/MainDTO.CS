// Shared across all roles
public class ResponseProductImageDTO
{
    public int ProductImageId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMainImage { get; set; }
    public string DisplayOrderName { get; set; } = string.Empty;
}

public class ResponseProductVariantAttributeDTO
{
    public int ProductVariantAttributeId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
}

public class ResponseInventoryDTO
{
    public int InventoryId { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
}