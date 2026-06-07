using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class UserProductService : IUserProductService
{
    public async Task<List<ResponseUserGetAllProductDTO>> GetAllAvailableProducts()
    {
        var products = await _productRepository.GetAllAvailableProducts();
        return _mapper.Map<List<ResponseUserGetAllProductDTO>>(products);
    }

    public async Task<List<ResponseUserGetAllProductDTO>> GetAllAvailableProductsBySubCategoryId(int subCategoryId)
    {
        var products = await _productRepository.GetAllAvailableProductsBySubCategoryId(subCategoryId);
        return _mapper.Map<List<ResponseUserGetAllProductDTO>>(products);
    }

    public async Task<List<ResponseUserGetAllProductDTO>> GetAllAvailableProductsByCategoryId(int categoryId)
    {
        var products = await _productRepository.GetAllAvailableProductsByCategoryId(categoryId);
        return _mapper.Map<List<ResponseUserGetAllProductDTO>>(products);
    }
    public async Task<List<ResponseUserGetAllProductDTO>> SearchProductsByName(string searchTerm)
    {
        var products = await _productRepository.SearchProductsByName(searchTerm);
        return _mapper.Map<List<ResponseUserGetAllProductDTO>>(products);
    }

    public async Task<ResponseUserGetProductDetailDTO> GetProductWithFullDetails(int productId)
    {
        var product = await _productRepository.GetProductWithFullDetails(productId);
        if (product == null)
        {
            throw new DataNotFoundException("Product not found");
        }
        return _mapper.Map<ResponseUserGetProductDetailDTO>(product);
    }
}