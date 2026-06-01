using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;

public partial class AuthenticationService : IAuthentication
{
    public async Task<ResponseLoginUserDTO> Login(RequestLoginUserDTO requestLoginUserDTO)
    {
        var result = await _userRepsository.GetUserByEmail(requestLoginUserDTO.Email);
        if (result == null)
        {
            throw new InvalidCredentialException("Email Not Found");
        }
        HMACSHA256 hMACSHA256 = new HMACSHA256(result.HashedKey);
        var userHashPassword = hMACSHA256.ComputeHash(Encoding.UTF32.GetBytes(requestLoginUserDTO.Password));
        Console.WriteLine(userHashPassword);
        for (int i = 0; i < userHashPassword.Length; i++)
            if (userHashPassword[i] != result.Password[i])
                throw new InvalidCredentialException("Invalid username or password");
        
        
        string token = _tokenService.CreateNewToken(_mapper.Map<TokenRequest>(result));
        return new ResponseLoginUserDTO
        {
            UserId = result.UserId,
            Email = result.Email,
            FirstName = result.FirstName,
            LastName = result.LastName,
            PhoneNumber = result.PhoneNumber,
            RoleId = result.RoleId,
            IsActive = result.IsActive,
            Token = token
        };
    }

}
