using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductService : IAdminProductService
{
    
    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllProducts()
    {
        var products = await _productRepsository.GetAll();
        return _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
    }
    public async Task<List<ResponseAdminGetStockProductDTO>> GetAllOutOfStockProducts()
    {
        var products = await _productRepsository.GetAllOutOfStockProducts();
        return _mapper.Map<List<ResponseAdminGetStockProductDTO>>(products);
    }
    public async Task<List<ResponseAdminGetStockProductDTO>> GetAllLowStockProducts(int threshold = 5)
    {
        var products = await _productRepsository.GetAllLowStockProducts(threshold);
        return _mapper.Map<List<ResponseAdminGetStockProductDTO>>(products);
    }

    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllProductsWithPendingVariants()
    {
        var products = await _productRepsository.GetAllProductsWithPendingVariants();
        return _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
    }

    public async Task<ResponseAdminGetAllProductDTO> GetProductWithFullDetails(int productId)
    {
        var product = await _productRepsository.GetProductWithFullDetails(productId);
        if (product == null)
        {
            throw new DataNotFoundException("Product not found");
        }
        return _mapper.Map<ResponseAdminGetAllProductDTO>(product);
    }
}