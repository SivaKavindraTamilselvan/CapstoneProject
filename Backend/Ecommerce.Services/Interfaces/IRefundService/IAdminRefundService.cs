namespace Ecommerce.DTOs;

public interface IAdminRefundService
{
    public Task<ResponseAddRefundDTO> CreateReturnRefund(RequestAddReturnRefundDTO requestAddReturnRefundDTO);
    public Task<ResponseAddRefundDTO> CreateRefund(RequestAddRefundDTO requestAddRefundDTO);
    public Task<ResponseUpdateRefundDTO> ReviewRefund(RequestUpdateRefundDTO requestUpdateRefundDTO);
}