
using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace EcommerceTest;

public class AddressServiceTest
{
    private Mock<IVendorUserValidation> _vendorUserValidation = null!;
    private Mock<INotificationService> _notificationService = null!;
    private Mock<IAddressRepsository> _addressRepo = null!;
    private Mock<IOrderRepsository> _orderRepo = null!;
    private Mock<IOrderItemRepsository> _orderItemRepo = null!;
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
            City = "Chennai",
            PinCode = "600001"
        };


    [SetUp]
    public void Setup()
    {
        _vendorUserValidation = new Mock<IVendorUserValidation>();
        _addressRepo = new Mock<IAddressRepsository>();
        _orderRepo = new Mock<IOrderRepsository>();
        _orderItemRepo = new Mock<IOrderItemRepsository>();
        _userValidation = new Mock<IUserValidation>();
        _logger = new Mock<ILogger<AddressService>>();
        _notificationService = new Mock<INotificationService>();

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
            _orderItemRepo.Object,
            _orderRepo.Object,
            _addressRepo.Object,
            _userValidation.Object,
            _mapper,
            _logger.Object
        );
    }
    [Test]
    public async Task AddAddress_ShouldCreateAddress_WhenNotDefault()
    {
        var request = new RequestAddAddressDTO { City = "Chennai", PinCode = "600001" };
        var address = MakeAddress(isDefault: false);
        var user = new User
        {
            UserId = 10,
            FirstName = "Siva"
        };

        _userValidation.Setup(v => v.ValidateUser(10)).ReturnsAsync(user);
        _userValidation.Setup(v => v.ValidateUser(10)).ReturnsAsync(user);
        _addressRepo.Setup(r => r.Create(It.IsAny<Address>())).ReturnsAsync(address);

        var result = await _sut.AddAddress(request, 10);

        Assert.That(result, Is.Not.Null);
        _addressRepo.Verify(r => r.Create(It.Is<Address>(a => a.UserId == 10)), Times.Once);
    }

    [Test]
    public async Task AddAddress_ShouldSetUserId_OnAddressBeforeCreating()
    {
        var request = new RequestAddAddressDTO { City = "Chennai", PinCode = "600001" };
        var address = MakeAddress(userId: 10, isDefault: false);

        var user = new User
        {
            UserId = 10,
            FirstName = "Siva"
        };

        _userValidation.Setup(v => v.ValidateUser(10)).ReturnsAsync(user);
        _userValidation.Setup(v => v.ValidateUser(10)).ReturnsAsync(user);
        _addressRepo.Setup(r => r.Create(It.IsAny<Address>())).ReturnsAsync(address);

        await _sut.AddAddress(request, 10);

        _addressRepo.Verify(r => r.Create(It.Is<Address>(a => a.UserId == 10)), Times.Once);
    }

    [Test]
    public async Task AddAddress_ShouldCallMakeAddressDefault_WhenIsDefaultTrue()
    {
        var request = new RequestAddAddressDTO { City = "Chennai", PinCode = "600001", IsDefault = true };
        var address = MakeAddress(addressId: 1, userId: 10, isDefault: true);
        var allAddresses = new List<Address> { address };
        var user = new User
        {
            UserId = 10,
            FirstName = "Siva"
        };

        _userValidation.Setup(v => v.ValidateUser(10)).ReturnsAsync(user);
        _userValidation.Setup(v => v.ValidateUser(10)).ReturnsAsync(user);
        _addressRepo.Setup(r => r.Create(It.IsAny<Address>())).ReturnsAsync(address);

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(address);
        _addressRepo.Setup(r => r.GetAllAddressByUserId(10)).ReturnsAsync(allAddresses);
        _addressRepo.Setup(r => r.Update(It.IsAny<int>(), It.IsAny<Address>())).ReturnsAsync(address);

        var result = await _sut.AddAddress(request, 10);

        Assert.That(result, Is.Not.Null);
        _addressRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Address>()), Times.AtLeastOnce);
    }

    [Test]
    public async Task AddAddress_ShouldNotCallMakeAddressDefault_WhenIsDefaultFalse()
    {
        var request = new RequestAddAddressDTO { City = "Chennai", PinCode = "600001", IsDefault = false };
        var address = MakeAddress(isDefault: false);
        var user = new User
        {
            UserId = 10,
            FirstName = "Siva"
        };

        _userValidation.Setup(v => v.ValidateUser(10)).ReturnsAsync(user);
        _userValidation.Setup(v => v.ValidateUser(10)).ReturnsAsync(user);
        _addressRepo.Setup(r => r.Create(It.IsAny<Address>())).ReturnsAsync(address);

        await _sut.AddAddress(request, 10);

        _addressRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Address>()), Times.Never);
    }

    [Test]
    public async Task AddAddress_ShouldThrow_WhenUserValidationFails()
    {
        _userValidation.Setup(v => v.ValidateUser(99))
                       .ThrowsAsync(new DataNotFoundException("User not found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.AddAddress(new RequestAddAddressDTO(), 99));

        Assert.That(ex!.Message, Is.EqualTo("User not found"));
        _addressRepo.Verify(r => r.Create(It.IsAny<Address>()), Times.Never);
    }

    [Test]
    public async Task AddAddress_ShouldThrow_WhenCreationReturnsNull()
    {
        var request = new RequestAddAddressDTO { City = "Chennai", PinCode = "600001" };

        var user = new User
        {
            UserId = 10,
            FirstName = "Siva"
        };

        _userValidation.Setup(v => v.ValidateUser(10)).ReturnsAsync(user);
        _addressRepo.Setup(r => r.Create(It.IsAny<Address>())).ReturnsAsync((Address?)null);

        var ex = Assert.ThrowsAsync<DataRegistrationException>(async () => await _sut.AddAddress(request, 10));

        Assert.That(ex!.Message, Is.EqualTo("Failed to create address"));
    }
    [Test]
    public async Task MakeAddressDefault_ShouldSetSelectedAddressAsDefault()
    {
        var selected = MakeAddress(addressId: 2, userId: 10);
        var others = new List<Address>
        {
            MakeAddress(addressId: 1, userId: 10, isDefault: true),
            MakeAddress(addressId: 2, userId: 10, isDefault: false)
        };

        _userValidation.Setup(v => v.ValidateAddress(2, 10)).ReturnsAsync(selected);
        _addressRepo.Setup(r => r.GetAllAddressByUserId(10)).ReturnsAsync(others);
        _addressRepo.Setup(r => r.Update(It.IsAny<int>(), It.IsAny<Address>())).ReturnsAsync(selected);

        var result = await _sut.MakeAddressDefault(2, 10);

        Assert.That(result, Is.Not.Null);
        Assert.That(selected.IsDefault, Is.True);
    }

    [Test]
    public async Task MakeAddressDefault_ShouldSetAllOtherAddresses_IsDefaultFalse()
    {
        var addr1 = MakeAddress(addressId: 1, userId: 10, isDefault: true);
        var addr2 = MakeAddress(addressId: 2, userId: 10, isDefault: true);
        var selected = MakeAddress(addressId: 3, userId: 10);
        var allAddresses = new List<Address> { addr1, addr2, selected };

        _userValidation.Setup(v => v.ValidateAddress(3, 10)).ReturnsAsync(selected);
        _addressRepo.Setup(r => r.GetAllAddressByUserId(10)).ReturnsAsync(allAddresses);
        _addressRepo.Setup(r => r.Update(It.IsAny<int>(), It.IsAny<Address>())).ReturnsAsync(selected);

        await _sut.MakeAddressDefault(3, 10);

        Assert.That(addr1.IsDefault, Is.False);
        Assert.That(addr2.IsDefault, Is.False);
    }

    [Test]
    public async Task MakeAddressDefault_ShouldThrow_WhenAddressValidationFails()
    {
        _userValidation.Setup(v => v.ValidateAddress(99, 10)).ThrowsAsync(new DataNotFoundException("Address not found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () => await _sut.MakeAddressDefault(99, 10));

        Assert.That(ex!.Message, Is.EqualTo("Address not found"));
        _addressRepo.Verify(r => r.GetAllAddressByUserId(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task MakeAddressDefault_ShouldThrow_WhenUpdateFails()
    {
        var selected = MakeAddress(addressId: 1, userId: 10);
        var allAddresses = new List<Address> { MakeAddress(addressId: 2, userId: 10) };

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(selected);
        _addressRepo.Setup(r => r.GetAllAddressByUserId(10)).ReturnsAsync(allAddresses);
        _addressRepo.Setup(r => r.Update(It.IsAny<int>(), It.IsAny<Address>())).ReturnsAsync((Address?)null);

        var ex = Assert.ThrowsAsync<DataRegistrationException>(async () => await _sut.MakeAddressDefault(1, 10));
        Assert.That(ex!.Message, Is.EqualTo("Failed to update address"));
    }

    [Test]
    public async Task MakeAddressDefault_ShouldSetUpdatedAt_OnAllAddresses()
    {
        var addr1 = MakeAddress(addressId: 1, userId: 10);
        var selected = MakeAddress(addressId: 2, userId: 10);
        var allAddresses = new List<Address> { addr1 };

        _userValidation.Setup(v => v.ValidateAddress(2, 10)).ReturnsAsync(selected);
        _addressRepo.Setup(r => r.GetAllAddressByUserId(10)).ReturnsAsync(allAddresses);
        _addressRepo.Setup(r => r.Update(It.IsAny<int>(), It.IsAny<Address>())).ReturnsAsync(addr1);

        var before = DateTime.Now.AddSeconds(-1);
        await _sut.MakeAddressDefault(2, 10);
        var after = DateTime.Now.AddSeconds(1);

        Assert.That(addr1.UpdatedAt, Is.InRange(before, after));
        Assert.That(selected.UpdatedAt, Is.InRange(before, after));
    }
    [Test]
    public async Task DeleteUserAddress_ShouldDeactivateAddress_WhenValid()
    {
        var address = MakeAddress(addressId: 1, userId: 10, isDefault: false, isActive: true);

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(address);
        _orderRepo.Setup(r => r.GetPendingOrdersByAddress(1)).ReturnsAsync(new List<Order>());
        _addressRepo.Setup(r => r.Update(1, It.IsAny<Address>())).ReturnsAsync(address);

        var result = await _sut.DeleteUserAddress(1, 10);

        Assert.That(result, Is.Not.Null);
        Assert.That(address.IsActive, Is.False);
        _addressRepo.Verify(r => r.Update(1, It.Is<Address>(a => !a.IsActive)), Times.Once);
    }

    [Test]
    public async Task DeleteUserAddress_ShouldThrow_WhenAddressHasPendingOrders()
    {
        var address = MakeAddress(isDefault: false);

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(address);
        _orderRepo.Setup(r => r.GetPendingOrdersByAddress(1)).ReturnsAsync(new List<Order> { new() { OrderId = 1 } });

        var ex = Assert.ThrowsAsync<DataApprovalStatusException>(async () =>
            await _sut.DeleteUserAddress(1, 10));

        Assert.That(ex!.Message, Is.EqualTo("Address cannot be deleted as there is a current order right now"));
        _addressRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Address>()), Times.Never);
    }

    [Test]
    public async Task DeleteUserAddress_ShouldThrow_WhenAddressIsDefault()
    {
        var address = MakeAddress(isDefault: true);

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(address);
        _orderRepo.Setup(r => r.GetPendingOrdersByAddress(1)).ReturnsAsync(new List<Order>());

        var ex = Assert.ThrowsAsync<DataApprovalStatusException>(async () =>
            await _sut.DeleteUserAddress(1, 10));

        Assert.That(ex!.Message, Is.EqualTo("Default address cannot be deleted"));
        _addressRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Address>()), Times.Never);
    }

    [Test]
    public async Task DeleteUserAddress_ShouldThrow_WhenAddressValidationFails()
    {
        _userValidation.Setup(v => v.ValidateAddress(99, 10)).ThrowsAsync(new DataNotFoundException("Address not found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () => await _sut.DeleteUserAddress(99, 10));

        Assert.That(ex!.Message, Is.EqualTo("Address not found"));
        _orderRepo.Verify(r => r.GetPendingOrdersByAddress(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task DeleteUserAddress_ShouldSetUpdatedAt_WhenDeleting()
    {
        var address = MakeAddress(isDefault: false);

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(address);
        _orderRepo.Setup(r => r.GetPendingOrdersByAddress(1)).ReturnsAsync(new List<Order>());
        _addressRepo.Setup(r => r.Update(1, It.IsAny<Address>())).ReturnsAsync(address);

        var before = DateTime.Now.AddSeconds(-1);
        await _sut.DeleteUserAddress(1, 10);
        var after = DateTime.Now.AddSeconds(1);

        Assert.That(address.UpdatedAt, Is.InRange(before, after));
    }
    [Test]
    public async Task DeleteInventoryAddress_ShouldDeactivateAddress_WhenValid()
    {
        var address = MakeAddress(addressId: 1, userId: 10, isDefault: false, isActive: true);

        var user = new User
        {
            UserId = 10,
            FirstName = "Siva"
        };

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(address);
        _orderItemRepo.Setup(r => r.GetPendingOrderByInventoryAddress(1)).ReturnsAsync(new List<OrderItems>());
        _addressRepo.Setup(r => r.Update(1, It.IsAny<Address>())).ReturnsAsync(address);

        var result = await _sut.DeleteInventoryAddress(1, 10);

        Assert.That(result, Is.Not.Null);
        Assert.That(address.IsActive, Is.False);
        _addressRepo.Verify(r => r.Update(1, It.Is<Address>(a => !a.IsActive)), Times.Once);
    }

    [Test]
    public async Task DeleteInventoryAddress_ShouldThrow_WhenAddressHasPendingOrderItems()
    {
        var address = MakeAddress(isDefault: false);

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(address);
        _orderItemRepo.Setup(r => r.GetPendingOrderByInventoryAddress(1)).ReturnsAsync(new List<OrderItems> { new() { OrderItemsId = 1 } });

        var ex = Assert.ThrowsAsync<DataApprovalStatusException>(async () => await _sut.DeleteInventoryAddress(1, 10));

        Assert.That(ex!.Message, Is.EqualTo("Address cannot be deleted as there is a current order right now"));
        _addressRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Address>()), Times.Never);
    }

    [Test]
    public async Task DeleteInventoryAddress_ShouldThrow_WhenAddressIsDefault()
    {
        var address = MakeAddress(isDefault: true);

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(address);
        _orderItemRepo.Setup(r => r.GetPendingOrderByInventoryAddress(1)).ReturnsAsync(new List<OrderItems>());

        var ex = Assert.ThrowsAsync<DataApprovalStatusException>(async () => await _sut.DeleteInventoryAddress(1, 10));

        Assert.That(ex!.Message, Is.EqualTo("Default address cannot be deleted"));
        _addressRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Address>()), Times.Never);
    }

    [Test]
    public async Task DeleteInventoryAddress_ShouldThrow_WhenAddressValidationFails()
    {
        _userValidation.Setup(v => v.ValidateAddress(99, 10)).ThrowsAsync(new DataNotFoundException("Address not found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () => await _sut.DeleteInventoryAddress(99, 10));

        Assert.That(ex!.Message, Is.EqualTo("Address not found"));
        _orderItemRepo.Verify(r => r.GetPendingOrderByInventoryAddress(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task DeleteInventoryAddress_ShouldSetUpdatedAt_WhenDeleting()
    {
        var address = MakeAddress(isDefault: false);

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(address);
        _orderItemRepo.Setup(r => r.GetPendingOrderByInventoryAddress(1)).ReturnsAsync(new List<OrderItems>());
        _addressRepo.Setup(r => r.Update(1, It.IsAny<Address>())).ReturnsAsync(address);

        var before = DateTime.Now.AddSeconds(-1);
        await _sut.DeleteInventoryAddress(1, 10);
        var after = DateTime.Now.AddSeconds(1);

        Assert.That(address.UpdatedAt, Is.InRange(before, after));
    }

    [Test]
    public async Task DeleteInventoryAddress_UsesOrderItemRepo_NotOrderRepo()
    {
        var address = MakeAddress(isDefault: false);

        _userValidation.Setup(v => v.ValidateAddress(1, 10)).ReturnsAsync(address);
        _orderItemRepo.Setup(r => r.GetPendingOrderByInventoryAddress(1)).ReturnsAsync(new List<OrderItems>());

        _addressRepo.Setup(r => r.Update(1, It.IsAny<Address>())).ReturnsAsync(address);

        await _sut.DeleteInventoryAddress(1, 10);

        _orderItemRepo.Verify(r => r.GetPendingOrderByInventoryAddress(1), Times.Once);
        _orderRepo.Verify(r => r.GetPendingOrdersByAddress(It.IsAny<int>()), Times.Never);
    }
}
