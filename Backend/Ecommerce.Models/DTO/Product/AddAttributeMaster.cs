namespace Ecommerce.DTOs;

public class RequestAddAttributeDTO
{
    public string AttributeName { get; set; } = string.Empty;
}


public class ResponseAddAttributeDTO
{
    public int AttributeMasterId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
}
