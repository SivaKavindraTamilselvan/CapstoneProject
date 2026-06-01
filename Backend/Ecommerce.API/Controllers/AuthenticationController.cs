using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthentication _authentication;
    public AuthenticationController(IAuthentication authentication)
    {
        _authentication = authentication;
    }
    [HttpPost("Register")]
    public async Task<ActionResult<ResponseRegisterUserDTO>> RegisterUser(RequestRegisterUserDTO requestRegisterUserDTO)
    {
        var result = await _authentication.Register(requestRegisterUserDTO);
        return Ok(result);
    }
    [HttpPost("Login")]
    public async Task<ActionResult<ResponseLoginUserDTO>> CustomerLogin(RequestLoginUserDTO requestLoginUserDTO)
    {
        var result = await _authentication.Login(requestLoginUserDTO);
        return Ok(result);
    }
}