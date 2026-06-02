using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class ProductService : IProductService
{
    public async Task<ResponseAddProduct> AddProduct(RequestAddProduct requestAddProduct,int vendorUserId)
    {
        var vendorUser = (await _vendorUserRepsository.GetAll()).FirstOrDefault(v=>v.UserId == vendorUserId);
        if(vendorUser == null)
        {
            throw new DataNotFoundException("Vendor User Not Found");
        }
        var vendor = (await _vendorRepository.GetAll()).FirstOrDefault(v=>v.VendorId == vendorUser.VendorId);
        if(vendor == null)
        {
            throw new DataNotFoundException("Vendor Not Found");
        }
        if(vendor.ApprovalStatusId != 2)
        {
            throw new Exception("The Vendor Is Not Approved Yet");
        }
        Product product = new Product();
        product.VendorId = vendor.VendorId;
        product.Description = requestAddProduct.Description;
        product.ProductName = requestAddProduct.ProductName;
        product.ProductSubCategoryId = requestAddProduct.ProductSubCategoryId;
        await _productRepsository.Create(product);
        foreach(var list in requestAddProduct.Images)
        {
            ProductImage productImage = new ProductImage();
            productImage.ProductId = product.ProductId;
            productImage.DisplayOrderId = list.DisplayOrderId;
            productImage.ImageUrl = list.ImageUrl;
            productImage.IsMainImage = list.IsMainImage;
            await _productImageRepsository.Create(productImage);
        }
        return _mapper.Map<ResponseAddProduct>(product);
    }
}