using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestUpdateProductVariantDTO
{
    public int ProductVariantId {get;set;}
    public int ProductStatusId {get;set;}
}

public class RequestUpdateProductVariant
{
    public int ProductVariantId {get;set;}
    public int ProductVariantStatusId {get;set;}
     public decimal Price { get; set; }
    public decimal WeightInKgs { get; set; }
    public decimal LengthInCm { get; set; }
    public decimal WidthInCm { get; set; }
    public decimal HeightInCm { get; set; }
    public int MinimuQuantityPerUser { get; set; }
    public bool IsReturn { get; set; } = true;
    public bool IsExchange { get; set; } = true;
}

public class ResponseUpdateProductVariantDTO
{
    public int ProductVariantId {get;set;}
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal WeightInKgs { get; set; }
    public decimal LengthInCm { get; set; }
    public decimal WidthInCm { get; set; }
    public decimal HeightInCm { get; set; }
    public DateTime UpdatedAt {get;set;}
    public DateTime CreatedAt { get; set; }


}