using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminProductController : ControllerBase
{
    private readonly IAdminProductAttributeService _adminProductAttributeService;
    private readonly IAdminProductCategoryService _adminProductCategoryService;
    private readonly IAdminProductService _adminProductService;

    public AdminProductController(IAdminProductService adminProductService,IAdminProductAttributeService adminProductAttributeService,IAdminProductCategoryService adminProductCategoryService)
    {
        _adminProductAttributeService = adminProductAttributeService;
        _adminProductCategoryService = adminProductCategoryService;
        _adminProductService = adminProductService;
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllProducts(int? approval,int? status,int? vendorId,int? subcategory,bool? hasIssues,[FromQuery] bool? isAvailableForSale = null)
    {
        var result = await _adminProductService.GetAllProducts(approval,status,vendorId,subcategory,hasIssues,isAvailableForSale);
        return Ok(result);
    }

    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpGet("outofstock")]
    public async Task<IActionResult> GetAllOutOfStockProducts()
    {
        var result = await _adminProductService.GetAllOutOfStockProducts();
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpGet("lowstock")]
    public async Task<IActionResult> GetAllLowStockProducts([FromQuery] int threshold = 5)
    {
        var result = await _adminProductService.GetAllLowStockProducts(threshold);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpGet("pending-variants")]
    public async Task<IActionResult> GetAllProductsWithPendingVariants()
    {
        var result = await _adminProductService.GetAllProductsWithPendingVariants();
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductWithFullDetails(int productId)
    {
        var result = await _adminProductService.GetProductWithFullDetails(productId);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("ReviewProduct")]
    public async Task<ActionResult<ResponseReviewOfProductDTO>> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductService.ReviewProduct(requestReviewOfProductDTO, adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("ReviewProductVariant")]
    public async Task<ActionResult<ResponseReviewOfProductVariantDTO>> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductService.ReviewProductVariant(requestReviewOfProductDTO, adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("AddProductCategory")]
    public async Task<ActionResult<ResponseAddProductCategoryDTO>> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductCategoryService.AddProductCategory(requestAddProductCategoryDTO,adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("AddProductSubCategory")]
    public async Task<ActionResult<ResponseAddProductSubCategoryDTO>> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductCategoryService.AddProductSubCategory(requestAddProductSubCategoryDTO,adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("AddProductSubCategoryAttribute")]
    public async Task<ActionResult<ResponseAddProductSubCategoryAttributeDTO>> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductAttributeService.AddProductSubCategoryAttribute(requestAddProductSubCategoryAttributeDTO,adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("AddAttribute")]
    public async Task<ActionResult<ResponseAddAttributeDTO>> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductAttributeService.AddAttribute(requestAddAttributeDTO,adminUserId);
        return Ok(result);
    }
    
}