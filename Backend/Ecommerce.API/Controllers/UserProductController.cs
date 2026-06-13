using Ecommerce.DTOs;
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
    public async Task<IActionResult> GetUserProducts([FromQuery] RequestUserProductFilter request)
    {
        var result = await _userProductService.GetUserProducts(request);
        return Ok(result);
    }
    /*
    [HttpGet("available/subcategory/{subCategoryId}")]
    public async Task<IActionResult> GetAllAvailableProductsBySubCategoryId(int subCategoryId)
    {
        var result = await _userProductService.
        return Ok(result);
    }
    [HttpGet("available/category/{categoryId}")]
    public async Task<IActionResult> GetAllAvailableProductsByCategoryId(int categoryId)
    {
        var result = await _userProductService.GetAllAvailableProductsByCategoryId(categoryId);
        return Ok(result);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductWithFullDetails(int productId)
    {
        var result = await _userProductService.GetProductWithFullDetails(productId);
        return Ok(result);
    }
    */
}