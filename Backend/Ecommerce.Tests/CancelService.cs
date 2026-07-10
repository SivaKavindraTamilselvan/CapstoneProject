using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace EcommerceTest;

public class CancelServiceTests
{
    private EcommerceContext _context = null!;

    private IOrderRepsository _orderRepo = null!;
    private IOrderItemRepsository _orderItemRepo = null!;
    private ICancelRepsository _cancelRepo = null!;
    private IRefundRepsository _refundRepo = null!;
    private ICancelRefundRepsository _cancelRefundRepo = null!;

    private Mock<IInventoryValidation> _inventoryValidation = null!;
    private Mock<IOrderValidation> _orderValidation = null!;

    private IMapper _mapper = null!;
    private CancelService _sut = null!;

    private static Order MakeOrder(int orderId = 1) =>
        new()
        {
            OrderId = orderId,
            UserId = 10,
            OrderNumber = "ORD-TEST",
            OrderStatusId = 2
        };

    private static OrderItems MakeOrderItem(
        int orderItemId = 1,
        int orderId = 1,
        int inventoryId = 1,
        int quantity = 2,
        decimal unitPrice = 1000,
        decimal discount = 100,
        int statusId = 1) =>
        new()
        {
            OrderItemsId = orderItemId,
            OrderId = orderId,
            InventoryId = inventoryId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Discount = discount,
            OrderItemStatusId = statusId
        };

    private static Inventory MakeInventory(int inventoryId = 1) =>
        new()
        {
            InventoryId = inventoryId,
            AvailableQuantity = 10,
            ReservedQuantity = 2
        };

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new EcommerceContext(options);

        _orderRepo = new OrderRepsository(_context);
        _orderItemRepo = new OrderItemRepsository(_context);
        _cancelRepo = new CancelRepsository(_context);
        _refundRepo = new RefundRepsository(_context);
        _cancelRefundRepo = new CancelRefundRepsository(_context);

        _inventoryValidation = new Mock<IInventoryValidation>();
        _orderValidation = new Mock<IOrderValidation>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<RequestCancelDTO, Cancel>();
            cfg.CreateMap<Cancel, ResponseCancelDTO>();
            cfg.CreateMap<Cancel, CancelSummaryDto>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();

        _sut = new CancelService(
            _orderItemRepo,
            _refundRepo,
            _cancelRefundRepo,
            _orderRepo,
            _inventoryValidation.Object,
            _cancelRepo,
            _orderValidation.Object,
            _mapper
        );
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task RequestCancel_ShouldCreateCancelRefundAndCancelRefund_WhenValid()
    {
        var order = MakeOrder();
        var orderItem = MakeOrderItem();
        var inventory = MakeInventory();

        _context.Order.Add(order);
        _context.OrderItems.Add(orderItem);
        _context.Inventory.Add(inventory);
        await _context.SaveChangesAsync();

        var request = new RequestCancelDTO
        {
            OrderItemId = 1,
            CancelQuantity = 2,
            CancelReasonId = 1,
            AdditionalReason = "Changed my mind"
        };

        _orderValidation
            .Setup(v => v.ValidateOrderItem(1))
            .ReturnsAsync(orderItem);

        _orderValidation
            .Setup(v => v.ValidateOrder(1))
            .ReturnsAsync(order);

        _inventoryValidation
            .Setup(v => v.ValidateInventory(1))
            .ReturnsAsync(inventory);

        var result = await _sut.RequestCancel(request);

        Assert.That(result, Is.Not.Null);

        var cancel = await _context.Cancel.FirstOrDefaultAsync();
        Assert.That(cancel, Is.Not.Null);
        Assert.That(cancel!.CancelStatusId, Is.EqualTo(2));
        Assert.That(cancel.ConvenienceFee, Is.EqualTo(95));

        var refund = await _context.Refund.FirstOrDefaultAsync();
        Assert.That(refund, Is.Not.Null);
        Assert.That(refund!.ActualRefundAmount, Is.EqualTo(1805));
        Assert.That(refund.RefundStatusId, Is.EqualTo(4));

        var cancelRefund = await _context.CancelRefund.FirstOrDefaultAsync();
        Assert.That(cancelRefund, Is.Not.Null);
        Assert.That(cancelRefund!.CancelId, Is.EqualTo(cancel.CancelId));
        Assert.That(cancelRefund.RefundId, Is.EqualTo(refund.RefundId));

        Assert.That(orderItem.OrderItemStatusId, Is.EqualTo(7));
        Assert.That(inventory.AvailableQuantity, Is.EqualTo(12));
        Assert.That(inventory.ReservedQuantity, Is.EqualTo(0));
    }

    [Test]
    public async Task RequestCancel_ShouldThrow_WhenCancelQuantityGreaterThanOrderQuantity()
    {
        var orderItem = MakeOrderItem(quantity: 2);

        var request = new RequestCancelDTO
        {
            OrderItemId = 1,
            CancelQuantity = 5
        };

        _orderValidation
            .Setup(v => v.ValidateOrderItem(1))
            .ReturnsAsync(orderItem);

        var ex = Assert.ThrowsAsync<DataApprovalStatusException>(async () =>
            await _sut.RequestCancel(request));

        Assert.That(ex!.Message, Is.EqualTo("Quantity Higher Than the Order Quantity Is Not Possible"));

        Assert.That(await _context.Cancel.CountAsync(), Is.EqualTo(0));
        Assert.That(await _context.Refund.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task RequestCancel_ShouldThrow_WhenOrderAlreadyShipped()
    {
        var orderItem = MakeOrderItem(statusId: 3);

        var request = new RequestCancelDTO
        {
            OrderItemId = 1,
            CancelQuantity = 1
        };

        _orderValidation
            .Setup(v => v.ValidateOrderItem(1))
            .ReturnsAsync(orderItem);

        var ex = Assert.ThrowsAsync<DataApprovalStatusException>(async () =>
            await _sut.RequestCancel(request));

        Assert.That(ex!.Message, Is.EqualTo("Order Already Shipped. Cannot be cancelled"));

        Assert.That(await _context.Cancel.CountAsync(), Is.EqualTo(0));
        Assert.That(await _context.Refund.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task RequestCancel_ShouldReduceQuantity_WhenPartialCancel()
    {
        var order = MakeOrder();
        var orderItem = MakeOrderItem(quantity: 5, statusId: 1);
        var inventory = MakeInventory();

        _context.Order.Add(order);
        _context.OrderItems.Add(orderItem);
        _context.Inventory.Add(inventory);
        await _context.SaveChangesAsync();

        var request = new RequestCancelDTO
        {
            OrderItemId = 1,
            CancelQuantity = 2
        };

        _orderValidation.Setup(v => v.ValidateOrderItem(1)).ReturnsAsync(orderItem);
        _orderValidation.Setup(v => v.ValidateOrder(1)).ReturnsAsync(order);
        _inventoryValidation.Setup(v => v.ValidateInventory(1)).ReturnsAsync(inventory);

        await _sut.RequestCancel(request);

        Assert.That(orderItem.Quantity, Is.EqualTo(3));
        Assert.That(orderItem.OrderItemStatusId, Is.EqualTo(1));
    }

    [Test]
    public async Task RequestCancel_ShouldUpdateOrderStatus_WhenNoCancelledOrderItemsFound()
    {
        var order = MakeOrder();
        var orderItem = MakeOrderItem();

        _context.Order.Add(order);
        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();

        _orderValidation.Setup(v => v.ValidateOrderItem(1)).ReturnsAsync(orderItem);
        _orderValidation.Setup(v => v.ValidateOrder(1)).ReturnsAsync(order);
        _inventoryValidation.Setup(v => v.ValidateInventory(1)).ReturnsAsync(MakeInventory());

        var request = new RequestCancelDTO
        {
            OrderItemId = 1,
            CancelQuantity = 2
        };

        await _sut.RequestCancel(request);

        Assert.That(order.OrderStatusId, Is.EqualTo(4));
    }

    [Test]
    public async Task GetAllCancelsForAdmin_ShouldThrow_WhenNoCancelsFound()
    {
        var request = new RequestAdminCancelFilter
        {
            PageNumber = 1,
            PageSize = 10
        };

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.GetAllCancelsForAdmin(request));

        Assert.That(ex!.Message, Is.EqualTo("No cancel requests found"));
    }

    [Test]
    public async Task GetAllCancelsForUser_ShouldThrow_WhenNoCancelsFound()
    {
        var request = new RequestUserCancelFilter
        {
            PageNumber = 1,
            PageSize = 10
        };

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.GetAllCancelsForUser(request, 10));

        Assert.That(ex!.Message, Is.EqualTo("No cancel requests found"));
    }

    [Test]
    public async Task GetAllCancelsForVendor_ShouldThrow_WhenNoCancelsFound()
    {
        var request = new RequestVendorCancelFilter
        {
            PageNumber = 1,
            PageSize = 10
        };

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.GetAllCancelsForVendor(request, 10));

        Assert.That(ex!.Message, Is.EqualTo("No cancel requests found"));
    }
}