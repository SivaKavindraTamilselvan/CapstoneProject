using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductService : IVendorProductService
{
    public async Task<PagedResponse<ResponseVendorGetAllProductDTO>> GetAllProductsByVendorId(
        RequestVendorProductFilter request, int vendorId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested product list {@Request}", vendorId, request);
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        _logger.LogInformation("Vendor UserId {VendorUserId} validated. VendorId {VendorId}", vendorId, vendor.VendorId);

        bool needsInMemoryPaging = request.hasIssues.HasValue || request.isAvailableForSale.HasValue;
        RequestVendorProductFilter repoRequest = request;

        if (needsInMemoryPaging)
        {
            repoRequest = new RequestVendorProductFilter
            {
                PageNumber = 1,
                PageSize = int.MaxValue,
                ProductApprovalStatusId = request.ProductApprovalStatusId,
                ProductCategoryId = request.ProductCategoryId,
                ProductSubCategoryId = request.ProductSubCategoryId,
                ProductStatusId = request.ProductStatusId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                SearchTerm = request.SearchTerm,
                ProductName = request.ProductName,
                MinAvailableQuantity = request.MinAvailableQuantity,
                MaxAvailableQuantity = request.MaxAvailableQuantity,
                MinReservedQuantity = request.MinReservedQuantity,
                MaxReservedQuantity = request.MaxReservedQuantity,
                includeIsDeleted = request.includeIsDeleted,
                isAvailableForSale = request.isAvailableForSale,
                hasIssues = request.hasIssues
            };
        }

        var result = await _productRepsository.GetVendorProduct(repoRequest, vendor.VendorId);
        var products = result.items;
        _logger.LogInformation("Repo returned {Count} products (TotalCount: {Total})", products.Count, result.totalCount);

        var response = _mapper.Map<List<ResponseVendorGetAllProductDTO>>(products);

        for (int i = 0; i < products.Count; i++)
        {
            _logger.LogDebug("Validating ProductId {ProductId}", products[i].ProductId);
            var validation = await _productValidation.ValidateProductChain(products[i]);

            response[i].IsAvailableForSale =
                products[i].ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved &&
                products[i].ProductStatusId == (int)ProductStatusEnum.Active &&
                validation.IsValid &&
                products[i].ProductVariants.Any(pv =>
                    pv.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved &&
                    pv.ProductVariantStatusId == (int)ProductStatusEnum.Active &&
                    pv.Inventories.Any(inv =>
                        inv != null && inv.IsActive &&
                        inv.AvailableQuantity > 0 &&
                        inv.Address != null && inv.Address.IsActive));

            response[i].ValidationIssues = validation.Issues;

            _logger.LogDebug("ProductId {Id} — IsValid: {Valid}, Issues: {Count}",
                products[i].ProductId, validation.IsValid, validation.Issues.Count);

            foreach (var variantResponse in response[i].ProductVariants)
            {
                var variant = products[i].ProductVariants
                    .FirstOrDefault(pv => pv.ProductVariantId == variantResponse.ProductVariantId);

                if (variant == null) continue;

                variantResponse.IsAvailableForSale =
                    variant.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved &&
                    variant.ProductVariantStatusId == (int)ProductStatusEnum.Active &&
                    validation.IsValid &&
                    variant.Inventories.Any(inv => inv.AvailableQuantity > 0);

                variantResponse.ValidationIssues = new List<string>
                    { "Same for all product variants. Present in the main product." };
            }
        }

        if (request.hasIssues.HasValue)
        {
            _logger.LogInformation("Applying hasIssues={Value} filter in memory", request.hasIssues.Value);
            response = request.hasIssues.Value
                ? response.Where(p => p.ValidationIssues.Any()).ToList()
                : response.Where(p => !p.ValidationIssues.Any()).ToList();
        }

        int totalCount;
        List<ResponseVendorGetAllProductDTO> pagedItems;

        if (needsInMemoryPaging)
        {
            totalCount = response.Count;
            pagedItems = response
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            _logger.LogInformation(
                "In-memory paging applied. TotalAfterFilter: {Total}, Page: {Page}, Returning: {Count}",
                totalCount, request.PageNumber, pagedItems.Count);
        }
        else
        {
            totalCount = result.totalCount;
            pagedItems = response;
        }

        _logger.LogInformation("Returning {Count}/{Total} products for VendorId {VendorId}",
            pagedItems.Count, totalCount, vendor.VendorId);

        return new PagedResponse<ResponseVendorGetAllProductDTO>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<ResponseVendorGetAllProductDTO> GetProductWithFullDetails(int productId, int userId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested full details for ProductId {ProductId}",
            userId, productId);

        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(userId);
        await _productValidation.VendorValidateProduct(productId, vendor.VendorId);
        _logger.LogInformation("Vendor access validated for ProductId {ProductId}", productId);

        var product = await _productRepsository.GetProductWithFullDetails(productId);
        if (product == null)
        {
            _logger.LogWarning("Product not found for ProductId {ProductId}", productId);
            throw new DataNotFoundException("Product not found");
        }

        var response = _mapper.Map<ResponseVendorGetAllProductDTO>(product);

        var validation = await _productValidation.ValidateProductChain(product);

        response.IsAvailableForSale =
            product.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved &&
            product.ProductStatusId == (int)ProductStatusEnum.Active &&
            validation.IsValid &&
            product.ProductVariants.Any(pv =>
                pv.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved &&
                pv.ProductVariantStatusId == (int)ProductStatusEnum.Active &&
                pv.Inventories.Any(inv =>
                    inv != null && inv.IsActive &&
                    inv.AvailableQuantity > 0 &&
                    inv.Address != null && inv.Address.IsActive));

        response.ValidationIssues = validation.Issues;

        foreach (var variantResponse in response.ProductVariants)
        {
            var variant = product.ProductVariants
                .FirstOrDefault(pv => pv.ProductVariantId == variantResponse.ProductVariantId);

            if (variant == null) continue;

            variantResponse.IsAvailableForSale =
                variant.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved &&
                variant.ProductVariantStatusId == (int)ProductStatusEnum.Active &&
                validation.IsValid &&
                variant.Inventories.Any(inv => inv.AvailableQuantity > 0);

            variantResponse.ValidationIssues = new List<string>
                { "Same for all product variants. Present in the main product." };
        }

        _logger.LogInformation("Returning full details for ProductId {ProductId}", product.ProductId);

        return response;
    }

    public async Task<PagedResponse<ResponseVendorGetProductVariantOnly>> GetAllProductVariant(
        RequestVendorProductVariantFilter request, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested product variant list with filters {@Request}",
            vendorUserId, request);

        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);

        bool needsInMemoryPaging = request.hasIssues.HasValue || request.IsAvailableForSale.HasValue;
        RequestVendorProductVariantFilter repoRequest = request;

        if (needsInMemoryPaging)
        {
            repoRequest = new RequestVendorProductVariantFilter
            {
                PageNumber = 1,
                PageSize = int.MaxValue,
                SKU = request.SKU,
                ProductId = request.ProductId,
                SearchTerm = request.SearchTerm,
                CategoryId = request.CategoryId,
                SubCategoryId = request.SubCategoryId,
                StatusId = request.StatusId,
                ApprovalStatusId = request.ApprovalStatusId,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                MinimuQuantityPerUser = request.MinimuQuantityPerUser,
                AddedByVendorUserId = request.AddedByVendorUserId,
                IsReturn = request.IsReturn,
                IsExchange = request.IsExchange,
                MainProductSubCategoryAttributeId = request.MainProductSubCategoryAttributeId,
                MinAvailableQuantity = request.MinAvailableQuantity,
                MaxAvailableQuantity = request.MaxAvailableQuantity,
                MinReservedQuantity = request.MinReservedQuantity,
                MaxReservedQuantity = request.MaxReservedQuantity,
                IsAvailableForSale = request.IsAvailableForSale,
                hasIssues = request.hasIssues
            };
        }

        var result = await _productVariantRepsository.GetAllVariantsForVendor(repoRequest, vendor.VendorId);
        _logger.LogInformation("Retrieved {VariantCount} variants for Vendor UserId {VendorUserId}",
            result.Items.Count, vendorUserId);

        var products = result.Items;
        var response = _mapper.Map<List<ResponseVendorGetProductVariantOnly>>(products);

        for (int i = 0; i < products.Count; i++)
        {
            _logger.LogDebug("Validating ProductVariantId {ProductVariantId}", products[i].ProductVariantId);
            var validation = await _productValidation.ValidateProductChain(products[i].Product!);

            response[i].IsAvailableForSale =
                products[i].ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved &&
                products[i].ProductVariantStatusId == (int)ProductStatusEnum.Active &&
                validation.IsValid &&
                products[i].Inventories.Any(inv =>
                    inv != null && inv.IsActive &&
                    inv.AvailableQuantity > 0 &&
                    inv.Address != null && inv.Address.IsActive);

            response[i].ValidationIssues = validation.Issues;

            _logger.LogDebug(
                "Validation completed for ProductVariantId {ProductVariantId}. IsValid: {IsValid}, IssuesCount: {IssuesCount}",
                products[i].ProductVariantId, validation.IsValid, validation.Issues.Count);
        }

        if (request.hasIssues.HasValue)
        {
            _logger.LogInformation("Applying hasIssues={Value} filter in memory", request.hasIssues.Value);
            response = request.hasIssues.Value
                ? response.Where(p => p.ValidationIssues.Any()).ToList()
                : response.Where(p => !p.ValidationIssues.Any()).ToList();
        }

        if (request.IsAvailableForSale.HasValue)
        {
            _logger.LogInformation("Applying IsAvailableForSale={Value} filter in memory", request.IsAvailableForSale.Value);
            response = response.Where(p => p.IsAvailableForSale == request.IsAvailableForSale.Value).ToList();
        }

        int totalCount;
        List<ResponseVendorGetProductVariantOnly> pagedItems;

        if (needsInMemoryPaging)
        {
            totalCount = response.Count;
            pagedItems = response
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            _logger.LogInformation(
                "In-memory paging applied. TotalAfterFilter: {Total}, Page: {Page}, Returning: {Count}",
                totalCount, request.PageNumber, pagedItems.Count);
        }
        else
        {
            totalCount = result.TotalCount;
            pagedItems = response;
        }

        _logger.LogInformation("Returning {Count} product variants after filtering", pagedItems.Count);

        return new PagedResponse<ResponseVendorGetProductVariantOnly>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<ResponseVendorGetProductVariantOnly> GetProductVariantWithFullDetails(int productVariantId, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested full details for ProductVariantId {ProductVariantId}",
            vendorUserId, productVariantId);

        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        await _productValidation.ValidateProductVariant(productVariantId, vendorUserId);
        _logger.LogInformation("Vendor access validated for ProductVariantId {ProductVariantId}", productVariantId);

        var variant = await _productVariantRepsository.GetVariantsForVendor(productVariantId);
        if (variant == null)
        {
            _logger.LogWarning("ProductVariant not found for ProductVariantId {ProductVariantId}", productVariantId);
            throw new DataNotFoundException("Product variant not found");
        }

        var response = _mapper.Map<ResponseVendorGetProductVariantOnly>(variant);

        var validation = await _productValidation.ValidateProductChain(variant.Product!);

        response.IsAvailableForSale =
            variant.ProductApprovalStatusId == (int)ProductApprovalStatusEnum.Admin_Approved &&
            variant.ProductVariantStatusId == (int)ProductStatusEnum.Active &&
            validation.IsValid &&
            variant.Inventories.Any(inv =>
                inv != null && inv.IsActive &&
                inv.AvailableQuantity > 0 &&
                inv.Address != null && inv.Address.IsActive);

        response.ValidationIssues = validation.Issues;

        _logger.LogInformation("Returning full details for ProductVariantId {ProductVariantId}", variant.ProductVariantId);

        return response;
    }
    public async Task<PagedResponse<ResponseAdminGetCategoryAttribute>> GetCategoryAttribute(RequestSubCategoryAttributeFilter request)
    {
        var productAttribute = await _productSubCategoryAttributeRepsository.GetAdminCategoryAttribute(request);
        if (productAttribute.totalCount == 0)
        {
            throw new DataNotFoundException("No Mapped attribute found");
        }
        return new PagedResponse<ResponseAdminGetCategoryAttribute>
        {
            Items = _mapper.Map<List<ResponseAdminGetCategoryAttribute>>(productAttribute.items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = productAttribute.totalCount
        };
    }
    public async Task<List<ResponseAdminGetAttribute>> GetAllAttribute()
    {
        var (attribute, totalCount) = await _attributeRepsository.GetAllAttributeVendor();
        if (totalCount == 0)
        {
            throw new DataNotFoundException("No Attribute is found");
        }
        return _mapper.Map<List<ResponseAdminGetAttribute>>(attribute);
    }
}