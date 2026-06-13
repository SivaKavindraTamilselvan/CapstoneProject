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
    public async Task<IActionResult> GetAllProducts([FromQuery] RequestAdminProductFilter request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductService.GetAllProductsForAdmin(request,adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductWithFullDetails(int productId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductService.GetProductWithFullDetails(productId,adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpGet("ProductVariant")]
    public async Task<IActionResult> GetAllProductsVariant(RequestAdminProductVariantFilter filter)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductService.GetAllProductVariant(filter,adminUserId);
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
    [HttpPatch("DeleteProduct")]
    public async Task<ActionResult<ResponseReviewOfProductDTO>> DeleteProduct(RequestDeleteProductDTO requestDeleteProductDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductService.DeleteProduct(requestDeleteProductDTO,adminUserId);
        return Ok(result);
    }
    
}