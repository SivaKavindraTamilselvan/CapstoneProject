using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestUpdateProduct
{
    public int ProductId {get;set;}
    public int ProductStatusId { get; set; }
}
public class ResponseUpdateProduct
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProductSubCategoryId { get; set; }
    public int ProductStatusId { get; set; }
    public DateTime UpdatedAt {get;set;}
    public DateTime CreatedAt { get; set; }
}
