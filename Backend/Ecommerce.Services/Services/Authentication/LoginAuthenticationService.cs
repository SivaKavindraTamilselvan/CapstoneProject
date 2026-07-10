using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AuthenticationService : IAuthentication
{
    public async Task<ResponseLoginUserDTO> Login(RequestLoginUserDTO requestLoginUserDTO)
    {
        _logger.LogInformation("Login attempt for {Email}", requestLoginUserDTO.Email);
        var result = await _userRepsository.GetUserByEmail(requestLoginUserDTO.Email);
        if (result == null)
        {
            _logger.LogError("Login failed. User not found for {Email}", requestLoginUserDTO.Email);
            throw new InvalidCredentialException("Email Not Found");
        }
        HMACSHA256 hMACSHA256 = new HMACSHA256(result.HashedKey);
        var userHashPassword = hMACSHA256.ComputeHash(Encoding.UTF32.GetBytes(requestLoginUserDTO.Password));
        for (int i = 0; i < userHashPassword.Length; i++)
        {
            if (userHashPassword[i] != result.Password[i])
            {
                _logger.LogError("Login failed. Invalid password for {Email}", requestLoginUserDTO.Email);
                throw new InvalidCredentialException("Invalid password for the email");
            }
        }
        if (!result.IsActive)
        {
            _logger.LogError("Login failed. User with {email} is deleted or deactivated", requestLoginUserDTO.Email);
            throw new InvalidCredentialException("User Is deleted or deactivated");
        }
        if (result.RoleId == (int)RoleEnum.Vendor)
        {
            var vendor = await _vendorUserRepsository.GetVendorUserByUserId(result.UserId);
            if (vendor == null)
            {
                _logger.LogError("Login failed.Vendor User with {email} is deleted or deactivated", requestLoginUserDTO.Email);
                throw new InvalidCredentialException("Vendor Credential is not valid");
            }
            if(!vendor.IsActive)
            {
                _logger.LogError("Login failed. Vendor Is deleted or deactivated");
                throw new InvalidCredentialException("Vendor Is deleted");
            }
        }
        int? adminRoleId = null;
        if (result.RoleId == (int)RoleEnum.Admin)
        {
            var adminUser = await _adminRepository.GetAdminUserByUserId(result.UserId);
            if (adminUser == null)
            {
                _logger.LogError("Admin user record not found for UserId {UserId}", result.UserId);
                throw new DataNotFoundException("Admin User Not Found");
            }
            adminRoleId = adminUser.AdminRoleId;
        }
        int? vendorRoleId = null;
        if (result.RoleId == (int)RoleEnum.Vendor)
        {
            var vendorUser = await _vendorUserRepsository.GetVendorUserByUserId(result.UserId);
            if (vendorUser == null)
            {
                _logger.LogError("Vendor user record not found for UserId {UserId}", result.UserId);
                throw new DataNotFoundException("Vendor User Not Found");
            }
            vendorRoleId = vendorUser?.VendorRoleId;
        }
        _logger.LogInformation("Generating token for UserId {UserId}", result.UserId);
        var tokenRequest = _mapper.Map<TokenRequest>(result);
        string token = _tokenService.CreateNewToken(tokenRequest);
        //Console.WriteLine("AdminRoleId" + tokenRequest.AdminRoleId);
        //Console.WriteLine("VendorRole" + tokenRequest.VendorRoleId);
        var response = _mapper.Map<ResponseLoginUserDTO>(result);
        response.Token = token;
        _logger.LogInformation("Login successful for UserId {UserId}", result.UserId);
        return response;
    }

}
