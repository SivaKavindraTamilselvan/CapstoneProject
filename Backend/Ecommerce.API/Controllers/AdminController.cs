using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IAdminProductService _adminProductService;
    private readonly IAdminVendorService _adminVendorService;
    public AdminController(IAdminService adminService,IAdminProductService adminProductService,IAdminVendorService adminVendorService)
    {
        _adminService = adminService;
        _adminProductService = adminProductService;
        _adminVendorService = adminVendorService;
    }

    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPost("RegisterAdmin")]
    public async Task<ActionResult<ResponseRegisterUserDTO>> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminService.RegisterAdmin(requestRegisterAdminDTO, adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpPost("ReviewVendor")]
    public async Task<ActionResult<ResponseReviewOfVendorDTO>> ReviewVendor(RequestReviewOfVendorDTO dto)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminVendorService.ReviewVendor(dto, adminUserId);

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
        var result = await _adminProductService.AddProductCategory(requestAddProductCategoryDTO);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("AddProductSubCategory")]
    public async Task<ActionResult<ResponseAddProductSubCategoryDTO>> AddProductSubCategory(RequestAddProductSubCategoryDTO requestAddProductSubCategoryDTO)
    {
        var result = await _adminProductService.AddProductSubCategory(requestAddProductSubCategoryDTO);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("AddProductSubCategoryAttribute")]
    public async Task<ActionResult<ResponseAddProductSubCategoryAttributeDTO>> AddProductSubCategoryAttribute(RequestAddProductSubCategoryAttributeDTO requestAddProductSubCategoryAttributeDTO)
    {
        var result = await _adminProductService.AddProductSubCategoryAttribute(requestAddProductSubCategoryAttributeDTO);
        return Ok(result);
    }
    [Authorize(Policy = "ProductAdminOrSuperAdminOnly")]
    [HttpPost("AddAttribute")]
    public async Task<ActionResult<ResponseAddAttributeDTO>> AddAttribute(RequestAddAttributeDTO requestAddAttributeDTO)
    {
        var result = await _adminProductService.AddAttribute(requestAddAttributeDTO);
        return Ok(result);
    }
}