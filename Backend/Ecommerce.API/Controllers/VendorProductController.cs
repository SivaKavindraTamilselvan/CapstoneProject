using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VendorProductController : ControllerBase
{
    private readonly IVendorProductService _vendorProductService;
    private readonly IVendorProductImageService _vendorProductImageService;
    private readonly IVendorProductVariantService _vendorProductVariantService;

    public VendorProductController(IVendorProductService vendorProductService, IVendorProductImageService vendorProductImageService, IVendorProductVariantService vendorProductVariantService)
    {
        _vendorProductService = vendorProductService;
        _vendorProductImageService = vendorProductImageService;
        _vendorProductVariantService = vendorProductVariantService;
    }

    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpGet]
    public async Task<IActionResult> GetAllProductsByVendorId([FromQuery] int? approval,[FromQuery] int? status,[FromQuery] int? subcategory,[FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10,[FromQuery] bool? hasIssues =null ,[FromQuery] bool? isAvailableForSale = null)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.GetAllProductsByVendorId(approval,status,vendorUserId,subcategory,pageNumber,pageSize,hasIssues,isAvailableForSale);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpGet("available")]
    public async Task<IActionResult> GetAllAvailableProductsByVendorId()
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.GetAllAvailableProductsByVendorId(vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpGet("lowstock")]
    public async Task<IActionResult> GetAllLowStockProducts([FromQuery] int threshold = 5)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.GetAllLowStockProducts(vendorUserId, threshold);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpGet("outofstock")]
    public async Task<IActionResult> GetAllOutOfStockProducts()
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.GetAllOutOfStockProducts(vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpGet("pending-variants")]
    public async Task<IActionResult> GetAllProductsWithPendingVariants()
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.GetAllProductsWithPendingVariants(vendorUserId);
        return Ok(result);
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
        var result = await _vendorProductImageService.AddProductImage(requestAddProductImage, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPost("AddProductVariantImage")]
    public async Task<ActionResult<ResponseAddProductVariantImage>> AddProductVariantImage(RequestAddProductVariantImage requestAddProductVariantImage)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductImageService.AddProductVariantImage(requestAddProductVariantImage, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPost("AddProductVariant")]
    public async Task<ActionResult<ResponseAddProductVariantDTO>> AddProductVariant(RequestAddProductVariantDTO requestAddProductVariantDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductVariantService.AddProductVariant(requestAddProductVariantDTO, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPost("AddProductVariantAttribute")]
    public async Task<ActionResult<ResponseAddProductVariantAttributeDTO>> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductVariantService.AddProductVariantAttribute(requestAddProductVariantAttributeDTO, true,vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("UpdateProduct")]
    public async Task<ActionResult<ResponseUpdateProduct>> UpdateProduct(RequestUpdateProductStatus requestUpdateProduct)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.UpdateProduct(requestUpdateProduct,vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("UpdateProductDetails")]
    public async Task<ActionResult<ResponseUpdateProduct>> UpdateProduct(RequestUpdateProduct requestUpdateProduct)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.UpdateRejectedOrPendingProduct(requestUpdateProduct,vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("UpdateProductVariant")]
    public async Task<ActionResult<ResponseUpdateProductVariantDTO>> UpdateProductVariant(RequestUpdateProductVariantDTO requestUpdateProductVariantDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductVariantService.UpdateProductVariant(requestUpdateProductVariantDTO,vendorUserId);
        return Ok(result);
    }

    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPut("UpdateProductImageAsDefault")]
    public async Task<ActionResult<ResponseMakeDefaultImageDTO>> UpdateProductImageAsDefault(RequestMakeDefaultImageDTO requestMakeDefaultImageDTO)
    {
        var result = await _vendorProductImageService.MakeImageDefault(requestMakeDefaultImageDTO);
        return Ok(result);
    }
}