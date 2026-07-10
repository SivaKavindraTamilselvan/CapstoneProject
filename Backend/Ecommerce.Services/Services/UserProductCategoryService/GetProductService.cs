using System.Diagnostics.CodeAnalysis;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public partial class UserProductService : IUserProductService
{
    public async Task<PagedResponse<ResponseUserGetProductDetailDTO>> GetUserProducts(RequestUserProductFilter request)
    {
        _logger.LogInformation("User requested product list with filters {@Request}", request);

        var products = await _productRepository.GetUserProducts(request);
        _logger.LogInformation("Retrieved {ProductCount} products. TotalCount {TotalCount}", products.items.Count, products.totalCount);

        return new PagedResponse<ResponseUserGetProductDetailDTO>
        {
            Items = _mapper.Map<List<ResponseUserGetProductDetailDTO>>(products.items),
            TotalCount = products.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<ResponseUserGetProductDetailDTO> GetProductWithFullDetails(int productId)
    {
        _logger.LogInformation("User requested full details for ProductId {ProductId}", productId);
        var result = await _productRepository.CheckTheWholeProduct(productId,1);
        if(result == null)
        {
            throw new DataNotFoundException("Product Not Found");
        }
        var product = await _productRepository.GetProductWithFullDetails(productId);
        if (product == null)
        {
            _logger.LogWarning("Product not found for ProductId {ProductId}", productId);
            throw new DataNotFoundException("Product not found");
        }
        _logger.LogInformation("Returning full details for ProductId {ProductId}", product.ProductId);

        return _mapper.Map<ResponseUserGetProductDetailDTO>(product);
    }
}