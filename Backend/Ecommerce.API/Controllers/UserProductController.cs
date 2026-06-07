using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserProductController : ControllerBase
{
    private readonly IUserProductService _userProductService;

    public UserProductController(IUserProductService userProductService)
    {
        _userProductService = userProductService;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAllAvailableProducts()
    {
        var result = await _userProductService.GetAllAvailableProducts();
        return Ok(result);
    }

    [HttpGet("available/subcategory/{subCategoryId}")]
    public async Task<IActionResult> GetAllAvailableProductsBySubCategoryId(int subCategoryId)
    {
        var result = await _userProductService.GetAllAvailableProductsBySubCategoryId(subCategoryId);
        return Ok(result);
    }
    [HttpGet("available/category/{categoryId}")]
    public async Task<IActionResult> GetAllAvailableProductsByCategoryId(int categoryId)
    {
        var result = await _userProductService.GetAllAvailableProductsByCategoryId(categoryId);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProductsByName([FromQuery] string searchTerm)
    {
        var result = await _userProductService.SearchProductsByName(searchTerm);
        return Ok(result);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductWithFullDetails(int productId)
    {
        var result = await _userProductService.GetProductWithFullDetails(productId);
        return Ok(result);
    }
}