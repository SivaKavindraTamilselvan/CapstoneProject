using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminProductService : IAdminProductService
{
    public async Task<PagedResponse<ResponseAdminGetAllProductDTO>> GetAllProductsForAdmin(
        RequestAdminProductFilter request, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);
        _logger.LogInformation("Admin requested product list {@Request}", request);

        bool needsInMemoryPaging = request.hasIssues.HasValue;
        RequestAdminProductFilter repoRequest = request;

        if (needsInMemoryPaging)
        {
            repoRequest = new RequestAdminProductFilter
            {
                PageNumber = 1,
                PageSize = int.MaxValue,
                ProductApprovalStatusId = request.ProductApprovalStatusId,
                ProductCategoryId = request.ProductCategoryId,
                ProductSubCategoryId = request.ProductSubCategoryId,
                ProductStatusId = request.ProductStatusId,
                VendorId = request.VendorId,
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

        var result = await _productRepsository.GetAdminProduct(repoRequest);
        var products = result.items;

        _logger.LogInformation("Repo returned {Count} products (TotalCount: {Total})",
            products.Count, result.totalCount);

        var response = _mapper.Map<List<ResponseAdminGetAllProductDTO>>(products);

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
        List<ResponseAdminGetAllProductDTO> pagedItems;

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

        return new PagedResponse<ResponseAdminGetAllProductDTO>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResponse<ResponseAdminProductVariantOnlyDTO>> GetAllProductVariant(
        RequestAdminProductVariantFilter request, int adminUserId)
    {
        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);

        bool needsInMemoryPaging = request.hasIssues.HasValue;
        RequestAdminProductVariantFilter repoRequest = request;

        if (needsInMemoryPaging)
        {
            repoRequest = new RequestAdminProductVariantFilter
            {
                PageNumber = 1,
                PageSize = int.MaxValue,
                VendorId = request.VendorId,
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

        _logger.LogInformation("Admin requested product variant list with filters {@Request}", request);

        // ── Fix: pass repoRequest, not request ───────────────────────────────────
        var result = await _productVariantRepsository.GetAllVariantsForAdmin(repoRequest);

        _logger.LogInformation("Retrieved {VariantCount} product variants from repository. TotalCount: {TotalCount}",
            result.Items.Count, result.TotalCount);

        var products = result.Items;
        var response = _mapper.Map<List<ResponseAdminProductVariantOnlyDTO>>(products);

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
        List<ResponseAdminProductVariantOnlyDTO> pagedItems;

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

        return new PagedResponse<ResponseAdminProductVariantOnlyDTO>
        {
            Items = pagedItems,
            TotalCount = totalCount,
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

        var response = _mapper.Map<ResponseAdminGetAllProductDTO>(product);

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
     public async Task<ResponseVendorGetProductVariantOnly> GetProductVariantWithFullDetails(int productVariantId, int adminUserId)
    {
        _logger.LogInformation("Admin UserId {VendorUserId} requested full details for ProductVariantId {ProductVariantId}",
            adminUserId, productVariantId);

        await _adminUserValidation.ValidateAdminUserByUserId(adminUserId);

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

     public async Task<List<ApprovalHistoryDto>> GetProductHistory(int entityId)
    {
        var histories = await _approvalHistoryRepsository.GetProductApprovalHistory(entityId);
        return _mapper.Map<List<ApprovalHistoryDto>>(histories);
    }
    public async Task<List<ApprovalHistoryDto>> GetProductVariantHistory(int entityId)
    {
        var histories = await _approvalHistoryRepsository.GetProductVariantApprovalHistory(entityId);
        return _mapper.Map<List<ApprovalHistoryDto>>(histories);
    }

}