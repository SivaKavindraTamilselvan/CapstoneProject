using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTOs;

public class RequestAddAttributeDTO
{
    [Required(ErrorMessage = "Attribute Name Is Needed")]
    [MaxLength(100 ,ErrorMessage = "Maximum 100 characters allowed")]
    public string AttributeName { get; set; } = string.Empty;
}


public class ResponseAddAttributeDTO
{
    public int AttributeMasterId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
}
