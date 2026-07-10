namespace Ecommerce.DTOs;
public class ResponseProductVariantAttribute
{
    public int ProductVariantAttributeId { get; set; }
    public string AttributeName {get;set;} = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
public class ResponseProductVariantAttributeDTO
{
    public int ProductVariantAttributeId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
}
