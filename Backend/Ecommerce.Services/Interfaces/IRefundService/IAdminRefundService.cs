namespace Ecommerce.DTOs;
public interface IAdminRefundService
{
    public Task<ResponseAddRefundDTO> CreateRefund(RequestAddRefundDTO requestAddRefundDTO);
    public Task<ResponseUpdateRefundDTO> ReviewRefund(RequestUpdateRefundDTO requestUpdateRefundDTO);
}