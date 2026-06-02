namespace Ecommerce.Services.Interfaces;
public interface IUserCartService
{
    public Task<ResponseCartItemsDTO> AddCart(RequestAddCartItemsDTO requestAddCartItemsDTO, int userId);
}