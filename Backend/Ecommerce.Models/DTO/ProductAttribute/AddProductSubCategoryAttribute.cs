using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestAddProductSubCategoryAttributeDTO
{
    [Required(ErrorMessage = "Product Sub Category Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Product Sub Category Id Should Be Greater Than 0")]
    public int ProductSubCategoryId { get; set; }

    [Required(ErrorMessage = "Attribute Id Needed")]
    [Range(1, int.MaxValue, ErrorMessage = "Attribute Id Should Be Greater Than 0")]
    public int AttributeMasterId { get; set; }

}
public class ResponseAddProductSubCategoryAttributeDTO
{
    public int ProductSubCategoryAttributeId { get; set; }
    public int ProductSubCategoryId { get; set; }
    public int AttributeMasterId { get; set; }
}
