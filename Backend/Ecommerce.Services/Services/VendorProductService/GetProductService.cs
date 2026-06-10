using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    public async Task<List<ResponseVendorGetAllProductDTO>> GetAllProductsByVendorId(int? approval, int? status, int vendorId, int? subcategory, int pageNumber, int pageSize, bool? hasIssues, bool? isAvailableForSale)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var products = await _productRepsository.GetVendorProduct(approval, status, vendor.VendorId, subcategory, pageNumber, pageSize);
        var response = _mapper.Map<List<ResponseVendorGetAllProductDTO>>(products);
        for (int i = 0; i < products.Count; i++)
        {
            var validation = await _productValidation.ValidateProductChain(products[i]);
            response[i].IsAvailableForSale = products[i].ProductApprovalStatusId == 4 && products[i].ProductStatusId == 2 && validation.IsValid && products[i]
            .ProductVariants.Any(pv => pv.ProductApprovalStatusId == 4 && pv.ProductVariantStatusId == 2 && pv.Inventories.Any(inv => inv.AvailableQuantity > 0));
            response[i].ValidationIssues = validation.Issues;
        }
        if (hasIssues.HasValue)
        {
            if (hasIssues.Value)
            {
                response = response.Where(p => p.ValidationIssues.Any()).ToList();
            }
            else
            {
                response = response.Where(p => !p.ValidationIssues.Any()).ToList();
            }
        }
        if (isAvailableForSale.HasValue)
        {
            response = response.Where(p => p.IsAvailableForSale == isAvailableForSale.Value).ToList();
        }
        return response;
    }

    public async Task<List<ResponseVendorGetAllProductDTO>> GetAllAvailableProductsByVendorId(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var products = await _productRepsository.GetAllAvailableProductsByVendorId(vendor.VendorId);
        return _mapper.Map<List<ResponseVendorGetAllProductDTO>>(products);
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
     public async Task<ResponseAdminGetAllProductDTO> GetProductWithFullDetails(int productId)
    {
        var product = await _productRepsository.GetProductWithFullDetails(productId);
        if (product == null)
        {
            throw new DataNotFoundException("Product not found");
        }
        return _mapper.Map<ResponseAdminGetAllProductDTO>(product);
    }
     public async Task<List<ResponseGetAllProductVariant>> GetAllProductVariant(ProductVariantFilterDto filter,int vendorUserId)
    {
        var product = await _productVariantRepsository.GetAllVariantsForVendor(vendorUserId,filter);
        return _mapper.Map<List<ResponseGetAllProductVariant>>(product);
    }
}