using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class VendorProductService : IVendorProductService
{
    public async Task<ResponseAddProductVariantDTO> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO)
    {
        var product = (await _productRepsository.GetAll()).FirstOrDefault(p => p.ProductId == requestAddProductVariantDTO.ProductId);
        if (product == null)
        {
            throw new DataNotFoundException("Product Not Found");
        }
        if(product.ApprovalStatusId !=2 )
        {
            throw new DataApprovalStatusException("Product Not Approved. Product Needed To Be Approved By Product Admin Or Super Admin");
        }
        ProductVariant productVariant = new ProductVariant();
        productVariant.AvailableQuantity = requestAddProductVariantDTO.AvailableQuantity;
        productVariant.ProductId = requestAddProductVariantDTO.ProductId;
        productVariant.Price = requestAddProductVariantDTO.Price;
        productVariant.WeightInKgs = requestAddProductVariantDTO.WeightInKgs;
        productVariant.WidthInCm = requestAddProductVariantDTO.WidthInCm;
        productVariant.LengthInCm = requestAddProductVariantDTO.LengthInCm;
        productVariant.HeightInCm = requestAddProductVariantDTO.HeightInCm;
        await _productVariantRepsository.Create(productVariant);
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
            throw new DataApprovalStatusException("Vendor Not Approved. Vendor Needed To Be Approved By Vendor Admin Or SuperAdmin");
        }
        Product product = new Product();
        product.VendorId = vendor.VendorId;
        product.Description = requestAddProduct.Description;
        product.ProductName = requestAddProduct.ProductName;
        product.ProductSubCategoryId = requestAddProduct.ProductSubCategoryId;
        await _productRepsository.Create(product);
        return _mapper.Map<ResponseAddProduct>(product);
    }
    public async Task<ResponseAddProductImage> AddProductImage(RequestAddProductImage requestAddProductImage)
    {
        ProductImage productImage = new ProductImage();
        productImage.ProductId = requestAddProductImage.ProductId;
        productImage.DisplayOrderId = requestAddProductImage.DisplayOrderId;
        productImage.ImageUrl = requestAddProductImage.ImageUrl;
        productImage.IsMainImage = requestAddProductImage.IsMainImage;
        await _productImageRepsository.Create(productImage);
        return _mapper.Map<ResponseAddProductImage>(productImage);
    }
}