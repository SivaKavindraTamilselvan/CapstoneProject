using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VendorProductController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IVendorProductService _vendorProductService;
    private readonly IVendorProductImageService _vendorProductImageService;
    private readonly IVendorProductVariantService _vendorProductVariantService;

    public VendorProductController(IBlobStorageService blobStorageService, IVendorProductService vendorProductService, IVendorProductImageService vendorProductImageService, IVendorProductVariantService vendorProductVariantService)
    {
        _blobStorageService = blobStorageService;
        _vendorProductService = vendorProductService;
        _vendorProductImageService = vendorProductImageService;
        _vendorProductVariantService = vendorProductVariantService;
    }

    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpGet]
    public async Task<IActionResult> GetAllProductsByVendorId([FromQuery] RequestVendorProductFilter request)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.GetAllProductsByVendorId(request, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpGet("ProductVariant")]
    public async Task<IActionResult> GetAllProductsVariantByVendorId([FromQuery] RequestVendorProductVariantFilter filter)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.GetAllProductVariant(filter, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductWithFullDetails(int productId)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.GetProductWithFullDetails(productId, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpGet("variant/{productVariantId}")]
    public async Task<IActionResult> GetProductVariantWithFullDetails(int productVariantId)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.GetProductVariantWithFullDetails(productVariantId, vendorUserId);
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
    [HttpDelete("DeleteProductImage/{productImageId}")]
    public async Task<ActionResult<ResponseAddProductVariantImage>> DeleteImage(int productImageId)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductImageService.DeleteProductImage(productImageId, vendorUserId);
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
        var result = await _vendorProductVariantService.AddProductVariantAttribute(requestAddProductVariantAttributeDTO, true, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("UpdateProduct")]
    public async Task<ActionResult<ResponseUpdateProduct>> UpdateProduct(RequestUpdateProductStatus requestUpdateProduct)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.UpdateProduct(requestUpdateProduct, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("UpdateProductDetails")]
    public async Task<ActionResult<ResponseUpdateProduct>> UpdateProduct(RequestUpdateProduct requestUpdateProduct)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductService.UpdateRejectedOrPendingProduct(requestUpdateProduct, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("UpdateProductVariant")]
    public async Task<ActionResult<ResponseUpdateProductVariantDTO>> UpdateProductVariant(RequestUpdateProductVariantDTO requestUpdateProductVariantDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductVariantService.UpdateProductVariant(requestUpdateProductVariantDTO, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("UpdateRejectedProductVariant")]
    public async Task<ActionResult<ResponseUpdateProductVariantDTO>> UpdateProductVariantDeatils(RequestUpdateProductVariant requestUpdateProductVariantDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorProductVariantService.UpdateRejectedProductVariant(requestUpdateProductVariantDTO, vendorUserId);
        return Ok(result);
    }

    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPut("UpdateProductImageAsDefault/{productImageId}")]
    public async Task<ActionResult<ResponseMakeDefaultImageDTO>> UpdateProductImageAsDefault(int productImageId)
    {
        RequestMakeDefaultImageDTO requestMakeDefaultImageDTO = new RequestMakeDefaultImageDTO();
        requestMakeDefaultImageDTO.ProductImageId = productImageId;
        var result = await _vendorProductImageService.MakeImageDefault(requestMakeDefaultImageDTO);
        return Ok(result);
    }


    [HttpGet("subcategory-attributes")]
    public async Task<ActionResult<List<ResponseAdminGetCategoryAttribute>>> GetSubCategoryAttributes([FromQuery] RequestSubCategoryAttributeFilter request)
    {
        var result = await _vendorProductService.GetCategoryAttribute(request);
        return Ok(result);
    }

    [HttpGet("attributes")]
    public async Task<ActionResult<List<ResponseAdminGetAttribute>>> GetAllAttributes()
    {
        var result = await _vendorProductService.GetAllAttribute();
        return Ok(result);
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadProductImage([FromForm] RequestUploadProductImage requestUploadProductImage)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (requestUploadProductImage.File == null || requestUploadProductImage.File.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = requestUploadProductImage.File.OpenReadStream();
        var blobUrl = await _blobStorageService.UploadImageAsync(
            stream,
            requestUploadProductImage.File.FileName,
            requestUploadProductImage.File.ContentType);

        var requestAddProductImage = new RequestAddProductImage
        {
            ProductId = requestUploadProductImage.ProductId,
            ImageUrl = blobUrl,
            DisplayOrderId = requestUploadProductImage.DisplayOrderId,
            IsMainImage = requestUploadProductImage.IsMainImage
        };

        var result = await _vendorProductImageService.AddProductImage(requestAddProductImage, vendorUserId);
        return Ok(result);
    }


    [HttpPost("upload-variant-image")]
    public async Task<IActionResult> UploadProductVariantImage([FromForm] RequestUploadProductVariantImage requestUploadProductImage)
    {
        Console.WriteLine("dnmkdsmc");
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        if (requestUploadProductImage.File == null || requestUploadProductImage.File.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = requestUploadProductImage.File.OpenReadStream();
        var blobUrl = await _blobStorageService.UploadImageAsync(
            stream,
            requestUploadProductImage.File.FileName,
            requestUploadProductImage.File.ContentType);

        Console.WriteLine(requestUploadProductImage.File);
        Console.WriteLine(blobUrl);
        var requestAddProductImage = new RequestAddProductVariantImage
        {
            ProductVariantId = requestUploadProductImage.ProductVariantId,
            ImageUrl = blobUrl,
            DisplayOrderId = requestUploadProductImage.DisplayOrderId,
            
        };

        var result = await _vendorProductImageService.AddProductVariantImage(requestAddProductImage, vendorUserId);
        return Ok(result);
    }

}