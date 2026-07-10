using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace EcommerceTest;

public class OrderServiceTests
{
    private EcommerceContext _context = null!;

    private IUserRepsository _userRepo = null!;
    private IOrderRepsository _orderRepo = null!;
    private IOrderItemRepsository _orderItemRepo = null!;
    private IInventoryRepsository _inventoryRepo = null!;
    private IShipmentRepsository _shipmentRepo = null!;
    private ICouponUsageRepsository _couponUsageRepo = null!;

    private Mock<IVendorUserValidation> _vendorUserValidation = null!;
    private Mock<IInventoryValidation> _inventoryValidation = null!;
    private Mock<IShipmentService> _shipmentService = null!;

    private IMapper _mapper = null!;
    private OrderService _sut = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new EcommerceContext(options);

        _userRepo = new UserRepsository(_context);
        _orderRepo = new OrderRepsository(_context);
        _orderItemRepo = new OrderItemRepsository(_context);
        _inventoryRepo = new InventoryRepsository(_context);
        _shipmentRepo = new ShipmentRepsository(_context);
        _couponUsageRepo = new CouponUsageRepsository(_context);

        _vendorUserValidation = new Mock<IVendorUserValidation>();
        _inventoryValidation = new Mock<IInventoryValidation>();
        _shipmentService = new Mock<IShipmentService>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<RequestCreateOrderDTO, Order>();
            cfg.CreateMap<AdminOrderFilterParams, OrderFilterParams>();
            cfg.CreateMap<Order, OrderSummaryDto>();
            cfg.CreateMap<OrderItems, OrderItemSummaryDto>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();

        _sut = new OrderService(
            _couponUsageRepo,
            _vendorUserValidation.Object,
            _userRepo,
            _shipmentService.Object,
            _shipmentRepo,
            _inventoryRepo,
            _inventoryValidation.Object,
            Mock.Of<IInventoryService>(),
            _orderRepo,
            _mapper,
            _orderItemRepo
        );
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CreateOrder_ShouldCreateOrder_WithCalculatedFinalAmount()
    {
        var request = new RequestCreateOrderDTO
        {
            UserId = 1,
            AddressId = 1,
            TotalProductAmount = 1000,
            TotalCouponAmount = 100,
            TotalShippingAmount = 50
        };

        var result = await _sut.CreateOrder(request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.OrderId, Is.GreaterThan(0));
        Assert.That(result.FinalAmount, Is.EqualTo(950));
        Assert.That(result.OrderStatusId, Is.EqualTo(1));
        Assert.That(result.OrderNumber, Does.StartWith("ORD-"));

        var dbOrder = await _context.Order.FindAsync(result.OrderId);
        Assert.That(dbOrder, Is.Not.Null);
    }

    [Test]
    public async Task CreateOrderItems_ShouldCreateOrderItem_AndUpdateInventory()
    {
        var product = new Product { ProductId = 1, VendorId = 10, ProductName = "Phone" };

        var variant = new ProductVariant
        {
            ProductVariantId = 1,
            ProductId = 1,
            Product = product,
            Price = 500
        };

        var inventory = new Inventory
        {
            InventoryId = 1,
            ProductVariantId = 1,
            AvailableQuantity = 10,
            ReservedQuantity = 0
        };

        var cartItem = new CartItems
        {
            CartItemsId = 1,
            ProductVariantId = 1,
            ProductVariant = variant,
            Qunatity = 2
        };

        var order = new Order
        {
            OrderId = 1,
            UserId = 1,
            OrderNumber = "ORD-TEST"
        };

        _context.Product.Add(product);
        _context.ProductVariant.Add(variant);
        _context.Inventory.Add(inventory);
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var selectedItems = new List<SelectedCartInventory>
        {
            new()
            {
                CartItem = cartItem,
                Inventory = inventory
            }
        };

        _inventoryValidation
            .Setup(v => v.ValidateInventory(1))
            .ReturnsAsync(inventory);

        var result = await _sut.CreateOrderItems(selectedItems, order, null);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Quantity, Is.EqualTo(2));
        Assert.That(result[0].UnitPrice, Is.EqualTo(500));

        Assert.That(inventory.AvailableQuantity, Is.EqualTo(8));
        Assert.That(inventory.ReservedQuantity, Is.EqualTo(2));
    }

    [Test]
    public async Task CreateOrderItems_ShouldApplyCoupon_AndCreateCouponUsage_WhenCouponApplicable()
    {
        var product = new Product { ProductId = 1, VendorId = 10, ProductName = "Phone" };

        var variant = new ProductVariant
        {
            ProductVariantId = 1,
            ProductId = 1,
            Product = product,
            Price = 500
        };

        var inventory = new Inventory
        {
            InventoryId = 1,
            ProductVariantId = 1,
            AvailableQuantity = 10,
            ReservedQuantity = 0
        };

        var cartItem = new CartItems
        {
            ProductVariantId = 1,
            ProductVariant = variant,
            Qunatity = 1
        };

        var order = new Order
        {
            OrderId = 1,
            UserId = 1,
            OrderNumber = "ORD-TEST"
        };

        var coupon = new Coupons
        {
            CouponId = 1,
            DiscountValue = 50,
            CouponsProducts = new List<CouponsProduct>
            {
                new() { ProductId = 1 }
            }
        };

        _context.Product.Add(product);
        _context.ProductVariant.Add(variant);
        _context.Inventory.Add(inventory);
        _context.Order.Add(order);
        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();

        _inventoryValidation
            .Setup(v => v.ValidateInventory(1))
            .ReturnsAsync(inventory);

        var selectedItems = new List<SelectedCartInventory>
        {
            new()
            {
                CartItem = cartItem,
                Inventory = inventory
            }
        };

        var result = await _sut.CreateOrderItems(selectedItems, order, coupon);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Discount, Is.EqualTo(50));

        var usage = await _context.CouponUsage.FirstOrDefaultAsync();
        Assert.That(usage, Is.Not.Null);
        Assert.That(usage!.CouponId, Is.EqualTo(1));
        Assert.That(usage.OrderId, Is.EqualTo(1));
    }

    [Test]
    public async Task ConfirmOrderStatus_ShouldSetOrderStatusToConfirmed_WhenStatusTrue()
    {
        var order = new Order
        {
            OrderId = 1,
            UserId = 1,
            OrderNumber = "ORD-TEST",
            OrderStatusId = 1
        };

        var shipment = new Shipment
        {
            ShipmentId = 1,
            OrderId = 1,
            ShipmentStatusId = 1
        };

        _context.Order.Add(order);
        _context.Shipment.Add(shipment);
        await _context.SaveChangesAsync();

        _shipmentService
    .Setup(s => s.CreateShipmentTracking(It.IsAny<RequestAddShipmentTrackingDTO>()))
    .ReturnsAsync(new ResponseAddShipmentTrackingDTO());

        await _sut.ConfirmOrderStatus(1, true);

        var dbOrder = await _context.Order.FindAsync(1);
        var dbShipment = await _context.Shipment.FindAsync(1);

        Assert.That(dbOrder!.OrderStatusId, Is.EqualTo(2));
        Assert.That(dbShipment!.ShipmentStatusId, Is.EqualTo(2));

        _shipmentService.Verify(s => s.CreateShipmentTracking(
            It.Is<RequestAddShipmentTrackingDTO>(x =>
                x.ShipmentId == 1 &&
                x.ShipmentStatusId == 2 &&
                x.Location == "Warehouse"
            )), Times.Once);
    }

    [Test]
    public async Task ConfirmOrderStatus_ShouldSetOrderStatusToFailed_WhenStatusFalse()
    {
        var order = new Order
        {
            OrderId = 1,
            UserId = 1,
            OrderNumber = "ORD-TEST",
            OrderStatusId = 1
        };

        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        await _sut.ConfirmOrderStatus(1, false);

        var dbOrder = await _context.Order.FindAsync(1);

        Assert.That(dbOrder!.OrderStatusId, Is.EqualTo(5));
    }

    [Test]
    public async Task ConfirmOrderStatus_ShouldThrow_WhenOrderNotFound()
    {
        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.ConfirmOrderStatus(999, true));

        Assert.That(ex!.Message, Is.EqualTo("Order Not Found"));
    }

    [Test]
    public async Task GetOrderByUserId_ShouldThrow_WhenUserNotFound()
    {
        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.GetOrderByUserId(new OrderFilterParams(), 99));

        Assert.That(ex!.Message, Is.EqualTo("User Not Found"));
    }

    [Test]
    public async Task GetOrderByUserId_ShouldReturnOrders_WhenUserExists()
    {
        var user = new User
        {
            UserId = 1,
            FirstName = "Siva",
            LastName = "K",
            Email = "siva@test.com",
            PhoneNumber = "9876543210",
            Password = new byte[] { 1 },
            HashedKey = new byte[] { 1 },
            RoleId = 3,
            IsActive = true
        };

        var order = new Order
        {
            OrderId = 1,
            UserId = 1,
            OrderNumber = "ORD-TEST",
            OrderStatusId = 2
        };

        _context.User.Add(user);
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        var result = await _sut.GetOrderByUserId(new OrderFilterParams(), 1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetOrderByVendor_ShouldReturnOnlyVendorOrderItems()
    {
        var vendorUser = new VendorUser
        {
            VendorUserId = 1,
            UserId = 99,
            VendorId = 10,
            IsActive = true
        };

        var product = new Product
        {
            ProductId = 1,
            VendorId = 10,
            ProductName = "Phone"
        };

        var variant = new ProductVariant
        {
            ProductVariantId = 1,
            ProductId = 1,
            Product = product
        };

        var order = new Order
        {
            OrderId = 1,
            UserId = 1,
            OrderNumber = "ORD-TEST",
            OrderStatusId = 2,
            OrderItems = new List<OrderItems>
            {
                new()
                {
                    OrderItemsId = 1,
                    OrderId = 1,
                    ProductVariantId = 1,
                    ProductVariant = variant,
                    Quantity = 1,
                    UnitPrice = 500
                }
            }
        };

        _context.Product.Add(product);
        _context.ProductVariant.Add(variant);
        _context.Order.Add(order);
        await _context.SaveChangesAsync();

        _vendorUserValidation
            .Setup(v => v.ValidateVendorUserByUserId(99))
            .ReturnsAsync(vendorUser);

        var result = await _sut.GetOrderByVendor(new OrderFilterParams(), 99);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
    }
}