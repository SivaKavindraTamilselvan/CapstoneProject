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

namespace EcommerceTest;

public class AuthenticationServiceTest
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

    private IMapper _mapper = null!;

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

            cfg.CreateMap<RequestRegisterVendorDTO, Vendor>();
            cfg.CreateMap<Vendor, ResponseRegisterVendorDTO>();

            cfg.CreateMap<AdminUser, ResponseRegisterAdminDTO>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();

        var logger = new Mock<ILogger<AuthenticationService>>();
        var tokenService = new Mock<ITokenService>();

        _authenticationService = new AuthenticationService(
            _registrationValidation,
            _context,
            _userRepository,
            _adminRepository,
            _vendorRepository,
            _vendorUserRepository,
            tokenService.Object,
            logger.Object,
            _mapper,
            _cartRepository,
            _favoriteRepository
        );
    }

    [Test]
    public async Task RegisterUser_ShouldCreateUser_WithGivenRole()
    {
        var request = new RequestRegisterUserDTO
        {
            FirstName = "Siva",
            LastName = "Kavindra",
            Email = "siva@test.com",
            PhoneNumber = "9876543210",
            Password = "Test@123"
        };

        var result = await _authenticationService.RegisterUser(request, (int)RoleEnum.User);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo("siva@test.com"));

        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == "siva@test.com");

        Assert.That(user, Is.Not.Null);
        Assert.That(user!.RoleId, Is.EqualTo((int)RoleEnum.User));
        Assert.That(user.Password, Is.Not.Null);
        Assert.That(user.HashedKey, Is.Not.Null);
    }

    [Test]
    public async Task Register_ShouldCreateUserCartAndFavorites()
    {
        var request = new RequestRegisterUserDTO
        {
            FirstName = "Siva",
            LastName = "Kavindra",
            Email = "siva@test.com",
            PhoneNumber = "9876543210",
            Password = "Test@123"
        };

        var result = await _authenticationService.Register(request);

        Assert.That(result, Is.Not.Null);

        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == "siva@test.com");
        Assert.That(user, Is.Not.Null);
        Assert.That(user!.RoleId, Is.EqualTo((int)RoleEnum.User));

        var cart = await _context.Cart.FirstOrDefaultAsync(c => c.UserId == user.UserId);
        Assert.That(cart, Is.Not.Null);

        var favorites = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.UserId);
        Assert.That(favorites, Is.Not.Null);
    }
    [Test]
    public async Task Register_ShouldThrowException_WhenEmailAlreadyExists()
    {
        var request = new RequestRegisterUserDTO
        {
            FirstName = "Siva",
            LastName = "Kavindra",
            Email = "siva@test.com",
            PhoneNumber = "9876543210",
            Password = "Test@123"
        };

        await _authenticationService.Register(request);

        var ex = Assert.ThrowsAsync<DataAlreadyRegisteredException>(async () =>
            await _authenticationService.Register(request));

        Assert.That(ex!.Message, Is.EqualTo("User Already Registered With The Email."));
    }

    [Test]
    public async Task Register_ShouldThrowException_WhenPhoneNumberAlreadyExists()
    {
        var request1 = new RequestRegisterUserDTO
        {
            FirstName = "Siva",
            LastName = "Kavindra",
            Email = "siva1@test.com",
            PhoneNumber = "9876543210",
            Password = "Test@123"
        };

        var request2 = new RequestRegisterUserDTO
        {
            FirstName = "Ravi",
            LastName = "Kumar",
            Email = "ravi@test.com",
            PhoneNumber = "9876543210",
            Password = "Test@123"
        };

        await _authenticationService.Register(request1);

        var ex = Assert.ThrowsAsync<DataAlreadyRegisteredException>(async () =>
            await _authenticationService.Register(request2));

        Assert.That(ex!.Message, Is.EqualTo("User Already Registered With The PhoneNumber."));
    }

    [Test]
    public async Task RegisterAdmin_ShouldCreateAdminUser()
    {
        _context.AdminRoles.Add(new AdminRole { AdminRoleId = 1, AdminRoleName = "SuperAdmin" });
        await _context.SaveChangesAsync();
        var existingUser = new User
        {
            FirstName = "Main",
            LastName = "Admin",
            Email = "mainadmin@test.com",
            PhoneNumber = "1111111111",
            Password = new byte[] { 1 },
            HashedKey = new byte[] { 1 },
            RoleId = (int)RoleEnum.Admin
        };

        var createdUser = await _userRepository.Create(existingUser);

        var existingAdmin = new AdminUser
        {
            UserId = createdUser!.UserId,
            AdminRoleId = 1,
            IsActive = true
        };

        var createdAdmin = await _adminRepository.Create(existingAdmin);

        var directAdmin = await _context.AdminUser
            .FirstOrDefaultAsync(a => a.UserId == createdUser.UserId && a.IsActive);

        Assert.That(directAdmin, Is.Not.Null);
        var request = new RequestRegisterAdminDTO
        {
            AdminRoleId = 1,
            requestRegisterUserDTO = new RequestRegisterUserDTO
            {
                FirstName = "New",
                LastName = "Admin",
                Email = "newadmin@test.com",
                PhoneNumber = "2222222222",
                Password = "Test@123"
            }
        };
        var result = await _authenticationService.RegisterAdmin(
            request,
            createdUser.UserId
        );

    }
    [Test]
    public async Task RegisterAdmin_ShouldThrowException_WhenAssignedAdminNotFound()
    {
        var request = new RequestRegisterAdminDTO
        {
            AdminRoleId = 1,
            requestRegisterUserDTO = new RequestRegisterUserDTO
            {
                FirstName = "New",
                LastName = "Admin",
                Email = "newadmin@test.com",
                PhoneNumber = "2222222222",
                Password = "Test@123"
            }
        };

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _authenticationService.RegisterAdmin(request, 999));

        Assert.That(ex!.Message, Is.EqualTo("Assining Admin User not found"));
    }

    [Test]
    public async Task RegisterVendor_ShouldCreateVendorAndVendorUser()
    {
        var request = new RequestRegisterVendorDTO
        {
            requestRegisterUserDTO = new RequestRegisterUserDTO
            {
                FirstName = "Vendor",
                LastName = "Owner",
                Email = "vendor@test.com",
                PhoneNumber = "3333333333",
                Password = "Test@123"
            },

            VendorCompanyName = "Siva Stores",
            ContactPersonName = "Siva",
            CompanyEmail = "company@test.com",
            CompanyPhoneNumber = "9999999999",
            GSTNumber = "GST123456"
        };

        var result = await _authenticationService.RegisterVendor(request);

        Assert.That(result, Is.Not.Null);

        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == "vendor@test.com");
        Assert.That(user, Is.Not.Null);
        Assert.That(user!.RoleId, Is.EqualTo((int)RoleEnum.Vendor));

        var vendor = await _context.Vendor.FirstOrDefaultAsync(v => v.VendorCompanyName == "Siva Stores");
        Assert.That(vendor, Is.Not.Null);

        var vendorUser = await _context.VendorUser
            .FirstOrDefaultAsync(vu => vu.UserId == user.UserId && vu.VendorId == vendor!.VendorId);

        Assert.That(vendorUser, Is.Not.Null);
        Assert.That(vendorUser!.VendorRoleId, Is.EqualTo((int)RoleEnum.VendorOwner));
    }
    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

}