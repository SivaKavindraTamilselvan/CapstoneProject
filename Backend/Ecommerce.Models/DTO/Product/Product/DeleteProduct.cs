
namespace Ecommerce.DTOs;

public class RequestDeleteProduct
{
    public int ProductId {get;set;}
}

public class ResponseDeleteProduct
{
    public int ProductId {get;set;}
    public int ProductApprovalStatusId { get; set; }
    public DateTime? UpdatedAt{get;set;}

}