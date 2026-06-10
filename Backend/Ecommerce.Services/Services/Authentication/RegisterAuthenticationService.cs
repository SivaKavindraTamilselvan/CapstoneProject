using System.Security.Cryptography;
using System.Text;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AuthenticationService : IAuthentication
{
    public async Task<ResponseRegisterUserDTO> RegisterUser(RequestRegisterUserDTO requestRegisterUserDTO, int RoleId)
    {
        await _registrationValidation.ValidateUserDetails(requestRegisterUserDTO);
        User user = _mapper.Map<User>(requestRegisterUserDTO);
        HMACSHA256 hMACSHA256 = new HMACSHA256();
        user.Password = hMACSHA256.ComputeHash(Encoding.UTF32.GetBytes(requestRegisterUserDTO.Password));
        user.HashedKey = hMACSHA256.Key;
        user.RoleId = RoleId;
        var createdUser = await _userRepsository.Create(user);
        if (createdUser == null)
        {
            throw new DataRegistrationException($"Registration for User with the Email {user.Email} failed");
        }
        return _mapper.Map<ResponseRegisterUserDTO>(createdUser);
    }

    public async Task<ResponseRegisterUserDTO> Register(RequestRegisterUserDTO requestRegisterUserDTO)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("User registration started for {Email}", requestRegisterUserDTO.Email);
            var user = await RegisterUser(requestRegisterUserDTO, (int)RoleEnum.User);
            Cart cart = new Cart();
            cart.UserId = user.UserId;
            var createdCart = await _cartRepsository.Create(cart);
            if (createdCart == null)
            {
                _logger.LogError("Cart Creation Failed for User {UserID}", user.UserId);
                throw new DataRegistrationException($"Cart Registration for User with the Email {user.Email} failed");
            }
            Favorites favorites = new Favorites();
            favorites.UserId = user.UserId;
            var createdFavorite = await _favoriteRepsository.Create(favorites);
            if (createdFavorite == null)
            {
                _logger.LogError("Favorite Creation Failed for User {UserID}", user.UserId);
                throw new DataRegistrationException($"Favorite Registration for User with the Email {user.Email} failed");
            }
            _logger.LogInformation("User registered successfully with UserId {UserId}", user.UserId);
            await transaction.CommitAsync();
            return _mapper.Map<ResponseRegisterUserDTO>(user);

        }
        catch
        {
            _logger.LogError("User registered failed for {Email}", requestRegisterUserDTO.Email);
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("User registration started for {Email}", requestRegisterAdminDTO.requestRegisterUserDTO.Email);
            var user = await RegisterUser(requestRegisterAdminDTO.requestRegisterUserDTO, (int)RoleEnum.Admin);
            var assignedAdmin = await _adminRepository.GetAdminUserByUserId(adminUserId);
            if (assignedAdmin == null)
            {
                _logger.LogError("Assigning Admin Id {AdminUserId} Not found", adminUserId);
                throw new DataNotFoundException("Assining Admin User not found");
            }
            AdminUser adminUser = new AdminUser();
            adminUser.AdminRoleId = requestRegisterAdminDTO.AdminRoleId;
            adminUser.UserId = user.UserId;
            adminUser.AssignedByAdminUserId = assignedAdmin.AdminUserId;
            var createdAdminUser = await _adminRepository.Create(adminUser);
            if (createdAdminUser == null)
            {
                _logger.LogError("Registration for Admin User with the Email {userEmail} failed", user.Email);
                throw new DataRegistrationException($"Registration for Admin User with the Email {user.Email} failed");
            }
            await transaction.CommitAsync();
            _logger.LogInformation("User registered successfully with UserId {UserId}", user.UserId);
            var createdadmin =  _mapper.Map<ResponseRegisterAdminDTO>(createdAdminUser);
            createdadmin.Email = user.Email;
            return createdadmin;
        }
        catch
        {
            _logger.LogError("User registration failed for {Email}", requestRegisterAdminDTO.requestRegisterUserDTO.Email);
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<ResponseRegisterVendorDTO> RegisterVendor(RequestRegisterVendorDTO requestRegisterVendorDTO)
    {
        using var transaction = await _ecommerceContext.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("User registration started for {Email}", requestRegisterVendorDTO.requestRegisterUserDTO.Email);
            await _registrationValidation.ValidateVendorDetails(requestRegisterVendorDTO);
            var user = await RegisterUser(requestRegisterVendorDTO.requestRegisterUserDTO, (int)RoleEnum.Vendor);
            var vendor = _mapper.Map<Vendor>(requestRegisterVendorDTO);
            var createdVendor = await _vendorRepsository.Create(vendor);
            if (createdVendor == null)
            {
                _logger.LogError("Registration for Vendor With Company name {companyname} failed", vendor.VendorCompanyName);
                throw new DataRegistrationException($"Registration for vendor failed");
            }
            VendorUser vendorUser = new VendorUser();
            vendorUser.VendorId = createdVendor.VendorId;
            vendorUser.UserId = user.UserId;
            vendorUser.VendorRoleId = (int)RoleEnum.VendorOwner;
            var createdVendorUser = await _vendorUserRepsository.Create(vendorUser);
            if (createdVendorUser == null)
            {
                _logger.LogError("Registration for Vendor User with the Email {userEmail} failed", user.Email);
                throw new DataRegistrationException($"Registration for vendor User with the Email {user.Email} failed");
            }
            await transaction.CommitAsync();
            _logger.LogInformation("User registered successfully with UserId {UserId}", user.UserId);
            return _mapper.Map<ResponseRegisterVendorDTO>(createdVendor);
        }
        catch
        {
            _logger.LogError("User registration failed for {Email}", requestRegisterVendorDTO.requestRegisterUserDTO.Email);
            await transaction.RollbackAsync();
            throw;
        }
    }
}