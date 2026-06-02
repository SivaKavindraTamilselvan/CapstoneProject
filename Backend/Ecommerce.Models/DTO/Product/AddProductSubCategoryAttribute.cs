namespace Ecommerce.DTOs;

public class RequestAddProductSubCategoryAttributeDTO
{
    public int ProductSubCategoryId { get; set; }
    public int AttributeMasterId { get; set; }

}
public class ResponseAddProductSubCategoryAttributeDTO
{
    public int ProductSubCategoryAttributeId { get; set; }
    public int ProductSubCategoryId { get; set; }
    public int AttributeMasterId { get; set; }
}
