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

        var userLog = new LogChanges
        {
            TableName = nameof(User),
            RecordId = createdUser.UserId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"UserId={createdUser.UserId}, Email={createdUser.Email}, RoleId={createdUser.RoleId}",
            UserId = createdUser.UserId,
            ChangedAt = DateTime.Now
        };

        var createdUserLog = await _logChanges.Create(userLog);
        if (createdUserLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", userLog.TableName, userLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
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

            var cartLog = new LogChanges
            {
                TableName = nameof(Cart),
                RecordId = createdCart.CartId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"CartId={createdCart.CartId}, UserId={createdCart.UserId}",
                UserId = user.UserId,
                ChangedAt = DateTime.Now
            };
            var createdCartLog = await _logChanges.Create(cartLog);
            if (createdCartLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", cartLog.TableName, cartLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

            Favorites favorites = new Favorites();
            favorites.UserId = user.UserId;
            var createdFavorite = await _favoriteRepsository.Create(favorites);
            if (createdFavorite == null)
            {
                _logger.LogError("Favorite Creation Failed for User {UserID}", user.UserId);
                throw new DataRegistrationException($"Favorite Registration for User with the Email {user.Email} failed");
            }

            var favoritesLog = new LogChanges
            {
                TableName = nameof(Favorites),
                RecordId = createdFavorite.FavoritesId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"FavoritesId={createdFavorite.FavoritesId}, UserId={createdFavorite.UserId}",
                UserId = user.UserId,
                ChangedAt = DateTime.Now
            };
            var createdFavoritesLog = await _logChanges.Create(favoritesLog);
            if (createdFavoritesLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", favoritesLog.TableName, favoritesLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
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

            var adminUserLog = new LogChanges
            {
                TableName = nameof(AdminUser),
                RecordId = createdAdminUser.AdminUserId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"AdminUserId={createdAdminUser.AdminUserId}, UserId={createdAdminUser.UserId}, AdminRoleId={createdAdminUser.AdminRoleId}, AssignedByAdminUserId={createdAdminUser.AssignedByAdminUserId}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };
            var createdAdminUserLog = await _logChanges.Create(adminUserLog);
            if (createdAdminUserLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", adminUserLog.TableName, adminUserLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

            token = Guid.NewGuid().ToString("N");
            var createdToken = await _passwordSetTokenRepsository.Create(new PasswordSetToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(48),
                IsUsed = false
            });
            if (createdToken == null)
            {
                _logger.LogError("Failed to create password set token for UserId {UserId}", user.UserId);
                throw new DataRegistrationException("Password set token creation failed");
            }

            var tokenLog = new LogChanges
            {
                TableName = nameof(PasswordSetToken),
                RecordId = createdToken.PasswordSetTokenId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"PasswordSetTokenId={createdToken.PasswordSetTokenId}, UserId={createdToken.UserId}, ExpiresAt={createdToken.ExpiresAt}",
                UserId = adminUserId,
                ChangedAt = DateTime.Now
            };
            var createdTokenLog = await _logChanges.Create(tokenLog);
            if (createdTokenLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", tokenLog.TableName, tokenLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

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

            var vendorLog = new LogChanges
            {
                TableName = nameof(Vendor),
                RecordId = createdVendor.VendorId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"VendorId={createdVendor.VendorId}, VendorCompanyName={createdVendor.VendorCompanyName}",
                UserId = user.UserId,
                ChangedAt = DateTime.Now
            };
            var createdVendorLog = await _logChanges.Create(vendorLog);
            if (createdVendorLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", vendorLog.TableName, vendorLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
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

            var vendorUserLog = new LogChanges
            {
                TableName = nameof(VendorUser),
                RecordId = createdVendorUser.VendorUserId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"VendorUserId={createdVendorUser.VendorUserId}, VendorId={createdVendorUser.VendorId}, UserId={createdVendorUser.UserId}, VendorRoleId={createdVendorUser.VendorRoleId}",
                UserId = user.UserId,
                ChangedAt = DateTime.Now
            };
            var createdVendorUserLog = await _logChanges.Create(vendorUserLog);
            if (createdVendorUserLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", vendorUserLog.TableName, vendorUserLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

            token = Guid.NewGuid().ToString("N");
            var createdToken = await _passwordSetTokenRepsository.Create(new PasswordSetToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(48),
                IsUsed = false
            });
            if (createdToken == null)
            {
                _logger.LogError("Failed to create password set token for UserId {UserId}", user.UserId);
                throw new DataRegistrationException("Password set token creation failed");
            }

            var tokenLog = new LogChanges
            {
                TableName = nameof(PasswordSetToken),
                RecordId = createdToken.PasswordSetTokenId,
                Actions = (int)AuditAction.Created,
                OldValue = string.Empty,
                NewValue = $"PasswordSetTokenId={createdToken.PasswordSetTokenId}, UserId={createdToken.UserId}, ExpiresAt={createdToken.ExpiresAt}",
                UserId = user.UserId,
                ChangedAt = DateTime.Now
            };
            var createdTokenLog = await _logChanges.Create(tokenLog);
            if (createdTokenLog == null)
            {
                _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", tokenLog.TableName, tokenLog.RecordId);
                throw new DataRegistrationException("Audit log creation failed.");
            }

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

        var userLog = new LogChanges
        {
            TableName = nameof(User),
            RecordId = createdUser.UserId,
            Actions = (int)AuditAction.Created,
            OldValue = string.Empty,
            NewValue = $"UserId={createdUser.UserId}, Email={createdUser.Email}, RoleId={createdUser.RoleId}",
            UserId = createdUser.UserId,
            ChangedAt = DateTime.Now
        };
        var createdUserLog = await _logChanges.Create(userLog);
        if (createdUserLog == null)
        {
            _logger.LogError("Failed to create audit log for TableName {TableName}, RecordId {RecordId}", userLog.TableName, userLog.RecordId);
            throw new DataRegistrationException("Audit log creation failed.");
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

        var updatedUser = await _userRepsository.Update(user.UserId, user); // adjust to whatever your update method is named
        if (updatedUser == null)
        {
            throw new DataRegistrationException($"Password update for User with the Email {user.Email} failed");
        }

        var passwordLog = new LogChanges
        {
            TableName = nameof(User),
            RecordId = updatedUser.UserId,
            Actions = (int)AuditAction.Updated,
            OldValue = $"UserId={user.UserId}, IsPasswordSet=false",
            NewValue = $"UserId={updatedUser.UserId}, IsPasswordSet={updatedUser.IsPasswordSet}",
            UserId = updatedUser.UserId,
            ChangedAt = DateTime.Now
        };
        var createdPasswordLog = await _logChanges.Create(passwordLog);
        if (createdPasswordLog == null)
        {
            throw new DataRegistrationException("Audit log creation failed.");
        }

        await _passwordSetTokenRepsository.MarkAsUsed(tokenEntity.PasswordSetTokenId);

        return new ResponseSetPasswordDTO
        {
            Email = user.Email,
            Message = "Password set successfully. You can now log in."
        };
    }
}