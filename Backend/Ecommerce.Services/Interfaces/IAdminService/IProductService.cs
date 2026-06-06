using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IAdminProductService
{
    public Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO, int adminUserId);

}