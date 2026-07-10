using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/admin/product-categories")]
[ApiController]
[Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
public class AdminProductCategoryController : ControllerBase
{
    private readonly IAdminProductCategoryService _adminProductCategoryService;

    public AdminProductCategoryController(IAdminProductCategoryService adminProductCategoryService)
    {
        _adminProductCategoryService = adminProductCategoryService;
    }

    [HttpPost]
    public async Task<ActionResult<ResponseAddProductCategoryDTO>> AddProductCategory([FromBody] RequestAddProductCategoryDTO request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductCategoryService.AddProductCategory(request, adminUserId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ResponseAdminGetAllCategory>>> GetAllProductCategories([FromQuery] RequestProductCategoryFilter request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductCategoryService.GetAllProductCategoryForAdmin(request,adminUserId);
        return Ok(result);
    }

    [HttpPatch("{productCategoryId}/activate")]
    public async Task<ActionResult<ResponseAdminGetAllCategory>> ActivateProductCategory(int productCategoryId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductCategoryService.ActivateProductCategory(productCategoryId,adminUserId);
        return Ok(result);
    }

    [HttpPatch("{productCategoryId}/deactivate")]
    public async Task<ActionResult<ResponseAdminGetAllCategory>> DeactivateProductCategory(int productCategoryId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductCategoryService.DeactivateProductCategory(productCategoryId,adminUserId);
        return Ok(result);
    }

    [HttpPost("subcategories")]
    public async Task<ActionResult<ResponseAddProductSubCategoryDTO>> AddProductSubCategory([FromBody] RequestAddProductSubCategoryDTO request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductCategoryService.AddProductSubCategory(request, adminUserId);
        return Ok(result);
    }

    [HttpGet("subcategories")]
    public async Task<ActionResult<List<ResponseAdminGetAllSubCategory>>> GetAllProductSubCategories([FromQuery] RequestProductSubCategoryFilter request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductCategoryService.GetAllSubProductCategoryForAdmin(request,adminUserId);
        return Ok(result);
    }

    [HttpPatch("subcategories/{productSubCategoryId}/activate")]
    public async Task<ActionResult<ResponseAdminGetAllSubCategory>> ActivateProductSubCategory(int productSubCategoryId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductCategoryService.ActivateProductSubCategory(productSubCategoryId,adminUserId);
        return Ok(result);
    }

    [HttpPatch("subcategories/{productSubCategoryId}/deactivate")]
    public async Task<ActionResult<ResponseAdminGetAllSubCategory>> DeactivateProductSubCategory(int productSubCategoryId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminProductCategoryService.DeactivateProductSubCategory(productSubCategoryId,adminUserId);
        return Ok(result);
    }
}