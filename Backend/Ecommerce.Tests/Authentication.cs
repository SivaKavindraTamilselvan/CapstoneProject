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

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

}