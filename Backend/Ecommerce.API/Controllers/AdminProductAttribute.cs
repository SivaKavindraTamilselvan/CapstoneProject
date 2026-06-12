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
        var result = await _service.GetAllAttributeAdmin(request);
        return Ok(result);
    }

    [HttpPatch("{attributeId}/activate")]
    public async Task<ActionResult<ResponseAdminGetAttribute>> ActivateAttribute(int attributeId)
    {
        var result = await _service.ActivateProductAttribute(attributeId);
        return Ok(result);
    }

    [HttpPatch("{attributeId}/deactivate")]
    public async Task<ActionResult<ResponseAdminGetAttribute>> DeactivateAttribute(int attributeId)
    {
        var result = await _service.DeactivateProductAttribute(attributeId);
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
        var result = await _service.GetAdminCategoryAttribute(request);
        return Ok(result);
    }

    [HttpPatch("subcategory-attributes/{subcategoryAttributeId}/activate")]
    public async Task<ActionResult<ResponseAdminGetCategoryAttribute>> ActivateSubCategoryAttribute(int subcategoryAttributeId)
    {
        var result = await _service.ActivateProductSubCategoryAttribute(subcategoryAttributeId);
        return Ok(result);
    }

    [HttpPatch("subcategory-attributes/{subcategoryAttributeId}/deactivate")]
    public async Task<ActionResult<ResponseAdminGetCategoryAttribute>> DeactivateSubCategoryAttribute(int subcategoryAttributeId)
    {
        var result = await _service.DectivateProductSubCategoryAttribute(subcategoryAttributeId);
        return Ok(result);
    }
}