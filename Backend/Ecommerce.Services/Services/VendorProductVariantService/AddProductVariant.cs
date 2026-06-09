using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class VendorProductVariantService : IVendorProductVariantService
{
    public async Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO, int vendorUserId)
    {
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
        var product = await _productValidation.ValidateProductIfApproved(requestAddProductVariantDTO.ProductId);
        if (requestAddProductVariantDTO.requestAddProductVariantAttributeDTOs == null || !requestAddProductVariantDTO.requestAddProductVariantAttributeDTOs.Any())
        {
            throw new InvalidDataException("Variant attributes are required");
        }
        if (!requestAddProductVariantDTO.requestAddProductVariantAttributeDTOs.Any(x => x.ProductSubCategoryAttributeId == requestAddProductVariantDTO.MainProductSubCategoryAttributeId))
        {
            throw new InvalidDataException("Main attribute value is required");
        }
        await _productAttributeValidation.ValidateProductSubCategoryAttribute(requestAddProductVariantDTO.MainProductSubCategoryAttributeId, product.ProductSubCategoryId);
        var duplicateAttribute = requestAddProductVariantDTO.requestAddProductVariantAttributeDTOs.GroupBy(x => x.ProductSubCategoryAttributeId).FirstOrDefault(g => g.Count() > 1);

        if (duplicateAttribute != null)
        {
            throw new InvalidDataException("Duplicate variant attributes are not allowed");
        }
        var productVariant = _mapper.Map<ProductVariant>(requestAddProductVariantDTO);
        productVariant.AddedByVendorUserId = vendorUser.VendorUserId;
        productVariant.SKU = await GenerateSku(product.ProductId);
        productVariant.ProductApprovalStatusId = 4; // need to change later

        await _productVariantRepsository.Create(productVariant);
        foreach (var list in requestAddProductVariantDTO.requestAddProductVariantAttributeDTOs)
        {
            RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO = new RequestAddProductVariantAttributeDTO();
            requestAddProductVariantAttributeDTO.AttributeValue = list.AttributeValue;
            requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId = list.ProductSubCategoryAttributeId;
            requestAddProductVariantAttributeDTO.ProductVariantId = productVariant.ProductVariantId;
            await AddProductVariantAttribute(requestAddProductVariantAttributeDTO, false, vendorUserId);
        }
        return _mapper.Map<ResponseAddProductVariantDTO>(productVariant);
    }
    public async Task<ResponseAddProductVariantAttributeDTO> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO, bool updation, int userId)
    {
        var productVariant = await _productValidation.ValidateProductVariant(requestAddProductVariantAttributeDTO.ProductVariantId, userId);
        var product = await _productValidation.ValidateProduct(productVariant.ProductId);
        await _productAttributeValidation.ValidateProductSubCategoryAttribute(requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId, product.ProductSubCategoryId);
        var productVariantAttribute = _mapper.Map<ProductVariantAttribute>(requestAddProductVariantAttributeDTO);
        await _productVariantAttributeRepsository.Create(productVariantAttribute);
        if (updation)
        {
            productVariant.ProductApprovalStatusId = 2;
            productVariant.UpdatedAt = DateTime.Now;
            await _productVariantRepsository.Update(productVariant.ProductVariantId, productVariant);
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