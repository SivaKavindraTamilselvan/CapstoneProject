using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/admin/product-attributes")]
[ApiController]
[Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
public class AdminProductAttributeController : ControllerBase
{
    private readonly IAdminProductAttributeService _service;

    public AdminProductAttributeController(IAdminProductAttributeService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ResponseAddAttributeDTO>> AddAttribute([FromBody] RequestAddAttributeDTO request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.AddAttribute(request, adminUserId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ResponseAdminGetAttribute>>> GetAllAttributes([FromQuery] RequestAttributeFilter request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.GetAllAttributeAdmin(request,adminUserId);
        return Ok(result);
    }

    [HttpPatch("{attributeId}/activate")]
    public async Task<ActionResult<ResponseAdminGetAttribute>> ActivateAttribute(int attributeId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.ActivateProductAttribute(attributeId,adminUserId);
        return Ok(result);
    }

    [HttpPatch("{attributeId}/deactivate")]
    public async Task<ActionResult<ResponseAdminGetAttribute>> DeactivateAttribute(int attributeId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.DeactivateProductAttribute(attributeId,adminUserId);
        return Ok(result);
    }

    [HttpPost("subcategory-attributes")]
    public async Task<ActionResult<ResponseAddProductSubCategoryAttributeDTO>> AddSubCategoryAttribute([FromBody] RequestAddProductSubCategoryAttributeDTO request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.AddProductSubCategoryAttribute(request, adminUserId);
        return Ok(result);
    }

    [HttpGet("subcategory-attributes")]
    public async Task<ActionResult<List<ResponseAdminGetCategoryAttribute>>> GetSubCategoryAttributes([FromQuery] RequestSubCategoryAttributeFilter request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.GetAdminCategoryAttribute(request,adminUserId);
        return Ok(result);
    }

    [HttpPatch("subcategory-attributes/{subcategoryAttributeId}/activate")]
    public async Task<ActionResult<ResponseAdminGetCategoryAttribute>> ActivateSubCategoryAttribute(int subcategoryAttributeId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.ActivateProductSubCategoryAttribute(subcategoryAttributeId,adminUserId);
        return Ok(result);
    }

    [HttpPatch("subcategory-attributes/{subcategoryAttributeId}/deactivate")]
    public async Task<ActionResult<ResponseAdminGetCategoryAttribute>> DeactivateSubCategoryAttribute(int subcategoryAttributeId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.DectivateProductSubCategoryAttribute(subcategoryAttributeId,adminUserId);
        return Ok(result);
    }
}