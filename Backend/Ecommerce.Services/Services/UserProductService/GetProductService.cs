using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;

public partial class UserProductService : IUserProductService
{
    public async Task<PagedResponse<ResponseUserGetProductDetailDTO>> GetUserProducts(RequestUserProductFilter request)
    {
        _logger.LogInformation("User requested product list with filters {@Request}", request);
        var result = await _productRepository.GetUserProducts(request);
        _logger.LogInformation("Repo returned {ProductCount} products (TotalCount: {TotalCount})", result.items.Count, result.totalCount);
        var response = _mapper.Map<List<ResponseUserGetProductDetailDTO>>(result.items);
        for (int i = 0; i < result.items.Count; i++)
        {
            foreach (var variantResponse in response[i].ProductVariants)
            {
                var variant = result.items[i].ProductVariants.FirstOrDefault(v => v.ProductVariantId == variantResponse.ProductVariantId);

                if (variant == null)
                    continue;

                variantResponse.isAvailableForSale =
                    variant.Inventories.Any(inv =>
                        inv != null &&
                        inv.IsActive &&
                        inv.AvailableQuantity > 0 &&
                        inv.Address != null &&
                        inv.Address.IsActive);
            }
        }

        return new PagedResponse<ResponseUserGetProductDetailDTO>
        {
            Items = response,
            TotalCount = result.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<ResponseUserGetProductDetailDTO> GetProductWithFullDetails(int productId)
    {
        _logger.LogInformation("User requested full details for ProductId {ProductId}", productId);

        var result = await _productRepository.CheckTheWholeProduct(productId, 1);
        if (result == null)
        {
            throw new DataNotFoundException("Product Not Found");
        }

        var product = await _productRepository.GetUserProductWithFullDetails(productId);
        if (product == null)
        {
            _logger.LogWarning("Product not found for ProductId {ProductId}", productId);
            throw new DataNotFoundException("Product not found");
        }

        var response = _mapper.Map<ResponseUserGetProductDetailDTO>(product);

        foreach (var variantResponse in response.ProductVariants)
        {
            var variant = product.ProductVariants.FirstOrDefault(v => v.ProductVariantId == variantResponse.ProductVariantId);

            if (variant == null)
                continue;

            variantResponse.isAvailableForSale =
                variant.Inventories.Any(inv =>
                    inv != null &&
                    inv.IsActive &&
                    inv.AvailableQuantity > 0 &&
                    inv.Address != null &&
                    inv.Address.IsActive) &&
            variant.Product.Vendor.ApprovalStatusId == (int)ApprovalStatusEnum.Accepted &&
               variant.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved &&
               variant.ProductVariantStatusId == (int)ProductStatusEnum.Active;
        }

        _logger.LogInformation("Returning full details for ProductId {ProductId}", product.ProductId);

        return response;
    }
}