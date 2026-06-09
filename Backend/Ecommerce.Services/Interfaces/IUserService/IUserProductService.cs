using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;
public interface IUserProductService
{
    Task<List<ResponseUserGetAllProductDTO>> GetUserProducts(int? categoryId, int? subcategoryId, string? searchTerm,int pageNumber,int pageSize);
    Task<List<ResponseUserGetAllProductDTO>> GetAllAvailableProductsBySubCategoryId(int subCategoryId);
    Task<List<ResponseUserGetAllProductDTO>> GetAllAvailableProductsByCategoryId(int categoryId);
    Task<List<ResponseUserGetAllProductDTO>> SearchProductsByName(string searchTerm);
    Task<ResponseUserGetProductDetailDTO> GetProductWithFullDetails(int productId);
}
