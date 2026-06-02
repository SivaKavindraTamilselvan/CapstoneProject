using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
       _productService = productService;
    }

    [Authorize(Policy = "VendorOnwerAndProductVendorOnly")]
    [HttpPost("AddProduct")]
    public async Task<ActionResult<ResponseAddProduct>> AddProduct(RequestAddProduct requestAddProduct)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _productService.AddProduct(requestAddProduct,vendorUserId);
        return Ok(result);
    }
}