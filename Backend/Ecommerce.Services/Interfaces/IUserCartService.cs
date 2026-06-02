public interface IUserCartService
{
    public Task<ResponseAddCartItemsDTO> AddCart(RequestAddCartItemsDTO requestAddCartItemsDTO, int userId);
}