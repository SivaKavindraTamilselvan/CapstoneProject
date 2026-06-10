using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminProductService : IAdminProductService
{

    public async Task<List<ResponseAdminGetAllProductDTO>> GetAllProducts(int? approval, int? status, int? vendorId, int? subcategory, bool? hasIssues, bool? isAvailableForSale, int pageNumber, int pageSize)
    {
        var products = await _productRepsository.GetAdminProduct(approval, status, vendorId, subcategory, pageNumber, pageSize);
        var response = _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
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
    public async Task<List<ResponseGetAllProductVariant>> GetAllProductVariant(ProductVariantFilterDto filter)
    {
        var product = await _productVariantRepsository.GetAllVariantsForAdmin(filter);
        return _mapper.Map<List<ResponseGetAllProductVariant>>(product);
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