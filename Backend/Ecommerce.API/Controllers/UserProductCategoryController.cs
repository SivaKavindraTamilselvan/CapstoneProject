using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
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

    [HttpGet("attributes")]
    public async Task<IActionResult> GetAllAttributeNames()
    {
        var result = await _userProductCategoryService.GetAllAttributeNames();
        return Ok(result);
    }

    [HttpGet("subcategory/attributes")]
    public async Task<IActionResult> GetAllProductSubCategoryAttributeNames([FromQuery] RequestGetAllProductSubCategoryAttributeName request)
    {
        var result = await _userProductCategoryService.GetAllProductSubCategoryAttributeNames(request);
        return Ok(result);
    }

    [HttpGet("subcategories")]
    public async Task<IActionResult> GetAllProductSubCategoryNames([FromQuery] RequestGetAllProductSubCategoryName request)
    {
        var result = await _userProductCategoryService.GetAllProductSubCategoryNames(request);
        return Ok(result);
    }
    [HttpGet("subcategories/vendor")]
    public async Task<IActionResult> GetAllProductSubCategoryNamesVendor([FromQuery] RequestGetAllProductSubCategoryName request)
    {
        var result = await _userProductCategoryService.GetAllProductSubCategoryNamesVendor(request);
        return Ok(result);
    }
}