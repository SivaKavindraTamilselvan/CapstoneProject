using System.Security.Cryptography;
using System.Text;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AuthenticationService : IAuthentication
{
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
    public async Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var requestUser = _mapper.Map<RequestRegisterUserDTO>(requestRegisterAdminDTO);
            HMACSHA256 hMACSHA256 = new HMACSHA256();
            requestUser.Password = requestRegisterAdminDTO.FirstName + "@12345";
            var user = _mapper.Map<User>(requestUser);
            user.Password = hMACSHA256.ComputeHash(Encoding.UTF32.GetBytes(requestUser.Password));
            user.HashedKey = hMACSHA256.Key;
            user.RoleId = 1;
            await _userRepsository.Create(user);

            var adminUser = _mapper.Map<AdminUser>(requestRegisterAdminDTO);
            adminUser.UserId = user.UserId;
            await _adminRepository.Create(adminUser);
            await transaction.CommitAsync();

            return _mapper.Map<ResponseRegisterAdminDTO>(adminUser);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<ResponseRegisterVendorDTO> RegisterVendor(RequestRegisterVendorDTO requestRegisterVendorDTO)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            User user = _mapper.Map<User>(requestRegisterVendorDTO.requestRegisterUserDTO);
            HMACSHA256 hMACSHA256 = new HMACSHA256();
            user.Password = hMACSHA256.ComputeHash(Encoding.UTF32.GetBytes(requestRegisterVendorDTO.requestRegisterUserDTO.Password));
            user.HashedKey = hMACSHA256.Key;
            user.RoleId = 2;
            await _userRepsository.Create(user);

            var vendor = _mapper.Map<Vendor>(requestRegisterVendorDTO);
            await _vendorRepsository.Create(vendor);

            VendorUser vendorUser = new VendorUser();
            vendorUser.VendorId = vendor.VendorId;
            vendorUser.UserId = user.UserId;
            vendorUser.VendorRoleId = 1;

            await _vendorUserRepsository.Create(vendorUser);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseRegisterVendorDTO>(vendor);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}