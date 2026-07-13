using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Ecommerce.API.Controllers;

[EnableRateLimiting("AuthPolicy")]
[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly IAuthentication _authentication;
    public AuthenticationController(IProfileService profileService, IAuthentication authentication)
    {
        _profileService = profileService;
        _authentication = authentication;
    }
    [HttpPost("Register")]
    public async Task<ActionResult<ResponseRegisterUserDTO>> RegisterUser(RequestRegisterUserDTO requestRegisterUserDTO)
    {
        var result = await _authentication.Register(requestRegisterUserDTO);
        return Ok(result);
    }
    [HttpPost("RegisterVendor")]
    public async Task<ActionResult<ResponseRegisterVendorDTO>> RegisterVendor(RequestRegisterVendorDTO requestRegisterVendorDTO)
    {
        var result = await _authentication.RegisterVendor(requestRegisterVendorDTO);
        return Ok(result);
    }
    [HttpPost("Login")]
    public async Task<ActionResult<ResponseLoginUserDTO>> CustomerLogin(RequestLoginUserDTO requestLoginUserDTO)
    {
        var result = await _authentication.Login(requestLoginUserDTO);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("Profile")]
    public async Task<ActionResult<ResponseGetProfileDTO>> GetProfile()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _profileService.GetProfile(userId);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] RequestChangePasswordDTO request)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _authentication.ChangePassword(request, userId);
        return Ok(result);
    }

    [HttpPost("set-password")]
    [AllowAnonymous]
    public async Task<IActionResult> SetPassword([FromBody] RequestSetPasswordDTO requestSetPasswordDTO)
    {
        var result = await _authentication.SetPassword(requestSetPasswordDTO);
        return Ok(result);
    }
}