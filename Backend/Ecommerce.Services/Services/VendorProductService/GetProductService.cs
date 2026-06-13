using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductService : IVendorProductService
{
    public async Task<PagedResponse<ResponseVendorGetAllProductDTO>> GetAllProductsByVendorId(RequestVendorProductFilter request, int vendorId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested product list with filters {@Request}", vendorId, request);
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        var result = await _productRepsository.GetVendorProduct(request, vendor.VendorId);
        var products = result.items;
        _logger.LogInformation("Vendor UserId {VendorUserId} validated successfully. VendorId {VendorId}", vendorId, vendor.VendorId);
        var response = _mapper.Map<List<ResponseVendorGetAllProductDTO>>(result.items);
        for (int i = 0; i < result.totalCount; i++)
        {
            _logger.LogDebug("Validating ProductId {ProductId}", products[i].ProductId);
            var validation = await _productValidation.ValidateProductChain(products[i]);
            response[i].IsAvailableForSale = products[i].ProductApprovalStatusId == 4 && products[i].ProductStatusId == 2 && validation.IsValid && products[i]
            .ProductVariants.Any(pv => pv.ProductApprovalStatusId == 4 && pv.ProductVariantStatusId == 2 && pv.Inventories.Any(inv => inv.AvailableQuantity > 0 && inv.Address!.IsActive));
            response[i].ValidationIssues = validation.Issues;
            _logger.LogDebug("Validation completed for ProductId {ProductId}. IsValid {IsValid}. IssuesCount {IssuesCount}", products[i].ProductId, validation.IsValid, validation.Issues.Count);
            foreach (var variantResponse in response[i].ProductVariants)
            {
                var variant = products[i].ProductVariants.FirstOrDefault(pv => pv.ProductVariantId == variantResponse.ProductVariantId);
                if (variant == null)
                {
                    continue;
                }
                variantResponse.IsAvailableForSale = variant.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && variant.ProductVariantStatusId == (int)ProductStatusEnum.Active &&
                validation.IsValid && variant.Inventories.Any(inv => inv.AvailableQuantity > 0);
                variantResponse.ValidationIssues = new List<string> { "Same For All Product Variant. Present In The Main Product" };
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
        _logger.LogInformation("Returning {ProductCount} products for VendorId {VendorId}", response.Count, vendor.VendorId);
        return new PagedResponse<ResponseVendorGetAllProductDTO>
        {
            Items = response,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = result.totalCount
        };
    }

    public async Task<ResponseVendorGetAllProductDTO> GetProductWithFullDetails(int productId, int userId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested full details for ProductId {ProductId}", userId, productId);
        await _productValidation.VendorValidateProduct(productId, userId);
        _logger.LogInformation("Vendor access validated for ProductId {ProductId}", productId);
        var product = await _productRepsository.GetProductWithFullDetails(productId);
        if (product == null)
        {
            _logger.LogWarning("Product not found for ProductId {ProductId}", productId);
            throw new DataNotFoundException("Product not found");
        }
        _logger.LogInformation("Returning full details for ProductId {ProductId}", product.ProductId);
        return _mapper.Map<ResponseVendorGetAllProductDTO>(product);
    }
    public async Task<PagedResponse<ResponseVendorGetProductVariantOnly>> GetAllProductVariant(RequestVendorProductVariantFilter request, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested product variant list with filters {@Request}", vendorUserId, request);
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var result = await _productVariantRepsository.GetAllVariantsForVendor(request, vendor.VendorId);
        _logger.LogInformation("Returning {VariantCount} variants for Vendor UserId {VendorUserId}", result.Items.Count, vendorUserId);

        var products = result.Items;
        //Console.WriteLine(validation.IsValid);
        var response = _mapper.Map<List<ResponseVendorGetProductVariantOnly>>(products);
        for (int i = 0; i < products.Count; i++)
        {
            _logger.LogDebug("Validating ProductVariantId {ProductVariantId}", products[i].ProductVariantId);
            var validation = await _productValidation.ValidateProductChain(products[i].Product!);

            response[i].IsAvailableForSale = products[i].ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved && products[i].ProductVariantStatusId == (int)ProductStatusEnum.Active
            && validation.IsValid && products[i].Inventories.Any(inv => inv.AvailableQuantity > 0 && inv.Address!.IsActive);
           
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
        return new PagedResponse<ResponseVendorGetProductVariantOnly>
        {
            Items = response,
            TotalCount = result.TotalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}