using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class VendorProductVariantService : IVendorProductVariantService
{
    public async Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO, int vendorUserId)
    {
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var product = await _productValidation.ValidateProductIfApproved(requestAddProductVariantDTO.ProductId);
        var productVariant = _mapper.Map<ProductVariant>(requestAddProductVariantDTO);
        productVariant.AddedByVendorUserId = vendorUser.VendorUserId;
        productVariant.SKU = await GenerateSku(product.ProductId);
        await _productVariantRepsository.Create(productVariant);
        foreach (var list in requestAddProductVariantDTO.requestAddProductVariantAttributeDTOs)
        {
            await AddProductVariantAttribute(list, productVariant.ProductVariantId,product.ProductSubCategoryId);
        }
        return _mapper.Map<ResponseAddProductVariantDTO>(productVariant);
    }
    public async Task<ResponseAddProductVariantAttributeDTO> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO, int productVariantId,int productSubCategoryId)
    {
        await _productAttributeValidation.ValidateProductSubCategoryAttribute(requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId,productSubCategoryId);
        var productVariantAttribute = _mapper.Map<ProductVariantAttribute>(requestAddProductVariantAttributeDTO);
        productVariantAttribute.ProductVariantId = productVariantId;
        await _productVariantAttributeRepsository.Create(productVariantAttribute);
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