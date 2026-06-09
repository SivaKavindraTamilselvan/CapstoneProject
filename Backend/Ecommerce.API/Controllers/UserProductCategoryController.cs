using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UserProductCategoryController : ControllerBase
{
    private readonly IUserProductCategoryService _userProductCategoryService;
    public UserProductCategoryController(IUserProductCategoryService userProductCategoryService)
    {
        _userProductCategoryService = userProductCategoryService;
    }
    [HttpGet("categories")]
    public async Task<IActionResult> GetAllProductCategory()
    {
        var result = await _userProductCategoryService.GetAllProductCategory();
        return Ok(result);
    }

    [HttpGet("subcategory/attributes")]
    public async Task<IActionResult> GetAllProductSubCategoryAttributeNames([FromQuery] int subcategoryid)
    {
        var result = await _userProductCategoryService.GetAllProductSubCategoryAttributeNames(subcategoryid);
        return Ok(result);
    }
    [HttpGet("subcategories")]
    public async Task<IActionResult> GetAllProductSubCategoryNames([FromQuery] int ProductCategoryId)
    {
        var result = await _userProductCategoryService.GetAllProductSubCategoryNames(ProductCategoryId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpGet("subcategories/vendor")]
    public async Task<IActionResult> GetAllProductSubCategoryNamesVendor([FromQuery] int ProductCategoryId)
    {
        var result = await _userProductCategoryService.GetAllProductSubCategoryNamesVendor(ProductCategoryId);
        return Ok(result);
    }
}