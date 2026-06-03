using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    public async Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO, int vendorUserId)
    {
        var vendorUser = await _vendorValidation.ValidateVendorUser(vendorUserId);
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
    public async Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct, int vendorUserId)
    {
        var vendorUser = await _vendorValidation.ValidateVendorUser(vendorUserId);
        var vendor = await _vendorValidation.ValidateVendorIfApproved(vendorUser.VendorId);
        await _productCategoryValidation.ValidateSubCategory(requestAddProduct.ProductSubCategoryId);
        var product = _mapper.Map<Product>(requestAddProduct);
        product.VendorId = vendor.VendorId;
        product.AddedByVendorUserId = vendorUser.VendorUserId;
        await _productRepsository.Create(product);
        return _mapper.Map<ResponseAddProduct>(product);
    }
    public async Task<ResponseAddProductImage> AddProductImage(RequestAddProductImage requestAddProductImage, int vendorUserId)
    {
        var vendorUser = await _vendorValidation.ValidateVendorUser(vendorUserId);
        var product = await _productValidation.ValidateProduct(requestAddProductImage.ProductId);
        if (requestAddProductImage.ProductVariantId != null)
        {
            var productVariant = await _productValidation.ValidateProductVariant(requestAddProductImage.ProductVariantId.Value);
        }
        var productImage = _mapper.Map<ProductImage>(requestAddProductImage);
        productImage.AddedByVendorUserId = vendorUser.VendorUserId;
        await _productImageRepsository.Create(productImage);
        return _mapper.Map<ResponseAddProductImage>(productImage);
    }

    public async Task<ResponseAddProductVariantAttributeDTO> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO, int productVariantId,int productSubCategoryId)
    {
        await _productCategoryValidation.ValidateProductSubCategoryAttribute(requestAddProductVariantAttributeDTO.ProductSubCategoryAttributeId,productSubCategoryId);
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