using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace EcommerceTest;

public class LoginAuthenticationServiceTest
{
    private EcommerceContext _context = null!;
    private IAuthentication _authenticationService = null!;

    private IUserRepsository _userRepository = null!;
    private IAdminUserRepsository _adminRepository = null!;
    private IVendorRepsository _vendorRepository = null!;
    private IVendorUserRepsository _vendorUserRepository = null!;
    private ICartRepsository _cartRepository = null!;
    private IFavoriteRepsository _favoriteRepository = null!;
    private IRegistrationValidation _registrationValidation = null!;
    private Mock<ITokenService> _tokenService = null!;
    private IMapper _mapper = null!;

    private static (byte[] password, byte[] hashedKey) HashPassword(string plain)
    {
        using var hmac = new HMACSHA256();
        return (hmac.ComputeHash(Encoding.UTF32.GetBytes(plain)), hmac.Key);
    }

    private async Task<User> SeedUser(string email, string phone, string plainPassword, int roleId, bool isActive = true)
    {
        var (pwd, key) = HashPassword(plainPassword);
        var user = new User
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            PhoneNumber = phone,
            Password = pwd,
            HashedKey = key,
            RoleId = roleId,
            IsActive = isActive
        };
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<EcommerceContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
        .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;

        _context = new EcommerceContext(options);

        _userRepository = new UserRepsository(_context);
        _adminRepository = new AdminRepsository(_context);
        _vendorRepository = new VendorRepsository(_context);
        _vendorUserRepository = new VendorUserRepsository(_context);
        _cartRepository = new CartRepsository(_context);
        _favoriteRepository = new FavoriteRepsository(_context);

        _registrationValidation = new RegsitrationValidation(
            _vendorRepository,
            _userRepository
        );

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<RequestRegisterUserDTO, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.HashedKey, opt => opt.Ignore());

            cfg.CreateMap<User, ResponseRegisterUserDTO>();
            cfg.CreateMap<User, ResponseLoginUserDTO>();
            cfg.CreateMap<User, TokenRequest>();

            cfg.CreateMap<RequestRegisterVendorDTO, Vendor>();
            cfg.CreateMap<Vendor, ResponseRegisterVendorDTO>();
            cfg.CreateMap<AdminUser, ResponseRegisterAdminDTO>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();

        _tokenService = new Mock<ITokenService>();
        _tokenService
        .Setup(t => t.CreateNewToken(It.IsAny<TokenRequest>()))
        .Returns("mock-jwt-token");

        var logger = new Mock<ILogger<AuthenticationService>>();

        _authenticationService = new AuthenticationService(
            _registrationValidation,
            _context,
            _userRepository,
            _adminRepository,
            _vendorRepository,
            _vendorUserRepository,
            _tokenService.Object,
            logger.Object,
            _mapper,
            _cartRepository,
            _favoriteRepository
        );
    }
    [Test]
    public async Task Login_ShouldReturnToken_WhenUserCredentialsAreValid()
    {
        await SeedUser("siva@test.com", "9876543210", "Test@123", (int)RoleEnum.User);

        var request = new RequestLoginUserDTO
        {
            Email = "siva@test.com",
            Password = "Test@123"
        };

        var result = await _authenticationService.Login(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo("mock-jwt-token"));
        Assert.That(result.Email, Is.EqualTo("siva@test.com"));
    }

    [Test]
    public async Task Login_ShouldCallCreateNewToken_WhenLoginSucceeds()
    {
        await SeedUser("siva@test.com", "9876543210", "Test@123", (int)RoleEnum.User);

        var request = new RequestLoginUserDTO
        {
            Email = "siva@test.com",
            Password = "Test@123"
        };

        await _authenticationService.Login(request);

        _tokenService.Verify(t => t.CreateNewToken(It.IsAny<TokenRequest>()), Times.Once);
    }

    [Test]
    public async Task Login_ShouldSucceed_ForAdminUser_WithAdminRoleId()
    {
        _context.AdminRoles.Add(new AdminRole { AdminRoleId = 1, AdminRoleName = "SuperAdmin" });
        await _context.SaveChangesAsync();
        var user = await SeedUser("admin@test.com", "1111111111", "Admin@123", (int)RoleEnum.Admin);

        _context.AdminUser.Add(new AdminUser
        {
            UserId = user.UserId,
            AdminRoleId = 1,
            IsActive = true
        });
        await _context.SaveChangesAsync();

        var request = new RequestLoginUserDTO
        {
            Email = "admin@test.com",
            Password = "Admin@123"
        };

        var result = await _authenticationService.Login(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo("mock-jwt-token"));
    }

    [Test]
    public async Task Login_ShouldSucceed_ForVendorUser_WhenVendorIsActive()
    {
        var user = await SeedUser("vendor@test.com", "2222222222", "Vendor@123", (int)RoleEnum.Vendor);

        var vendor = new Vendor
        {
            VendorCompanyName = "Test Stores",
            ContactPersonName = "Test",
            CompanyEmail = "company@test.com",
            CompanyPhoneNumber = "9999999999",
            GSTNumber = "GST123"
        };
        _context.Vendor.Add(vendor);
        await _context.SaveChangesAsync();

        _context.VendorUser.Add(new VendorUser
        {
            UserId = user.UserId,
            VendorId = vendor.VendorId,
            VendorRoleId = (int)RoleEnum.VendorOwner,
            IsActive = true
        });
        await _context.SaveChangesAsync();

        var request = new RequestLoginUserDTO
        {
            Email = "vendor@test.com",
            Password = "Vendor@123"
        };

        var result = await _authenticationService.Login(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Token, Is.EqualTo("mock-jwt-token"));
    }
    [Test]
    public async Task Login_ShouldThrow_WhenEmailNotFound()
    {
        var request = new RequestLoginUserDTO
        {
            Email = "notexist@test.com",
            Password = "Test@123"
        };

        var ex = Assert.ThrowsAsync<InvalidCredentialException>(async () => await _authenticationService.Login(request));

        Assert.That(ex!.Message, Is.EqualTo("Email Not Found"));
    }

    [Test]
    public async Task Login_ShouldThrow_WhenPasswordIsWrong()
    {
        await SeedUser("siva@test.com", "9876543210", "Test@123", (int)RoleEnum.User);

        var request = new RequestLoginUserDTO
        {
            Email = "siva@test.com",
            Password = "WrongPassword"
        };

        var ex = Assert.ThrowsAsync<InvalidCredentialException>(async () =>
            await _authenticationService.Login(request));

        Assert.That(ex!.Message, Is.EqualTo("Invalid password for the email"));
    }

    [Test]
    public async Task Login_ShouldThrow_WhenPasswordIsEmpty()
    {
        await SeedUser("siva@test.com", "9876543210", "Test@123", (int)RoleEnum.User);

        var request = new RequestLoginUserDTO
        {
            Email = "siva@test.com",
            Password = ""
        };

        var ex = Assert.ThrowsAsync<InvalidCredentialException>(async () => await _authenticationService.Login(request));

        Assert.That(ex!.Message, Is.EqualTo("Invalid password for the email"));
    }

    [Test]
    public async Task Login_ShouldThrow_WhenPasswordIsCaseDifferent()
    {
        await SeedUser("siva@test.com", "9876543210", "Test@123", (int)RoleEnum.User);

        var request = new RequestLoginUserDTO
        {
            Email = "siva@test.com",
            Password = "test@123"
        };

        var ex = Assert.ThrowsAsync<InvalidCredentialException>(async () => await _authenticationService.Login(request));

        Assert.That(ex!.Message, Is.EqualTo("Invalid password for the email"));
    }
    [Test]
    public async Task Login_ShouldThrow_WhenUserIsDeactivated()
    {
        await SeedUser("siva@test.com", "9876543210", "Test@123", (int)RoleEnum.User, isActive: false);

        var request = new RequestLoginUserDTO
        {
            Email = "siva@test.com",
            Password = "Test@123"
        };

        var ex = Assert.ThrowsAsync<InvalidCredentialException>(async () => await _authenticationService.Login(request));

        Assert.That(ex!.Message, Is.EqualTo("User Is deleted or deactivated"));
    }

    [Test]
    public async Task Login_ShouldThrow_WhenVendorUserRecordNotFound()
    {
        await SeedUser("vendor@test.com", "2222222222", "Vendor@123", (int)RoleEnum.Vendor);

        var request = new RequestLoginUserDTO
        {
            Email = "vendor@test.com",
            Password = "Vendor@123"
        };

        var ex = Assert.ThrowsAsync<InvalidCredentialException>(async () => await _authenticationService.Login(request));

        Assert.That(ex!.Message, Is.EqualTo("Vendor Credential is not valid"));
    }

    [Test]
    public async Task Login_ShouldThrow_WhenVendorUserIsInactive()
    {
        var user = await SeedUser("vendor@test.com", "2222222222", "Vendor@123", (int)RoleEnum.Vendor);

        var vendor = new Vendor
        {
            VendorCompanyName = "Test Stores",
            ContactPersonName = "Test",
            CompanyEmail = "company@test.com",
            CompanyPhoneNumber = "9999999999",
            GSTNumber = "GST123"
        };
        _context.Vendor.Add(vendor);
        await _context.SaveChangesAsync();

        _context.VendorUser.Add(new VendorUser
        {
            UserId = user.UserId,
            VendorId = vendor.VendorId,
            VendorRoleId = (int)RoleEnum.VendorOwner,
            IsActive = false
        });
        await _context.SaveChangesAsync();

        var request = new RequestLoginUserDTO
        {
            Email = "vendor@test.com",
            Password = "Vendor@123"
        };

        var ex = Assert.ThrowsAsync<InvalidCredentialException>(async () => await _authenticationService.Login(request));

        Assert.That(ex!.Message, Is.EqualTo("Vendor Is deleted"));
    }
    [Test]
    public async Task Login_ShouldThrow_WhenAdminUserRecordNotFound()
    {
        await SeedUser("admin@test.com", "1111111111", "Admin@123", (int)RoleEnum.Admin);

        var request = new RequestLoginUserDTO
        {
            Email = "admin@test.com",
            Password = "Admin@123"
        };

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _authenticationService.Login(request));

        Assert.That(ex!.Message, Is.EqualTo("Admin User Not Found"));
    }

    [Test]
    public async Task Login_ShouldNotCallCreateNewToken_WhenLoginFails()
    {
        var request = new RequestLoginUserDTO
        {
            Email = "ghost@test.com",
            Password = "Test@123"
        };

        Assert.ThrowsAsync<InvalidCredentialException>(async () =>
            await _authenticationService.Login(request));

        _tokenService.Verify(t => t.CreateNewToken(It.IsAny<TokenRequest>()), Times.Never);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}