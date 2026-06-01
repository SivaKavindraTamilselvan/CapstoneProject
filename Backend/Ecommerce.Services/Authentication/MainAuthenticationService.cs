using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AuthenticationService : IAuthentication
{
    private readonly IUserRepsository _userRepsository;
    private readonly ITokenService _tokenService;
    private readonly EcommerceContext _ecommerceContext;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IMapper _mapper;
    public AuthenticationService(EcommerceContext ecommerceContext, IUserRepsository userRepsository, ITokenService tokenService, ILogger<AuthenticationService> logger, IMapper mapper)
    {
        _ecommerceContext = ecommerceContext;
        _userRepsository = userRepsository;
        _tokenService = tokenService;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<ResponseRegisterUserDTO> Register(RequestRegisterUserDTO requestRegisterUserDTO)
    {
        _logger.LogInformation("User registration started for {Email}", requestRegisterUserDTO.Email);
        var existingUser = await _userRepsository.GetUserByEmail(requestRegisterUserDTO.Email);
        if (existingUser != null)
        {
            throw new Exception("Email already found");
        }
        User user = _mapper.Map<User>(requestRegisterUserDTO);
        HMACSHA256 hMACSHA256 = new HMACSHA256();
        user.Password = hMACSHA256.ComputeHash(Encoding.UTF32.GetBytes(requestRegisterUserDTO.Password));
        user.HashedKey = hMACSHA256.Key;
        user.RoleId = 3;
        await _userRepsository.Create(user);
        _logger.LogInformation("User registered successfully with UserId {UserId}", user.UserId);
        return _mapper.Map<ResponseRegisterUserDTO>(user);

    }
}