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
    [HttpGet("categories/{productCategoryId}/subcategories")]
    public async Task<IActionResult> GetSubCategoriesForUser(int productCategoryId)
    {
        var result = await _userProductCategoryService.GetAllProductSubCategor(productCategoryId);
        return Ok(result);
    }
    [HttpGet("vendor-categories/{productCategoryId}/subcategories")]
    public async Task<IActionResult> GetSubCategoriesForVendor(int productCategoryId)
    {
        var result = await _userProductCategoryService.GetAllProductSubCategoryVendor(productCategoryId);
        return Ok(result);
    }
}