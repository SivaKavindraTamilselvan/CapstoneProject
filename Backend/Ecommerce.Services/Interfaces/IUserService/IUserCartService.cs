namespace Ecommerce.Services.Interfaces;

public interface IUserCartService
{
    public Task<ResponseCartItemsDTO> AddCart(RequestAddCartItemsDTO requestAddCartItemsDTO, int userId);
    public Task<List<ResponseCartItemsDTO>> DeleteAllCart(int userId);
    public Task<ResponseCartItemsDTO> DeleteCart(RequestDeleteCartItemsDTO requestDeleteCartItemsDTO);
    public Task<ResponseCartItemsDTO> UpdateCart(RequestUpdateCartItemsDTO requestUpdateCartItemsDTO);

}