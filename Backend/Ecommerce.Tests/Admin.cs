using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace EcommerceTest;

public class AdminServiceTests
{
    private Mock<IAdminUserValidation> _adminUserValidation = null!;
    private Mock<IAuthentication> _authentication = null!;
    private Mock<IAdminUserRepsository> _adminUserRepo = null!;
    private Mock<IUserRepsository> _userRepo = null!;
    private Mock<ILogger<AdminService>> _logger = null!;
    private IMapper _mapper = null!;
    private EcommerceContext _context = null!;
    private AdminService _sut = null!;

    // ── seed helpers ─────────────────────────────────────────────────────────
    private static AdminUser MakeAdminUser(int adminUserId = 1, int userId = 10, bool isActive = true) =>
        new() { AdminUserId = adminUserId, UserId = userId, IsActive = isActive };

    private static User MakeUser(int userId = 10, bool isActive = true) =>
        new() { UserId = userId, IsActive = isActive };

    // =========================================================================
    // Setup
    // =========================================================================
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new EcommerceContext(options);

        _adminUserValidation = new Mock<IAdminUserValidation>();
        _authentication = new Mock<IAuthentication>();
        _adminUserRepo = new Mock<IAdminUserRepsository>();
        _userRepo = new Mock<IUserRepsository>();
        _logger = new Mock<ILogger<AdminService>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<AdminUser, ResponseGetAdminUserDTO>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();

        _sut = new AdminService(
            _adminUserValidation.Object,
            _context,
            _userRepo.Object,
            _authentication.Object,
            _mapper,
            _logger.Object,
            _adminUserRepo.Object);

        var adminUser = new AdminUser
{
    AdminUserId = 10,
    UserId = 99
};

        // default: validation always passes unless overridden per test
        _adminUserValidation
            .Setup(v => v.ValidateAdminUserByUserId(10))
            .ReturnsAsync(adminUser);
    }

    // =========================================================================
    // TearDown
    // =========================================================================
    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    // =========================================================================
    // GetAllAdminUser
    // =========================================================================

    [Test]
    public async Task GetAllAdminUser_ShouldReturnPagedResponse_WhenUsersExist()
    {
        var request = new RequestAdiminUserFilter { PageNumber = 1, PageSize = 10 };
        var adminUser = MakeAdminUser();
        var repoResult = (items: new List<AdminUser> { adminUser }, totalCount: 1);

        _adminUserRepo
            .Setup(r => r.GetAllAdminUser(request))
            .ReturnsAsync(repoResult);

        var result = await _sut.GetAllAdminUser(request, logedusedId: 99);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.PageNumber, Is.EqualTo(request.PageNumber));
        Assert.That(result.PageSize, Is.EqualTo(request.PageSize));
    }

    [Test]
    public async Task GetAllAdminUser_ShouldThrowDataNotFoundException_WhenNoUsersExist()
    {
        var request = new RequestAdiminUserFilter { PageNumber = 1, PageSize = 10 };
        var repoResult = (items: new List<AdminUser>(), totalCount: 0);

        _adminUserRepo
            .Setup(r => r.GetAllAdminUser(request))
            .ReturnsAsync(repoResult);

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.GetAllAdminUser(request, logedusedId: 99));

        Assert.That(ex!.Message, Is.EqualTo("No Admin User Found"));
    }

    [Test]
    public async Task GetAllAdminUser_ShouldThrow_WhenValidationFails()
    {
        _adminUserValidation
            .Setup(v => v.ValidateAdminUserByUserId(It.IsAny<int>()))
            .ThrowsAsync(new DataNotFoundException("Admin Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.GetAllAdminUser(new RequestAdiminUserFilter(), logedusedId: 0));

        Assert.That(ex!.Message, Is.EqualTo("Admin Not Found"));
    }

    // =========================================================================
    // GetAdminUserByUserId
    // =========================================================================

    [Test]
    public async Task GetAdminUserByUserId_ShouldReturnMappedDto_WhenFound()
    {
        var adminUser = MakeAdminUser();

        _adminUserRepo
            .Setup(r => r.GetAdminUserByAdminUserId(1))
            .ReturnsAsync(adminUser);

        var result = await _sut.GetAdminUserByUserId(userId: 1, logedusedId: 99);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.AdminUserId, Is.EqualTo(adminUser.AdminUserId));
    }

    [Test]
    public async Task GetAdminUserByUserId_ShouldThrowDataNotFoundException_WhenNotFound()
    {
        _adminUserRepo
            .Setup(r => r.GetAdminUserByAdminUserId(It.IsAny<int>()))
            .ReturnsAsync((AdminUser?)null);

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.GetAdminUserByUserId(userId: 99, logedusedId: 1));

        Assert.That(ex!.Message, Is.EqualTo("Admin User Not Found"));
    }

    [Test]
    public async Task GetAdminUserByUserId_ShouldThrow_WhenValidationFails()
    {
        _adminUserValidation
            .Setup(v => v.ValidateAdminUserByUserId(It.IsAny<int>()))
            .ThrowsAsync(new DataNotFoundException("Admin Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.GetAdminUserByUserId(userId: 1, logedusedId: 0));

        Assert.That(ex!.Message, Is.EqualTo("Admin Not Found"));
    }

    // =========================================================================
    // DeactivateAdminUser
    // =========================================================================

    [Test]
    public async Task DeactivateAdminUser_ShouldDeactivateBothRecords_WhenValid()
    {
        var adminUser = MakeAdminUser(adminUserId: 1, userId: 10, isActive: true);
        var deactivatedAdmin = MakeAdminUser(adminUserId: 1, userId: 10, isActive: false);
        var user = MakeUser(userId: 10, isActive: true);

        _adminUserRepo.Setup(r => r.GetAdminUserByAdminUserId(1)).ReturnsAsync(adminUser);
        _adminUserRepo.Setup(r => r.Update(1, It.IsAny<AdminUser>())).ReturnsAsync(deactivatedAdmin);
        _userRepo.Setup(r => r.Get(10)).ReturnsAsync(user);
        _userRepo.Setup(r => r.Update(10, It.IsAny<User>())).ReturnsAsync(user);

        var result = await _sut.DeactivateAdminUser(adminUserId: 1, logedusedId: 99);

        Assert.That(result, Is.Not.Null);

        _adminUserRepo.Verify(r => r.Update(1, It.Is<AdminUser>(u => !u.IsActive)), Times.Once);
        _userRepo.Verify(r => r.Update(10, It.Is<User>(u => !u.IsActive)), Times.Once);
    }

    [Test]
    public async Task DeactivateAdminUser_ShouldThrowDataNotFoundException_WhenAdminUserNotFound()
    {
        _adminUserRepo
            .Setup(r => r.GetAdminUserByAdminUserId(It.IsAny<int>()))
            .ReturnsAsync((AdminUser?)null);

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.DeactivateAdminUser(adminUserId: 99, logedusedId: 1));

        Assert.That(ex!.Message, Is.EqualTo("Admin User Not Found"));
    }

    [Test]
    public async Task DeactivateAdminUser_ShouldThrowDataApprovalStatusException_WhenAlreadyInactive()
    {
        var inactiveAdmin = MakeAdminUser(isActive: false);

        _adminUserRepo
            .Setup(r => r.GetAdminUserByAdminUserId(1))
            .ReturnsAsync(inactiveAdmin);

        var ex = Assert.ThrowsAsync<DataApprovalStatusException>(async () =>
            await _sut.DeactivateAdminUser(adminUserId: 1, logedusedId: 99));

        Assert.That(ex!.Message, Is.EqualTo("Admin User is already deactivated"));
        _adminUserRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<AdminUser>()), Times.Never);
    }

    [Test]
    public async Task DeactivateAdminUser_ShouldThrowDataRegistrationException_WhenUpdateReturnsNull()
    {
        var adminUser = MakeAdminUser(isActive: true);

        _adminUserRepo.Setup(r => r.GetAdminUserByAdminUserId(1)).ReturnsAsync(adminUser);
        _adminUserRepo
            .Setup(r => r.Update(It.IsAny<int>(), It.IsAny<AdminUser>()))
            .ReturnsAsync((AdminUser?)null);

        var ex = Assert.ThrowsAsync<DataRegistrationException>(async () =>
            await _sut.DeactivateAdminUser(adminUserId: 1, logedusedId: 99));

        Assert.That(ex!.Message, Is.EqualTo("Failed to deactivate Admin User"));
    }

    [Test]
    public async Task DeactivateAdminUser_ShouldThrowDataNotFoundException_WhenLinkedUserNotFound()
    {
        var adminUser = MakeAdminUser(adminUserId: 1, userId: 10, isActive: true);
        var deactivatedAdmin = MakeAdminUser(adminUserId: 1, userId: 10, isActive: false);

        _adminUserRepo.Setup(r => r.GetAdminUserByAdminUserId(1)).ReturnsAsync(adminUser);
        _adminUserRepo.Setup(r => r.Update(1, It.IsAny<AdminUser>())).ReturnsAsync(deactivatedAdmin);
        _userRepo.Setup(r => r.Get(10)).ReturnsAsync((User?)null);

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.DeactivateAdminUser(adminUserId: 1, logedusedId: 99));

        Assert.That(ex!.Message, Is.EqualTo("User Not Found"));
        _userRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task DeactivateAdminUser_ShouldRollback_WhenExceptionThrown()
    {
        _adminUserValidation
            .Setup(v => v.ValidateAdminUserByUserId(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _sut.DeactivateAdminUser(adminUserId: 1, logedusedId: 99));

        Assert.That(ex!.Message, Is.EqualTo("Unexpected error"));
        Assert.That(_context.AdminUser.Any(), Is.False);
    }

    // =========================================================================
    // RegisterAdmin
    // =========================================================================

    [Test]
    public async Task RegisterAdmin_ShouldReturnDto_WhenValid()
    {
        var request = new RequestRegisterAdminDTO
        {
            requestRegisterUserDTO = new RequestRegisterUserDTO { Email = "admin@test.com" }
        };
        var expectedDto = new ResponseRegisterAdminDTO { UserId = 5 };

        _authentication
            .Setup(a => a.RegisterAdmin(request, 99))
            .ReturnsAsync(expectedDto);

        var result = await _sut.RegisterAdmin(request, adminUserId: 99);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(5));

        _authentication.Verify(a => a.RegisterAdmin(request, 99), Times.Once);
    }

    [Test]
    public async Task RegisterAdmin_ShouldThrow_WhenValidationFails()
    {
        _adminUserValidation
            .Setup(v => v.ValidateAdminUserByUserId(It.IsAny<int>()))
            .ThrowsAsync(new DataNotFoundException("Admin Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.RegisterAdmin(new RequestRegisterAdminDTO(), adminUserId: 0));

        Assert.That(ex!.Message, Is.EqualTo("Admin Not Found"));
    }

    [Test]
    public async Task RegisterAdmin_ShouldThrow_WhenAuthenticationFails()
    {
        _authentication
            .Setup(a => a.RegisterAdmin(It.IsAny<RequestRegisterAdminDTO>(), It.IsAny<int>()))
            .ThrowsAsync(new DataRegistrationException("Registration failed"));

        var ex = Assert.ThrowsAsync<DataRegistrationException>(async () =>
            await _sut.RegisterAdmin(new RequestRegisterAdminDTO(), adminUserId: 99));

        Assert.That(ex!.Message, Is.EqualTo("Registration failed"));
    }
}