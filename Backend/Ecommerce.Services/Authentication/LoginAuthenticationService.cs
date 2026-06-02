using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Ecommerce.DTOs;
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
        for (int i = 0; i < userHashPassword.Length; i++)
            if (userHashPassword[i] != result.Password[i])
                throw new InvalidCredentialException("Invalid username or password");
        
        int? adminRoleId = null;
        if (result.RoleId == 1)
        {
            var adminUser = (await _adminRepository.GetAll()).FirstOrDefault(a => a.UserId == result.UserId); ;
            adminRoleId = adminUser?.AdminRoleId;
        }
        string token = _tokenService.CreateNewToken(_mapper.Map<TokenRequest>(result));
        var response = _mapper.Map<ResponseLoginUserDTO>(result);
        response.Token = token;
        return response;
    }

}
