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
    private readonly IVendorProductService _vendorProductService;
    private readonly IVendorProductImageService _vendorProductImageService;
    private readonly IVendorProductVariantService _vendorProductVariantService;
    private readonly IUserProductService _userProductService;
    public ProductController(IUserProductService userProductService,IVendorProductService vendorProductService, IVendorProductImageService vendorProductImageService, IVendorProductVariantService vendorProductVariantService)
    {
        _vendorProductService = vendorProductService;
        _vendorProductImageService = vendorProductImageService;
        _vendorProductVariantService = vendorProductVariantService;
        _userProductService = userProductService;
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
    [Authorize(Policy = "VendorOnwerOnly")]
    [HttpPost("AddProductVariantAttribute")]
    public async Task<ActionResult<ResponseAddProductVariantAttributeDTO>> AddProductVariantAttribute(RequestAddProductVariantAttributeDTO requestAddProductVariantAttributeDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductVariantService.AddProductVariantAttribute(requestAddProductVariantAttributeDTO, true);
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

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllProductCategory()
    {
        var result = await _userProductService.GetAllProductCategory();
        return Ok(result);
    }

    [HttpGet("attributes")]
    public async Task<IActionResult> GetAllAttributeNames()
    {
        var result = await _userProductService.GetAllAttributeNames();
        return Ok(result);
    }

    [HttpGet("subcategory/attributes")]
    public async Task<IActionResult> GetAllProductSubCategoryAttributeNames([FromQuery] RequestGetAllProductSubCategoryAttributeName request)
    {
        var result = await _userProductService.GetAllProductSubCategoryAttributeNames(request);
        return Ok(result);
    }

    [HttpGet("subcategories")]
    public async Task<IActionResult> GetAllProductSubCategoryNames([FromQuery] RequestGetAllProductSubCategoryName request)
    {
        var result = await _userProductService.GetAllProductSubCategoryNames(request);
        return Ok(result);
    }

    [HttpGet("subcategories/vendor")]
    public async Task<IActionResult> GetAllProductSubCategoryNamesVendor([FromQuery] RequestGetAllProductSubCategoryName request)
    {
        var result = await _userProductService.GetAllProductSubCategoryNamesVendor(request);
        return Ok(result);
    }

}