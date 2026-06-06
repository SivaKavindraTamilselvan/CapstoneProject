using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IVendorProductService _vendorProductService;
    private readonly IVendorProductImageService _vendorProductImageService;
    private readonly IVendorProductVariantService _vendorProductVariantService;
    public ProductController(IProductService productService, IVendorProductService vendorProductService,IVendorProductImageService vendorProductImageService,IVendorProductVariantService vendorProductVariantService)
    {
        _productService = productService;
        _vendorProductService = vendorProductService;
        _vendorProductImageService = vendorProductImageService;
        _vendorProductVariantService = vendorProductVariantService;
    }

    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPost("AddProduct")]
    public async Task<ActionResult<ResponseAddProduct>> AddProduct(RequestAddProduct requestAddProduct)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.AddProduct(requestAddProduct, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPost("AddProductImage")]
    public async Task<ActionResult<ResponseAddProductImage>> AddProductImage(RequestAddProductImage requestAddProductImage)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductImageService.AddProductImage(requestAddProductImage,vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPost("AddProductVariantImage")]
    public async Task<ActionResult<ResponseAddProductVariantImage>> AddProductVariantImage(RequestAddProductVariantImage requestAddProductVariantImage)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductImageService.AddProductVariantImage(requestAddProductVariantImage,vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPost("AddProductVariant")]
    public async Task<ActionResult<ResponseAddProductVariantDTO>> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductVariantService.AddProductVariant(requestAddProductVariantDTO,vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPost("AddProductVariantAttribute")]
    public async Task<ActionResult<ResponseAddProductVariantAttributeDTO>> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductVariantService.AddProductVariantAttribute(requestAddProductVariantAttributeDTO,true);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerOnly")]
    [HttpPost("UpdateProduct")]
    public async Task<ActionResult<ResponseUpdateProduct>> UpdateProduct(RequestUpdateProduct requestUpdateProduct)
    {
        var result = await _vendorProductService.UpdateProduct(requestUpdateProduct);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerOnly")]
    [HttpPost("UpdateProductVariant")]
    public async Task<ActionResult<ResponseUpdateProductVariantDTO>> UpdateProductVariant(RequestUpdateProductVariantDTO requestUpdateProductVariantDTO)
    {
        var result = await _vendorProductVariantService.UpdateProductVariant(requestUpdateProductVariantDTO);
        return Ok(result);
    }

    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPost("UpdateProductImageAsDefault")]
    public async Task<ActionResult<ResponseMakeDefaultImageDTO>> UpdateProductImageAsDefault(RequestMakeDefaultImageDTO requestMakeDefaultImageDTO)
    {
        var result = await _vendorProductImageService.MakeImageDefault(requestMakeDefaultImageDTO);
        return Ok(result);
    }


}