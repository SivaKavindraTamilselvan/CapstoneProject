using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;
public interface IUserProductService
{
    public Task<PagedResponse<ResponseUserGetProductDetailDTO>> GetUserProducts(RequestUserProductFilter request);
    Task<ResponseUserGetProductDetailDTO> GetProductWithFullDetails(int productId);
}
