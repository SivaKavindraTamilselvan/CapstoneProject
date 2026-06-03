using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    public async Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO, int vendorUserId)
    {
        var vendorUser = (await _vendorUserRepsository.GetAll()).FirstOrDefault(v => v.UserId == vendorUserId);
        if (vendorUser == null)
        {
            throw new DataNotFoundException("Vendor User Not Found");
        }
        var product = (await _productRepsository.GetAll()).FirstOrDefault(p => p.ProductId == requestAddProductVariantDTO.ProductId);
        if (product == null)
        {
            throw new DataNotFoundException("Product Not Found");
        }
        if (product.ProductApprovalStatusId != 4)
        {
            throw new DataApprovalStatusException("Product Is Not Yet Approved");
        }
        ProductVariant productVariant = new ProductVariant();
        productVariant.AvailableQuantity = requestAddProductVariantDTO.AvailableQuantity;
        productVariant.ProductId = requestAddProductVariantDTO.ProductId;
        productVariant.Price = requestAddProductVariantDTO.Price;
        productVariant.WeightInKgs = requestAddProductVariantDTO.WeightInKgs;
        productVariant.WidthInCm = requestAddProductVariantDTO.WidthInCm;
        productVariant.LengthInCm = requestAddProductVariantDTO.LengthInCm;
        productVariant.HeightInCm = requestAddProductVariantDTO.HeightInCm;
        productVariant.AddedByVendorUserId = vendorUser.VendorUserId;
        productVariant.SKU = await GenerateSku(product.ProductId);
        await _productVariantRepsository.Create(productVariant);
        foreach (var list in requestAddProductVariantDTO.requestAddProductVariantAttributeDTOs)
        {
            await AddProductVariantAttribute(list, productVariant.ProductVariantId);
        }
        return _mapper.Map<ResponseAddProductVariantDTO>(productVariant);
    }
    public async Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct, int vendorUserId)
    {
        var vendorUser = (await _vendorUserRepsository.GetAll()).FirstOrDefault(v => v.UserId == vendorUserId);
        if (vendorUser == null)
        {
            throw new DataNotFoundException("Vendor User Not Found");
        }
        var vendor = (await _vendorRepsository.GetAll()).FirstOrDefault(v => v.VendorId == vendorUser.VendorId);
        if (vendor == null)
        {
            throw new DataNotFoundException("Vendor Not Found");
        }
        if (vendor.ApprovalStatusId != 2)
        {
            throw new DataApprovalStatusException("The Vendor Is Not Approved Yet");
        }
        Product product = new Product();
        product.VendorId = vendor.VendorId;
        product.Description = requestAddProduct.Description;
        product.ProductName = requestAddProduct.ProductName;
        product.ProductSubCategoryId = requestAddProduct.ProductSubCategoryId;
        product.AddedByVendorUserId = vendorUser.VendorUserId;
        await _productRepsository.Create(product);
        return _mapper.Map<ResponseAddProduct>(product);
    }
    public async Task<ResponseAddProductImage> AddProductImage(RequestAddProductImage requestAddProductImage, int vendorUserId)
    {
        var vendorUser = (await _vendorUserRepsository.GetAll()).FirstOrDefault(v => v.UserId == vendorUserId);
        if (vendorUser == null)
        {
            throw new DataNotFoundException("Vendor User Not Found");
        }
        var product = (await _productRepsository.GetAll()).FirstOrDefault(p => p.ProductId == requestAddProductImage.ProductId);
        if (product == null)
        {
            throw new DataNotFoundException("Product Not Found");
        }
        if (requestAddProductImage.ProductVariantId != null)
        {
            var productVariant = (await _productVariantRepsository.GetAll()).FirstOrDefault(p => p.ProductVariantId == requestAddProductImage.ProductVariantId);
            if (productVariant == null)
            {
                throw new DataNotFoundException("Product Variant Not Found");
            }
        }
        ProductImage productImage = new ProductImage();
        productImage.ProductId = requestAddProductImage.ProductId;
        productImage.DisplayOrderId = requestAddProductImage.DisplayOrderId;
        productImage.ImageUrl = requestAddProductImage.ImageUrl;
        productImage.IsMainImage = requestAddProductImage.IsMainImage;
        productImage.AddedByVendorUserId = vendorUser.VendorUserId;
        await _productImageRepsository.Create(productImage);
        return _mapper.Map<ResponseAddProductImage>(productImage);
    }

    public async Task<ResponseAddProductVariantAttributeDTO> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO, int productVariantId)
    {
        ProductVariantAttribute productVariantAttribute = new ProductVariantAttribute();
        productVariantAttribute.ProductVariantId = productVariantId;
        productVariantAttribute.AttributeValue = requestAddProductVariantAttributeDTO.AttributeValue;
        productVariantAttribute.AttributeMasterId = requestAddProductVariantAttributeDTO.AttributeMasterId;
        await _productVariantAttributeRepsository.Create(productVariantAttribute);
        return _mapper.Map<ResponseAddProductVariantAttributeDTO>(productVariantAttribute);
    }

    private async Task<string> GenerateSku(int productId)
    {
        string sku;

        do
        {
            var randomCode = Guid.NewGuid()
                .ToString("N")
                .Substring(0, 8)
                .ToUpper();

            sku = $"PV-{productId:D6}-{randomCode}";

        } while ((await _productVariantRepsository.GetAll())
                    .Any(v => v.SKU == sku));

        return sku;
    }
}