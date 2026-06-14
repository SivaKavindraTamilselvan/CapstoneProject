using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace EcommerceTest;

public class AddressServiceTest
{
    private EcommerceContext _context = null!;

    private IAddressRepsository _addressRepo = null!;
    private IOrderRepsository _orderRepo = null!;
    private IOrderItemRepsository _orderItemRepo = null!;

    private Mock<IVendorUserValidation> _vendorUserValidation = null!;
    private Mock<INotificationService> _notificationService = null!;
    private Mock<IUserValidation> _userValidation = null!;
    private Mock<ILogger<AddressService>> _logger = null!;

    private IMapper _mapper = null!;
    private AddressService _sut = null!;

    private static Address MakeAddress(
        int addressId = 1,
        int userId = 10,
        bool isDefault = false,
        bool isActive = true) =>
        new()
        {
            AddressId = addressId,
            UserId = userId,
            IsDefault = isDefault,
            IsActive = isActive,
            ContactName = "Siva",
            ContactPhoneNumber = "9876543210",
            AddressLine = "Main Street",
            LandMark = "Near Bus Stand",
            City = "Chennai",
            State = "Tamil Nadu",
            Country = "India",
            PinCode = "600001"
        };

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new EcommerceContext(options);

        _addressRepo = new AddressRepsository(_context);
        _orderRepo = new OrderRepsository(_context);
        _orderItemRepo = new OrderItemRepsository(_context);

        _vendorUserValidation = new Mock<IVendorUserValidation>();
        _notificationService = new Mock<INotificationService>();
        _userValidation = new Mock<IUserValidation>();
        _logger = new Mock<ILogger<AddressService>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<RequestAddAddressDTO, Address>();
            cfg.CreateMap<Address, ResponseAddAddressDTO>();
            cfg.CreateMap<Address, ResponseMakeDefaultAddressDTO>();
            cfg.CreateMap<Address, ResponseGetAddressDTO>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();

        _sut = new AddressService(
            _notificationService.Object,
            _vendorUserValidation.Object,
            _orderItemRepo,
            _orderRepo,
            _addressRepo,
            _userValidation.Object,
            _mapper,
            _logger.Object
        );
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task AddAddress_ShouldCreateAddress_InDatabase()
    {
        var user = new User { UserId = 10, FirstName = "Siva", IsActive = true };

        _userValidation
            .Setup(v => v.ValidateUser(10))
            .ReturnsAsync(user);

        var request = new RequestAddAddressDTO
        {
            ContactName = "Siva",
            ContactPhoneNumber = "9876543210",
            AddressLine = "Main Street",
            LandMark = "Near Bus Stand",
            City = "Chennai",
            State = "Tamil Nadu",
            PinCode = "600001",
            IsDefault = false
        };

        var result = await _sut.AddAddress(request, 10);

        Assert.That(result, Is.Not.Null);

        var dbAddress = await _context.Address.FirstOrDefaultAsync();

        Assert.That(dbAddress, Is.Not.Null);
        Assert.That(dbAddress!.UserId, Is.EqualTo(10));
        Assert.That(dbAddress.City, Is.EqualTo("Chennai"));
    }

    [Test]
    public async Task AddAddress_ShouldMakeAddressDefault_WhenIsDefaultTrue()
    {
        var user = new User { UserId = 10, FirstName = "Siva", IsActive = true };

        _userValidation
            .Setup(v => v.ValidateUser(10))
            .ReturnsAsync(user);

        _userValidation
            .Setup(v => v.ValidateAddress(It.IsAny<int>(), 10))
            .ReturnsAsync((int addressId, int userId) =>
                _context.Address.First(a => a.AddressId == addressId && a.UserId == userId));

        var oldAddress = MakeAddress(addressId: 1, userId: 10, isDefault: true);
        _context.Address.Add(oldAddress);
        await _context.SaveChangesAsync();

        var request = new RequestAddAddressDTO
        {
            ContactName = "Siva",
            ContactPhoneNumber = "9876543210",
            AddressLine = "New Street",
            LandMark = "Near School",
            City = "Madurai",
            State = "Tamil Nadu",
            PinCode = "625001",
            IsDefault = true
        };

        await _sut.AddAddress(request, 10);

        var addresses = await _context.Address
            .Where(a => a.UserId == 10)
            .ToListAsync();

        Assert.That(addresses.Count, Is.EqualTo(2));
        Assert.That(addresses.Count(a => a.IsDefault), Is.EqualTo(1));
        Assert.That(addresses.First(a => a.City == "Madurai").IsDefault, Is.True);
        Assert.That(addresses.First(a => a.City == "Chennai").IsDefault, Is.False);
    }

    [Test]
    public async Task MakeAddressDefault_ShouldUpdateDatabase()
    {
        var address1 = MakeAddress(addressId: 1, userId: 10, isDefault: true);
        var address2 = MakeAddress(addressId: 2, userId: 10, isDefault: false);

        _context.Address.AddRange(address1, address2);
        await _context.SaveChangesAsync();

        _userValidation
            .Setup(v => v.ValidateAddress(2, 10))
            .ReturnsAsync(address2);

        var result = await _sut.MakeAddressDefault(2, 10);

        Assert.That(result, Is.Not.Null);

        var dbAddress1 = await _context.Address.FindAsync(1);
        var dbAddress2 = await _context.Address.FindAsync(2);

        Assert.That(dbAddress1!.IsDefault, Is.False);
        Assert.That(dbAddress2!.IsDefault, Is.True);
    }

    [Test]
    public async Task DeleteUserAddress_ShouldDeactivateAddress_InDatabase()
    {
        var address = MakeAddress(addressId: 1, userId: 10, isDefault: false, isActive: true);

        _context.Address.Add(address);
        await _context.SaveChangesAsync();

        _userValidation
            .Setup(v => v.ValidateAddress(1, 10))
            .ReturnsAsync(address);

        var result = await _sut.DeleteUserAddress(1, 10);

        Assert.That(result, Is.Not.Null);

        var dbAddress = await _context.Address.FindAsync(1);

        Assert.That(dbAddress, Is.Not.Null);
        Assert.That(dbAddress!.IsActive, Is.False);
    }

    [Test]
    public async Task DeleteUserAddress_ShouldThrow_WhenAddressIsDefault()
    {
        var address = MakeAddress(addressId: 1, userId: 10, isDefault: true);

        _context.Address.Add(address);
        await _context.SaveChangesAsync();

        _userValidation
            .Setup(v => v.ValidateAddress(1, 10))
            .ReturnsAsync(address);

        var ex = Assert.ThrowsAsync<DataApprovalStatusException>(async () =>
            await _sut.DeleteUserAddress(1, 10));

        Assert.That(ex!.Message, Is.EqualTo("Default address cannot be deleted"));
    }

    [Test]
    public async Task DeleteInventoryAddress_ShouldDeactivateAddress_InDatabase()
    {
        var address = MakeAddress(addressId: 1, userId: 10, isDefault: false, isActive: true);

        _context.Address.Add(address);
        await _context.SaveChangesAsync();

        _userValidation
            .Setup(v => v.ValidateAddress(1, 10))
            .ReturnsAsync(address);

        var result = await _sut.DeleteInventoryAddress(1, 10);

        Assert.That(result, Is.Not.Null);

        var dbAddress = await _context.Address.FindAsync(1);

        Assert.That(dbAddress, Is.Not.Null);
        Assert.That(dbAddress!.IsActive, Is.False);
    }
}