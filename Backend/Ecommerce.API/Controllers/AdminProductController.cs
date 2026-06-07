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

    [HttpGet("all")]
    public async Task<IActionResult> GetAllProducts()
    {
        var result = await _adminProductService.GetAllProducts();
        return Ok(result);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetAllPendingAdminApprovalProducts()
    {
        var result = await _adminProductService.GetAllPendingAdminApprovalProducts();
        return Ok(result);
    }
    [HttpGet("approved")]
    public async Task<IActionResult> GetAllAdminApprovedProducts()
    {
        var result = await _adminProductService.GetAllAdminApprovedProducts();
        return Ok(result);
    }

    [HttpGet("rejected")]
    public async Task<IActionResult> GetAllAdminRejectedProducts()
    {
        var result = await _adminProductService.GetAllAdminRejectedProducts();
        return Ok(result);
    }

    [HttpGet("vendor-rejected")]
    public async Task<IActionResult> GetAllVendorRejectedProducts()
    {
        var result = await _adminProductService.GetAllVendorRejectedProducts();
        return Ok(result);
    }
    [HttpGet("deleted")]
    public async Task<IActionResult> GetAllDeletedByAdminProducts()
    {
        var result = await _adminProductService.GetAllDeletedByAdminProducts();
        return Ok(result);
    }

    [HttpGet("unavailable")]
    public async Task<IActionResult> GetAllTemporarilyUnavailableProducts()
    {
        var result = await _adminProductService.GetAllTemporarilyUnavailableProducts();
        return Ok(result);
    }

    [HttpGet("archived")]
    public async Task<IActionResult> GetAllArchivedProducts()
    {
        var result = await _adminProductService.GetAllArchivedProducts();
        return Ok(result);
    }
    [HttpGet("outofstock")]
    public async Task<IActionResult> GetAllOutOfStockProducts()
    {
        var result = await _adminProductService.GetAllOutOfStockProducts();
        return Ok(result);
    }

    [HttpGet("lowstock")]
    public async Task<IActionResult> GetAllLowStockProducts([FromQuery] int threshold = 5)
    {
        var result = await _adminProductService.GetAllLowStockProducts(threshold);
        return Ok(result);
    }

    [HttpGet("pending-variants")]
    public async Task<IActionResult> GetAllProductsWithPendingVariants()
    {
        var result = await _adminProductService.GetAllProductsWithPendingVariants();
        return Ok(result);
    }
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
    [HttpPost("AddProductCategory")]
    public async Task<ActionResult<ResponseAddProductCategoryDTO>> AddProductCategory(RequestAddProductCategoryDTO requestAddProductCategoryDTO)
    {
        var result = await _adminProductCategoryService.AddProductCategory(requestAddProductCategoryDTO);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("AddProductSubCategory")]
    public async Task<ActionResult<ResponseAddProductSubCategoryDTO>> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO)
    {
        var result = await _adminProductCategoryService.AddProductSubCategory(requestAddProductSubCategoryDTO);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("AddProductSubCategoryAttribute")]
    public async Task<ActionResult<ResponseAddProductSubCategoryAttributeDTO>> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO)
    {
        var result = await _adminProductAttributeService.AddProductSubCategoryAttribute(requestAddProductSubCategoryAttributeDTO);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("AddAttribute")]
    public async Task<ActionResult<ResponseAddAttributeDTO>> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO)
    {
        var result = await _adminProductAttributeService.AddAttribute(requestAddAttributeDTO);
        return Ok(result);
    }
    
}