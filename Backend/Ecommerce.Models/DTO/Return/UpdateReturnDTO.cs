namespace Ecommerce.DTOs;

public class RequestUpdateReturnDTO
{
    public int ReturnId { get; set; }
    public int ReturnStatusId { get; set; }

}
public class ResponseUpdateReturnDTO
{
    public int ReturnId { get; set; }
    public int ReturnStatusId { get; set; }
    public DateTime? ReviewedDate { get; set; }
}