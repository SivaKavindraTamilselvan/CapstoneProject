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

public class UserFavoritesServiceTests
{
    private EcommerceContext _context = null!;
    private IFavoriteItemsRepsository _favoriteItemsRepo = null!;

    private Mock<IFavoriteValidation> _favoriteValidation = null!;

    private IMapper _mapper = null!;
    private UserFavoritesService _sut = null!;

    private static Favorites MakeFavorite(int favoritesId = 1, int userId = 10) =>
        new()
        {
            FavoritesId = favoritesId,
            UserId = userId
        };

    private static FavoritesItems MakeFavoriteItem(
        int favoritesItemsId = 1,
        int favoritesId = 1,
        int productVariantId = 5) =>
        new()
        {
            FavoritesItemsId = favoritesItemsId,
            FavoritesId = favoritesId,
            ProductVariantId = productVariantId
        };

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new EcommerceContext(options);

        _favoriteItemsRepo = new FavoriteItemsRepsository(_context);
        _favoriteValidation = new Mock<IFavoriteValidation>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<FavoritesItems, ResponseFavoriteItemsDTO>();
            cfg.CreateMap<FavoritesItems, ResponseGetFavoriteDTO>();
            cfg.CreateMap<List<FavoritesItems>, ResponseGetFavoriteDTO>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();

        _sut = new UserFavoritesService(
            _favoriteItemsRepo,
            _mapper,
            _favoriteValidation.Object
        );
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task AddFavorite_ShouldCreateFavoriteItem_InDatabase()
    {
        var favorite = MakeFavorite(favoritesId: 1, userId: 10);

        _favoriteValidation
            .Setup(v => v.ValidateFavoriteByUserId(10))
            .ReturnsAsync(favorite);

        _favoriteValidation
            .Setup(v => v.ValidateFavoriteItemsByProductAndUser(5, 1))
            .Returns(Task.CompletedTask);

        var request = new RequestAddFavoriteItemsDTO
        {
            ProductVariantId = 5
        };

        var result = await _sut.AddFavorite(request, 10);

        Assert.That(result, Is.Not.Null);

        var dbFavoriteItem = await _context.FavoritesItems.FirstOrDefaultAsync();

        Assert.That(dbFavoriteItem, Is.Not.Null);
        Assert.That(dbFavoriteItem!.FavoritesId, Is.EqualTo(1));
        Assert.That(dbFavoriteItem.ProductVariantId, Is.EqualTo(5));
    }

    [Test]
    public async Task AddFavorite_ShouldThrow_WhenFavoriteValidationFails()
    {
        _favoriteValidation
            .Setup(v => v.ValidateFavoriteByUserId(99))
            .ThrowsAsync(new DataNotFoundException("Favorite Not Found"));

        var request = new RequestAddFavoriteItemsDTO
        {
            ProductVariantId = 5
        };

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.AddFavorite(request, 99));

        Assert.That(ex!.Message, Is.EqualTo("Favorite Not Found"));

        Assert.That(await _context.FavoritesItems.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task AddFavorite_ShouldThrow_WhenFavoriteItemAlreadyExists()
    {
        var favorite = MakeFavorite(favoritesId: 1, userId: 10);

        _favoriteValidation
            .Setup(v => v.ValidateFavoriteByUserId(10))
            .ReturnsAsync(favorite);

        _favoriteValidation
            .Setup(v => v.ValidateFavoriteItemsByProductAndUser(5, 1))
            .ThrowsAsync(new DataAlreadyRegisteredException("Product Already Added To Favorite"));

        var request = new RequestAddFavoriteItemsDTO
        {
            ProductVariantId = 5
        };

        var ex = Assert.ThrowsAsync<DataAlreadyRegisteredException>(async () =>
            await _sut.AddFavorite(request, 10));

        Assert.That(ex!.Message, Is.EqualTo("Product Already Added To Favorite"));

        Assert.That(await _context.FavoritesItems.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task DeleteFavorite_ShouldDeleteFavoriteItem_FromDatabase()
    {
        var favoriteItem = MakeFavoriteItem(favoritesItemsId: 1);

        _context.FavoritesItems.Add(favoriteItem);
        await _context.SaveChangesAsync();

        _favoriteValidation
            .Setup(v => v.ValidateFavoriteItems(1))
            .ReturnsAsync(favoriteItem);

        var result = await _sut.DeleteFavorite(new RequestDeleteFavoriteItemsDTO
        {
            FavoritesItemsId = 1
        });

        Assert.That(result, Is.Not.Null);

        var dbFavoriteItem = await _context.FavoritesItems.FindAsync(1);

        Assert.That(dbFavoriteItem, Is.Null);
    }

    [Test]
    public async Task DeleteFavorite_ShouldThrow_WhenFavoriteItemNotFound()
    {
        _favoriteValidation
            .Setup(v => v.ValidateFavoriteItems(99))
            .ThrowsAsync(new DataNotFoundException("Favorite Item Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.DeleteFavorite(new RequestDeleteFavoriteItemsDTO
            {
                FavoritesItemsId = 99
            }));

        Assert.That(ex!.Message, Is.EqualTo("Favorite Item Not Found"));
    }

    [Test]
    public async Task GetFavoriteByUserId_ShouldReturnFavorite_WhenValid()
    {
        var favoriteItem = MakeFavoriteItem(favoritesItemsId: 1);

        _favoriteValidation
    .Setup(v => v.ValidateGetFavoriteItemsByUserId(10))
    .ReturnsAsync(new List<FavoritesItems> { favoriteItem });

        var result = await _sut.GetFavoriteByUserId(10);

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetFavoriteByUserId_ShouldThrow_WhenValidationFails()
    {
        _favoriteValidation
            .Setup(v => v.ValidateGetFavoriteItemsByUserId(99))
            .ThrowsAsync(new DataNotFoundException("Favorite Items Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.GetFavoriteByUserId(99));

        Assert.That(ex!.Message, Is.EqualTo("Favorite Items Not Found"));
    }
}