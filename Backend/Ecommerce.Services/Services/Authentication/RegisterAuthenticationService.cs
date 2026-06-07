using System.Security.Cryptography;
using System.Text;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AuthenticationService : IAuthentication
{
    public async Task<ResponseRegisterUserDTO> RegisterUser(RequestRegisterUserDTO requestRegisterUserDTO, int RoleId)
    {
        var existingUser = await _userRepsository.GetUserByEmail(requestRegisterUserDTO.Email);
        if (existingUser != null)
        {
            throw new Exception("Email already found");
        }
        User user = _mapper.Map<User>(requestRegisterUserDTO);
        HMACSHA256 hMACSHA256 = new HMACSHA256();
        user.Password = hMACSHA256.ComputeHash(Encoding.UTF32.GetBytes(requestRegisterUserDTO.Password));
        user.HashedKey = hMACSHA256.Key;
        user.RoleId = RoleId;
        await _userRepsository.Create(user);
        Cart cart = new Cart();
        cart.UserId = user.UserId;
        await _cartRepsository.Create(cart);
        Favorites favorites = new Favorites();
        favorites.UserId = user.UserId;
        await _favoriteRepsository.Create(favorites);

        return _mapper.Map<ResponseRegisterUserDTO>(user);
    }

    public async Task<ResponseRegisterUserDTO> Register(RequestRegisterUserDTO requestRegisterUserDTO)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("User registration started for {Email}", requestRegisterUserDTO.Email);
            var user = await RegisterUser(requestRegisterUserDTO, 3);
            _logger.LogInformation("User registered successfully with UserId {UserId}", user.UserId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseRegisterUserDTO>(user);

        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            var user = await RegisterUser(requestRegisterAdminDTO.requestRegisterUserDTO, 1);
            var assignedAdmin = (await _adminRepository.GetAll()).FirstOrDefault(u => u.AdminUserId == adminUserId);
            if (assignedAdmin == null)
            {
                throw new Exception("Admin not found");
            }
            AdminUser adminUser = new AdminUser();
            adminUser.AdminRoleId = requestRegisterAdminDTO.AdminRoleId;
            adminUser.UserId = user.UserId;
            adminUser.AssignedByAdminUserId = assignedAdmin.AdminUserId;
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
            var user = await RegisterUser(requestRegisterVendorDTO.requestRegisterUserDTO, 2);
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