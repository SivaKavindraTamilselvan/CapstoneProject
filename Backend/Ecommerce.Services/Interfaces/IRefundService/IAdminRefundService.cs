namespace Ecommerce.DTOs;

public interface IAdminRefundService
{
    public Task<ResponseAddRefundDTO> CreateReturnRefund(RequestAddReturnRefundDTO requestAddReturnRefundDTO);
}