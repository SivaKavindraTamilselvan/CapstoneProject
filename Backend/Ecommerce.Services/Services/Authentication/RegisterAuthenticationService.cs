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
        User user = _mapper.Map<User>(requestRegisterUserDTO); //authentication mapper
        HMACSHA256 hMACSHA256 = new HMACSHA256();
        user.Password = hMACSHA256.ComputeHash(Encoding.UTF32.GetBytes(requestRegisterUserDTO.Password));
        user.HashedKey = hMACSHA256.Key;
        user.RoleId = RoleId;
        var createdUser = await _userRepsository.Create(user);
        if (createdUser == null)
        {
            throw new DataRegistrationException($"Registration for User with the Email {user.Email} failed");
        }
        return _mapper.Map<ResponseRegisterUserDTO>(createdUser); // authentication mapper
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
            return _mapper.Map<ResponseRegisterUserDTO>(user); // authentication mapper

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
        User user;
        string token;
        try
        {
            _logger.LogInformation("User registration started for {Email}", requestRegisterAdminDTO.requestRegisterUserDTO.Email);

            user = await RegisterUserWithoutPassword(requestRegisterAdminDTO.requestRegisterUserDTO, (int)RoleEnum.Admin);

            var assignedAdmin = await _adminRepository.GetAdminUserByUserId(adminUserId);
            if (assignedAdmin == null)
            {
                _logger.LogError("Assigning Admin Id {AdminUserId} Not found", adminUserId);
                throw new DataNotFoundException("Assigning Admin User not found");
            }

            AdminUser adminUser = new AdminUser
            {
                AdminRoleId = requestRegisterAdminDTO.AdminRoleId,
                UserId = user.UserId,
                AssignedByAdminUserId = assignedAdmin.AdminUserId
            };

            var createdAdminUser = await _adminRepository.Create(adminUser);
            if (createdAdminUser == null)
            {
                _logger.LogError("Registration for Admin User with the Email {userEmail} failed", user.Email);
                throw new DataRegistrationException($"Registration for Admin User with the Email {user.Email} failed");
            }

            token = Guid.NewGuid().ToString("N");
            await _passwordSetTokenRepsository.Create(new PasswordSetToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(48),
                IsUsed = false
            });

            await transaction.CommitAsync();

            var createdadmin = _mapper.Map<ResponseRegisterAdminDTO>(createdAdminUser);
            createdadmin.Email = user.Email;

            _logger.LogInformation("Admin registered successfully with UserId {UserId}", user.UserId);

            try
            {
                await _emailService.SendSetPasswordEmailAsync(user.Email, user.FirstName, token);
            }
            catch (Exception emailEx)
            {
                _logger.LogError(emailEx, "Admin created but invite email failed to send to {Email}", user.Email);
            }

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
        User user;
        string token;
        try
        {
            _logger.LogInformation("User registration started for {Email}", requestRegisterVendorDTO.requestRegisterUserDTO.Email);
            await _registrationValidation.ValidateVendorDetails(requestRegisterVendorDTO);
            user = await RegisterUserWithoutPassword(requestRegisterVendorDTO.requestRegisterUserDTO, (int)RoleEnum.Vendor);
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
            token = Guid.NewGuid().ToString("N");
            await _passwordSetTokenRepsository.Create(new PasswordSetToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(48),
                IsUsed = false
            });
            await transaction.CommitAsync();
            _logger.LogInformation("Vendor registered successfully with UserId {UserId}", user.UserId);
            var responseVendor = _mapper.Map<ResponseRegisterVendorDTO>(createdVendor);
            try
            {
                await _emailService.SendSetPasswordEmailAsync(user.Email, user.FirstName, token);
            }
            catch (Exception emailEx)
            {
                _logger.LogError(emailEx, "Vendor created but invite email failed to send to {Email}", user.Email);
            }
            return responseVendor;
        }
        catch
        {
            _logger.LogError("User registration failed for {Email}", requestRegisterVendorDTO.requestRegisterUserDTO.Email);
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<User> RegisterUserWithoutPassword(RequestRegisterUserDTO requestRegisterUserDTO, int roleId)
    {
        await _registrationValidation.ValidateUserDetails(requestRegisterUserDTO);
        User user = _mapper.Map<User>(requestRegisterUserDTO);

        var hmac = new HMACSHA256();
        user.Password = hmac.ComputeHash(Encoding.UTF32.GetBytes(Guid.NewGuid().ToString()));
        user.HashedKey = hmac.Key;
        user.RoleId = roleId;
        user.IsPasswordSet = false;

        var createdUser = await _userRepsository.Create(user);
        if (createdUser == null)
        {
            throw new DataRegistrationException($"Registration for User with the Email {user.Email} failed");
        }
        return createdUser;
    }
    public async Task<ResponseSetPasswordDTO> SetPassword(RequestSetPasswordDTO requestSetPasswordDTO)
    {
        var tokenEntity = await _passwordSetTokenRepsository.GetByToken(requestSetPasswordDTO.Token);

        if (tokenEntity == null)
        {
            throw new InvalidTokenException("Invalid or unrecognized password set token");
        }
        if (tokenEntity.IsUsed)
        {
            throw new InvalidTokenException("This password set link has already been used");
        }
        if (tokenEntity.ExpiresAt < DateTime.UtcNow)
        {
            throw new TokenExpiredException("This password set link has expired. Please request a new one");
        }

        var user = tokenEntity.User;

        var hmac = new HMACSHA256();
        user.Password = hmac.ComputeHash(Encoding.UTF32.GetBytes(requestSetPasswordDTO.NewPassword));
        user.HashedKey = hmac.Key;
        user.IsPasswordSet = true;

        await _userRepsository.Update(user.UserId, user); // adjust to whatever your update method is named
        await _passwordSetTokenRepsository.MarkAsUsed(tokenEntity.PasswordSetTokenId);

        return new ResponseSetPasswordDTO
        {
            Email = user.Email,
            Message = "Password set successfully. You can now log in."
        };
    }
}