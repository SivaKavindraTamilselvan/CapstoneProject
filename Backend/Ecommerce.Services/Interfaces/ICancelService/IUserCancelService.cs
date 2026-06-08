namespace Ecommerce.Services.Interfaces;
public interface IUserCancelService
{
    public Task<ResponseCancelDTO> RequestCancel(RequestCancelDTO requestCancelDTO);
}