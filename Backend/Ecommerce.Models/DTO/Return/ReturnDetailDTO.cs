namespace Ecommerce.DTOs.Returns;
public class ReturnDetailsDto
{
    public ReturnInformationDto Return { get; set; } = new();
    public CustomerInformationDto Customer { get; set; } = new();
    public OrderInformationDto Order { get; set; } = new();
    public AddressInformationDto Address { get; set; } = new();
    public ProductInformationDto Product { get; set; } = new();
}
public class ReturnInformationDto
{
    public int ReturnId { get; set; }
    public string ReturnStatus { get; set; } = string.Empty;
    public string ReturnReason { get; set; } = string.Empty;
    public string? AdditionalReason { get; set; }
    public int ReturnQuantity { get; set; }
    public DateTime RequestedDate { get; set; }
    public decimal? DamageCost { get; set; }
    public string? VendorReview { get; set; }
    public string? ReviewRemarks { get; set; }
    public int? ReviewedByVendorId { get; set; }
    public string? ReviewedByVendorName { get; set; }
    public DateTime? ReviewedDate { get; set; }
}


public class CustomerInformationDto
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class OrderInformationDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public decimal TotalProductAmount { get; set; }
    public decimal TotalShippingAmount { get; set; }
    public decimal TotalCouponAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public int OrderedQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}


public class AddressInformationDto
{
    public int AddressId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}


public class ProductInformationDto
{
    public int ProductId { get; set; }
    public int ProductVariantId { get; set; }
    public int OrderItemId { get; set; }
    public int InventoryId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsReturnAllowed { get; set; }
    public bool IsExchangeAllowed { get; set; }
    public List<ProductAttributeDto> Attributes { get; set; } = [];
}


public class ProductAttributeDto
{
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeValue { get; set; } = string.Empty;
}