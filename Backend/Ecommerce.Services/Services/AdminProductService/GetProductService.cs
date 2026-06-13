using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql.Internal;
using Org.BouncyCastle.Ocsp;

public partial class AdminProductService : IAdminProductService
{

    public async Task<PagedResponse<ResponseAdminGetAllProductDTO>> GetAllProductsForAdmin(RequestAdminProductFilter request, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Admin requested product list with filters {@Request}", request);
        var result = await _productRepsository.GetAdminProduct(request);
        _logger.LogInformation("Retrieved {ProductCount} products from repository. TotalCount: {TotalCount}", result.items.Count, result.totalCount);
        var products = result.items;
        var response = _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);
        for (int i = 0; i < products.Count; i++)
        {
            _logger.LogDebug("Validating ProductId {ProductId}", products[i].ProductId);
            var validation = await _productValidation.ValidateProductChain(products[i]);

            response[i].IsAvailableForSale = products[i].ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && products[i].ProductStatusId == (int)ProductStatusEnum.Active
            && validation.IsValid && products[i].ProductVariants.Any(pv => pv.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && pv.ProductVariantStatusId == (int)ProductStatusEnum.Active
            && pv.Inventories.Any(inv => inv.AvailableQuantity > 0));

            response[i].ValidationIssues = validation.Issues;

            _logger.LogDebug("Validation completed for ProductId {ProductId}. IsValid: {IsValid}, IssuesCount: {IssuesCount}", products[i].ProductId, validation.IsValid, validation.Issues.Count);
            foreach (var variantResponse in response[i].ProductVariants)
            {
                var variant = products[i].ProductVariants.FirstOrDefault(pv => pv.ProductVariantId == variantResponse.ProductVariantId);
                if (variant == null)
                {
                    continue;
                }
                variantResponse.IsAvailableForSale = variant.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && variant.ProductVariantStatusId == (int)ProductStatusEnum.Active &&
                validation.IsValid && variant.Inventories.Any(inv => inv.AvailableQuantity > 0);
                variantResponse.ValidationIssues = new List<string> {"Same For All Product Variant. Present In The Main Product"};
            }
        }
        _logger.LogInformation("Applying HasIssues filter: {HasIssues}", request.hasIssues);
        if (request.hasIssues.HasValue)
        {
            if (request.hasIssues.Value)
            {
                response = response.Where(p => p.ValidationIssues.Any()).ToList();
            }
            else
            {
                response = response.Where(p => !p.ValidationIssues.Any()).ToList();
            }
        }
        _logger.LogInformation("Applying IsAvailableForSale filter: {IsAvailableForSale}", request.isAvailableForSale);
        if (request.isAvailableForSale.HasValue)
        {
            response = response.Where(p => p.IsAvailableForSale == request.isAvailableForSale.Value).ToList();
        }
        _logger.LogInformation("Returning {Count} products after filtering", response.Count);
        return new PagedResponse<ResponseAdminGetAllProductDTO>
        {
            Items = response,
            TotalCount = result.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<PagedResponse<ResponseAdminProductVariantDTO>> GetAllProductVariant(RequestAdminProductVariantFilter request, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Admin requested product variant list with filters {@Request}", request);
        var result = await _productVariantRepsository.GetAllVariantsForAdmin(request);
        _logger.LogInformation("Retrieved {VariantCount} product variants from repository. TotalCount: {TotalCount}", result.Items.Count, result.TotalCount);
        var products = result.Items;
        var response = _mapper.Map<List<ResponseAdminProductVariantDTO>>(products);
        for (int i = 0; i < products.Count; i++)
        {
            _logger.LogDebug("Validating ProductVariantId {ProductVariantId}", products[i].ProductVariantId);

            var validation = await _productValidation.ValidateProductChain(products[i].Product!);

            response[i].IsAvailableForSale = products[i].ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && products[i].ProductVariantStatusId == (int)ProductStatusEnum.Active
            && validation.IsValid && products[i].Inventories.Any(inv => inv.AvailableQuantity > 0);

            response[i].ValidationIssues = validation.Issues;
            _logger.LogDebug("Validation completed for ProductVariantId {ProductVariantId}. IsValid: {IsValid}, IssuesCount: {IssuesCount}", products[i].ProductVariantId, validation.IsValid, validation.Issues.Count);
        }
        _logger.LogInformation("Applying HasIssues filter: {HasIssues}", request.hasIssues);
        if (request.hasIssues.HasValue)
        {
            if (request.hasIssues.Value)
            {
                response = response.Where(p => p.ValidationIssues.Any()).ToList();
            }
            else
            {
                response = response.Where(p => !p.ValidationIssues.Any()).ToList();
            }
        }
        _logger.LogInformation("Applying IsAvailableForSale filter: {IsAvailableForSale}", request.IsAvailableForSale);
        if (request.IsAvailableForSale.HasValue)
        {
            response = response.Where(p => p.IsAvailableForSale == request.IsAvailableForSale.Value).ToList();
        }
        _logger.LogInformation("Returning {Count} product variants after filtering", response.Count);
        return new PagedResponse<ResponseAdminProductVariantDTO>
        {
            Items = _mapper.Map<List<ResponseAdminProductVariantDTO>>(products),
            TotalCount = result.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<ResponseAdminGetAllProductDTO> GetProductWithFullDetails(int productId, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Admin requested full details for ProductId {ProductId}", productId);
        var product = await _productRepsository.GetProductWithFullDetails(productId);
        if (product == null)
        {
            _logger.LogWarning("Product not found for ProductId {ProductId}", productId);
            throw new DataNotFoundException("Product not found");
        }
        _logger.LogInformation("Returning full details for ProductId {ProductId}", product.ProductId);
        return _mapper.Map<ResponseAdminGetAllProductDTO>(product);
    }

}