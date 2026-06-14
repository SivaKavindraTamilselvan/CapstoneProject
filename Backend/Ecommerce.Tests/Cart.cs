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

public class UserCartServiceTests
{
    private EcommerceContext _context = null!;
    private ICartItemsRepsository _cartItemsRepo = null!;

    private Mock<IProductValidation> _productValidation = null!;
    private Mock<ICartValidation> _cartValidation = null!;

    private IMapper _mapper = null!;
    private UserCartService _sut = null!;

    private static Cart MakeCart(int cartId = 1, int userId = 10) =>
        new()
        {
            CartId = cartId,
            UserId = userId
        };

    private static CartItems MakeCartItem(
        int cartItemsId = 1,
        int cartId = 1,
        int productVariantId = 5,
        int quantity = 1) =>
        new()
        {
            CartItemsId = cartItemsId,
            CartId = cartId,
            ProductVariantId = productVariantId,
            Qunatity = quantity
        };

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new EcommerceContext(options);

        _cartItemsRepo = new CartItemsRepsository(_context);

        _productValidation = new Mock<IProductValidation>();
        _cartValidation = new Mock<ICartValidation>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CartItems, ResponseCartItemsDTO>();
            cfg.CreateMap<CartItems, ResponseGetCartDTO>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();

        _sut = new UserCartService(
            _productValidation.Object,
            _cartItemsRepo,
            _mapper,
            _cartValidation.Object
        );
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task AddCart_ShouldCreateCartItem_WhenValid()
    {
        var request = new RequestAddCartItemsDTO
        {
            ProductVariantId = 5
        };

        var cart = MakeCart(cartId: 1, userId: 10);

        _cartValidation
            .Setup(v => v.ValidateCartByUserId(10))
            .ReturnsAsync(cart);

        _productValidation
            .Setup(v => v.ValidateProductVariantIfApproved(5))
            .ReturnsAsync(new ProductVariant { ProductVariantId = 5 });

        _cartValidation
            .Setup(v => v.ValidateCartItemsByProductAndUser(5, 1))
            .Returns(Task.CompletedTask);

        var result = await _sut.AddCart(request, 10);

        Assert.That(result, Is.Not.Null);

        var dbCartItem = await _context.CartItems.FirstOrDefaultAsync();
        Assert.That(dbCartItem, Is.Not.Null);
        Assert.That(dbCartItem!.CartId, Is.EqualTo(1));
        Assert.That(dbCartItem.ProductVariantId, Is.EqualTo(5));
    }

    [Test]
    public async Task AddCart_ShouldThrow_WhenCartValidationFails()
    {
        var request = new RequestAddCartItemsDTO
        {
            ProductVariantId = 5
        };

        _cartValidation
            .Setup(v => v.ValidateCartByUserId(10))
            .ThrowsAsync(new DataNotFoundException("Cart Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.AddCart(request, 10));

        Assert.That(ex!.Message, Is.EqualTo("Cart Not Found"));

        Assert.That(await _context.CartItems.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task AddCart_ShouldThrow_WhenProductVariantValidationFails()
    {
        var request = new RequestAddCartItemsDTO
        {
            ProductVariantId = 5
        };

        _cartValidation
            .Setup(v => v.ValidateCartByUserId(10))
            .ReturnsAsync(MakeCart());

        _productValidation
            .Setup(v => v.ValidateProductVariantIfApproved(5))
            .ThrowsAsync(new DataNotFoundException("Product Variant Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.AddCart(request, 10));

        Assert.That(ex!.Message, Is.EqualTo("Product Variant Not Found"));

        Assert.That(await _context.CartItems.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task AddCart_ShouldThrow_WhenProductAlreadyInCart()
    {
        var request = new RequestAddCartItemsDTO
        {
            ProductVariantId = 5
        };

        _cartValidation
            .Setup(v => v.ValidateCartByUserId(10))
            .ReturnsAsync(MakeCart());

        _productValidation
            .Setup(v => v.ValidateProductVariantIfApproved(5))
            .ReturnsAsync(new ProductVariant { ProductVariantId = 5 });

        _cartValidation
            .Setup(v => v.ValidateCartItemsByProductAndUser(5, 1))
            .ThrowsAsync(new DataAlreadyRegisteredException("Product Already Added To Cart"));

        var ex = Assert.ThrowsAsync<DataAlreadyRegisteredException>(async () =>
            await _sut.AddCart(request, 10));

        Assert.That(ex!.Message, Is.EqualTo("Product Already Added To Cart"));

        Assert.That(await _context.CartItems.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task DeleteCart_ShouldDeleteCartItem_WhenValid()
    {
        var cartItem = MakeCartItem(cartItemsId: 1);

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();

        _cartValidation
            .Setup(v => v.ValidateCartItems(1))
            .ReturnsAsync(cartItem);

        var result = await _sut.DeleteCart(new RequestDeleteCartItemsDTO
        {
            CartItemsId = 1
        });

        Assert.That(result, Is.Not.Null);

        var dbCartItem = await _context.CartItems.FindAsync(1);
        Assert.That(dbCartItem, Is.Null);
    }

    [Test]
    public async Task DeleteCart_ShouldThrow_WhenCartItemNotFound()
    {
        _cartValidation
            .Setup(v => v.ValidateCartItems(99))
            .ThrowsAsync(new DataNotFoundException("Cart Items Is Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.DeleteCart(new RequestDeleteCartItemsDTO
            {
                CartItemsId = 99
            }));

        Assert.That(ex!.Message, Is.EqualTo("Cart Items Is Not Found"));
    }

    [Test]
    public async Task DeleteAllCart_ShouldReturnMappedCartItems_WhenValid()
    {
        var cartItems = new List<CartItems>
        {
            MakeCartItem(cartItemsId: 1),
            MakeCartItem(cartItemsId: 2)
        };

        _cartValidation
            .Setup(v => v.ValidateDeleteCartItemsByUserId(10))
            .ReturnsAsync(cartItems);

        var result = await _sut.DeleteAllCart(10);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task DeleteAllCart_ShouldThrow_WhenValidationFails()
    {
        _cartValidation
            .Setup(v => v.ValidateDeleteCartItemsByUserId(99))
            .ThrowsAsync(new DataNotFoundException("Cart Items Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.DeleteAllCart(99));

        Assert.That(ex!.Message, Is.EqualTo("Cart Items Not Found"));
    }

    [Test]
    public async Task GetCartByUserId_ShouldReturnCartItems_WhenValid()
    {
        var cartItems = new List<CartItems>
        {
            MakeCartItem(cartItemsId: 1),
            MakeCartItem(cartItemsId: 2)
        };

        _cartValidation
            .Setup(v => v.ValidateGetCartItemsByUserId(10))
            .ReturnsAsync(cartItems);

        var result = await _sut.GetCartByUserId(10);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetCartByUserId_ShouldThrow_WhenValidationFails()
    {
        _cartValidation
            .Setup(v => v.ValidateGetCartItemsByUserId(99))
            .ThrowsAsync(new DataNotFoundException("Cart Items Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.GetCartByUserId(99));

        Assert.That(ex!.Message, Is.EqualTo("Cart Items Not Found"));
    }

    [Test]
    public async Task UpdateCart_ShouldUpdateQuantity_WhenCartItemExists()
    {
        var cartItem = MakeCartItem(cartItemsId: 1, quantity: 1);

        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();

        var request = new RequestUpdateCartItemsDTO
        {
            CartItemsId = 1,
            Qunatity = 5
        };

        var result = await _sut.UpdateCart(request);

        Assert.That(result, Is.Not.Null);

        var dbCartItem = await _context.CartItems.FindAsync(1);
        Assert.That(dbCartItem, Is.Not.Null);
        Assert.That(dbCartItem!.Qunatity, Is.EqualTo(5));
    }

    [Test]
    public async Task UpdateCart_ShouldThrow_WhenCartItemNotFound()
    {
        var request = new RequestUpdateCartItemsDTO
        {
            CartItemsId = 99,
            Qunatity = 5
        };

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.UpdateCart(request));

        Assert.That(ex!.Message, Is.EqualTo("Cart Items Is Not Found"));
    }
}