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
        productVariant.ProductApprovalStatusId = 4; // need to change later
        await _productVariantRepsository.Create(productVariant);
        foreach (var list in requestAddProductVariantDTO.requestAddProductVariantAttributeDTOs)
        {
            RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO = new RequestAddProductVariantAttributeDTO();
            requestAddProductVariantAttributeDTO.AttributeValue = list.AttributeValue;
            requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId = list.ProductSubCategoryAttributeId;
            requestAddProductVariantAttributeDTO.ProductVariantId = productVariant.ProductVariantId;
            await AddProductVariantAttribute(requestAddProductVariantAttributeDTO,false);
        }
        return _mapper.Map<ResponseAddProductVariantDTO>(productVariant);
    }
    public async Task<ResponseAddProductVariantAttributeDTO> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO,bool updation)
    {
        var productVariant = await _productValidation.ValidateProductVariant(requestAddProductVariantAttributeDTO.ProductVariantId);
        var product = await _productValidation.ValidateProduct(productVariant.ProductId);
        await _productAttributeValidation.ValidateProductSubCategoryAttribute(requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId,product.ProductSubCategoryId);
        var productVariantAttribute = _mapper.Map<ProductVariantAttribute>(requestAddProductVariantAttributeDTO);
        await _productVariantAttributeRepsository.Create(productVariantAttribute);
        if(updation)
        {
            productVariant.ProductApprovalStatusId = 2;
            productVariant.UpdatedAt = DateTime.Now;
            await _productVariantRepsository.Update(productVariant.ProductVariantId,productVariant);
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