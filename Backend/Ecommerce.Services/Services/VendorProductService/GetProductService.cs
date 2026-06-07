using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    public async Task<List<ResponseVendorGetAllProductDTO>> GetAllProductsByVendorId(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var products = await _productRepsository.GetAllProductsByVendorId(vendor.VendorId);
        return _mapper.Map<List<ResponseVendorGetAllProductDTO>>(products);
    }

    public async Task<List<ResponseVendorGetAllProductDTO>> GetAllAvailableProductsByVendorId(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var products = await _productRepsository.GetAllAvailableProductsByVendorId(vendor.VendorId);
        return _mapper.Map<List<ResponseVendorGetAllProductDTO>>(products);
    }
    public async Task<List<ResponseVendorGetDraftProductDTO>> GetAllDraftProducts(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var products = await _productRepsository.GetAllDraftProducts();
        var vendorProducts = products.Where(p => p.VendorId == vendor.VendorId).ToList();
        return _mapper.Map<List<ResponseVendorGetDraftProductDTO>>(vendorProducts);
    }

    public async Task<List<ResponseVendorGetStockProductDTO>> GetAllLowStockProducts(int vendorId, int threshold = 5)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var products = await _productRepsository.GetAllLowStockProducts(threshold);
        var vendorProducts = products.Where(p => p.VendorId == vendor.VendorId).ToList();
        return _mapper.Map<List<ResponseVendorGetStockProductDTO>>(vendorProducts);
    }
    public async Task<List<ResponseVendorGetStockProductDTO>> GetAllOutOfStockProducts(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var products = await _productRepsository.GetAllOutOfStockProducts();
        var vendorProducts = products.Where(p => p.VendorId == vendor.VendorId).ToList();
        return _mapper.Map<List<ResponseVendorGetStockProductDTO>>(vendorProducts);
    }

    public async Task<List<ResponseVendorGetAllProductDTO>> GetAllProductsWithPendingVariants(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var products = await _productRepsository.GetAllProductsWithPendingVariants();
        var vendorProducts = products.Where(p => p.VendorId == vendor.VendorId).ToList();
        return _mapper.Map<List<ResponseVendorGetAllProductDTO>>(vendorProducts);
    }

}