using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductService : IAdminProductService
{
    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllProducts()
    {
        var products = await _productRepsository.GetAll();
        return _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
    }

    public async Task<List<ResponseAdminGetPendingProductDTO>> GetAllPendingAdminApprovalProducts()
    {
        var products = await _productRepsository.GetAllPendingAdminApprovalProducts();
        return _mapper.Map<List<ResponseAdminGetPendingProductDTO>>(products);
    }
    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllAdminApprovedProducts()
    {
        var products = await _productRepsository.GetAllAdminApprovedProducts();
        return _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
    }

    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllAdminRejectedProducts()
    {
        var products = await _productRepsository.GetAllAdminRejectedProducts();
        return _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
    }

    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllVendorRejectedProducts()
    {
        var products = await _productRepsository.GetAllVendorRejectedProducts();
        return _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
    }
    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllDeletedByAdminProducts()
    {
        var products = await _productRepsository.GetAllDeletedByAdminProducts();
        return _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
    }

    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllTemporarilyUnavailableProducts()
    {
        var products = await _productRepsository.GetAllTemporarilyUnavailableProducts();
        return _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
    }

    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllArchivedProducts()
    {
        var products = await _productRepsository.GetAllArchivedProducts();
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