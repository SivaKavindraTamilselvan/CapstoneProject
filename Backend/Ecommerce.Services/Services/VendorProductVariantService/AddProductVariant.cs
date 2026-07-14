using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorProductVariantService : IVendorProductVariantService
{
    public async Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} started adding product variant for ProductId {ProductId}", vendorUserId, requestAddProductVariantDTO.ProductId);

        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);

        var product = await _productValidation.ValidateProductIfApproved(requestAddProductVariantDTO.ProductId);
        await _productValidation.VendorValidateProduct(product.ProductId, vendorUser.VendorId);

        if (requestAddProductVariantDTO.ProductVariantAttribute == null || !requestAddProductVariantDTO.ProductVariantAttribute.Any())
        {
            _logger.LogWarning("Variant creation failed for ProductId {ProductId}: attributes are missing", requestAddProductVariantDTO.ProductId);
            throw new InvalidDataException("Variant attributes are required");
        }

        if (!requestAddProductVariantDTO.ProductVariantAttribute.Any(x => x.ProductSubCategoryAttributeId == product.MainProductSubCategoryAttributeId))
        {
            _logger.LogWarning("Variant creation failed for ProductId {ProductId}: main attribute missing", product.ProductId);
            throw new InvalidDataException("Main attribute value is required");
        }

        await _productAttributeValidation.ValidateProductSubCategoryAttribute(product.MainProductSubCategoryAttributeId, product.ProductSubCategoryId);

        var duplicateAttribute = requestAddProductVariantDTO.ProductVariantAttribute.GroupBy(x => x.ProductSubCategoryAttributeId).FirstOrDefault(g => g.Count() > 1);

        if (duplicateAttribute != null)
        {
            _logger.LogWarning("Duplicate variant attribute found for ProductId {ProductId}, ProductSubCategoryAttributeId {AttributeId}", product.ProductId, duplicateAttribute.Key);
            throw new InvalidDataException("Duplicate variant attributes are not allowed");
        }

        var productVariant = _mapper.Map<ProductVariant>(requestAddProductVariantDTO);
        productVariant.AddedByVendorUserId = vendorUser.VendorUserId;
        productVariant.SKU = await GenerateSku(product.ProductId);

        if (vendorUser.VendorRoleId == 1)
        {
            productVariant.ProductApprovalStatusId = 2;
        }
        await _productVariantRepsository.Create(productVariant);

        _logger.LogInformation("Product Variant {ProductVariantId} created for ProductId {ProductId} with SKU {SKU}", productVariant.ProductVariantId, product.ProductId, productVariant.SKU);

        foreach (var list in requestAddProductVariantDTO.ProductVariantAttribute)
        {
            RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO = new RequestAddProductVariantAttributeDTO();
            requestAddProductVariantAttributeDTO.AttributeValue = list.AttributeValue;
            requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId = list.ProductSubCategoryAttributeId;
            requestAddProductVariantAttributeDTO.ProductVariantId = productVariant.ProductVariantId;

            await AddProductVariantAttribute(requestAddProductVariantAttributeDTO, false, vendorUserId);
        }

        var productAdminUserIds = await _adminUserRepsository.GetProductAdminUserIds();

        _logger.LogInformation("Sending new product variant notification to {AdminCount} product admins for ProductVariantId {ProductVariantId}", productAdminUserIds.Count, productVariant.ProductVariantId);

        foreach (var adminUserId in productAdminUserIds)
        {
            await _notificationService.SendToUser(adminUserId, "New Product Variant Submitted", $"A new variant '{productVariant.SKU}' for product '{product.ProductName}' has been submitted and is waiting for approval.", notificationTypeId: 1, referenceType: "ProductVariant", referenceId: productVariant.ProductVariantId);
        }

        _logger.LogInformation("Product variant notifications sent successfully for ProductVariantId {ProductVariantId}", productVariant.ProductVariantId);

        return _mapper.Map<ResponseAddProductVariantDTO>(productVariant);
    }
    public async Task<ResponseAddProductVariantAttributeDTO> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO, bool updation, int userId)
    {
        _logger.LogInformation("Vendor UserId {UserId} started processing attribute for ProductVariantId {ProductVariantId}", userId, requestAddProductVariantAttributeDTO.ProductVariantId);
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(userId);
        var productVariant = await _productValidation.ValidateProductVariant(requestAddProductVariantAttributeDTO.ProductVariantId, userId);
        var product = await _productValidation.ValidateProduct(productVariant.ProductId);
        var productVariantAttribute = _mapper.Map<ProductVariantAttribute>(requestAddProductVariantAttributeDTO);
        await _productAttributeValidation.ValidateProductSubCategoryAttribute(requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId, product.ProductSubCategoryId);
        if (!updation)
        {
            productVariantAttribute.AddedByVendorUserId = vendorUser.VendorUserId;
            await _productVariantAttributeRepsository.Create(productVariantAttribute);
        }
        if(updation)
        {
            var result = await _productVariantAttributeRepsository.CheckAttributeAlreadyAdded(requestAddProductVariantAttributeDTO.ProductVariantId,requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId);
            if(result !=null)
            {
                throw new DataAlreadyRegisteredException("Product Attribute Already Added");
            }
            productVariantAttribute.AddedByVendorUserId = vendorUser.VendorUserId;
            productVariantAttribute.UpdatedAt = DateTime.Now;
            await _productVariantAttributeRepsository.Create(productVariantAttribute);
        }
        var ownerUser = await _vendorUserRepsository.GetOwnerVendorUserByVendorId(vendorUser.VendorId);
        if (ownerUser != null)
        {
            await _notificationService.SendToUser(
                ownerUser.UserId,
                "New Product Submitted",
                $"A new product variant attribute'{product.ProductName}' has been submitted and is waiting for approval.",
                notificationTypeId: 1,
                referenceType: "Product",
                referenceId: product.ProductId);
        }
        return _mapper.Map<ResponseAddProductVariantAttributeDTO>(productVariantAttribute);
    }
    private async Task<string> GenerateSku(int productId)
    {
        string sku;
        do
        {
            var randomCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            sku = $"PV-{productId:D6}-{randomCode}";

        } while ((await _productVariantRepsository.GetAll()).Any(v => v.SKU == sku));

        return sku;
    }
}