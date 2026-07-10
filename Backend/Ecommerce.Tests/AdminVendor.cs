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

namespace EcommerceTest;

public class AdminVendorServiceTest
{
    private Mock<IVendorUserRepsository> _vendorUserRepo = null!;
    private Mock<IVendorValidation> _vendorValidation = null!;
    private Mock<IApprovalHistoryRepsository> _approvalRepo = null!;
    private Mock<IVendorRepsository> _vendorRepo = null!;
    private Mock<IAdminUserRepsository> _adminUserRepo = null!;
    private Mock<IAdminUserValidation> _adminUserValidation = null!;
    private Mock<INotificationService> _notificationSvc = null!;
    private Mock<ILogger<AdminVendorService>> _logger = null!;
    private IMapper _mapper = null!;
    private AdminVendorService _sut = null!;
    private EcommerceContext _context = null!;

    private static Vendor MakeVendor(int vendorId = 1, int approvalStatus = 1) =>
        new()
        {
            VendorId = vendorId,
            ApprovalStatusId = approvalStatus,
            VendorCompanyName = "Test Stores"
        };

    private static AdminUser MakeAdminUser(int adminUserId = 10, int userId = 99) =>
        new()
        {
            AdminUserId = adminUserId,
            UserId = userId
        };

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<EcommerceContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new EcommerceContext(options);

        _vendorUserRepo = new Mock<IVendorUserRepsository>();
        _vendorValidation = new Mock<IVendorValidation>();
        _approvalRepo = new Mock<IApprovalHistoryRepsository>();
        _vendorRepo = new Mock<IVendorRepsository>();
        _adminUserRepo = new Mock<IAdminUserRepsository>();
        _adminUserValidation = new Mock<IAdminUserValidation>();
        _notificationSvc = new Mock<INotificationService>();
        _logger = new Mock<ILogger<AdminVendorService>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Vendor, ResponseGetVendor>();
            cfg.CreateMap<Vendor, ResponseReviewOfVendorDTO>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();

        _sut = new AdminVendorService(
            _context,
            _approvalRepo.Object,
            _vendorRepo.Object,
            _adminUserValidation.Object,
            _vendorValidation.Object,
            _logger.Object,
            _notificationSvc.Object,
            _mapper,
            _vendorUserRepo.Object,
            _adminUserRepo.Object
        );
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task ReviewVendor_ShouldApproveVendor_AndSendNotification()
    {
        var vendor = MakeVendor(approvalStatus: 1);
        var adminUser = MakeAdminUser();

        var request = new RequestReviewOfVendorDTO
        {
            VendorId = 1,
            ApprovalStatusId = 2,
            Remark = ""
        };

        _vendorValidation.Setup(v => v.ValidateVendor(1)).ReturnsAsync(vendor);
        _adminUserValidation.Setup(v => v.ValidateAdminUserByUserId(99)).ReturnsAsync(adminUser);
        _vendorRepo.Setup(r => r.Update(1, It.IsAny<Vendor>())).ReturnsAsync(vendor);
        _approvalRepo.Setup(r => r.Create(It.IsAny<ApprovalHistory>())).ReturnsAsync(new ApprovalHistory());
        _vendorUserRepo.Setup(r => r.GetOwnerVendorUserByVendorId(1))
            .ReturnsAsync(new VendorUser { UserId = 50, VendorId = 1 });

        var result = await _sut.ReviewVendor(request, 99);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ApprovalStatusId, Is.EqualTo(2));

        _vendorRepo.Verify(r => r.Update(1, It.Is<Vendor>(v =>
            v.ApprovalStatusId == 2 &&
            v.ReviewedByAdminId == 10)), Times.Once);

        _approvalRepo.Verify(r => r.Create(It.Is<ApprovalHistory>(a =>
            a.EntityId == 1 &&
            a.PreviousStatusId == 1 &&
            a.NewStatusId == 2 &&
            a.ReviewedByAdminId == 10)), Times.Once);

        _notificationSvc.Verify(n => n.SendToUser(
            50,
            "Vendor Approved",
            "",
            15,
            "Vendor",
            1), Times.Once);
    }

    [Test]
    public async Task ReviewVendor_ShouldRejectVendor_AndSendRejectionNotification()
    {
        var vendor = MakeVendor(approvalStatus: 1);
        var adminUser = MakeAdminUser();

        var request = new RequestReviewOfVendorDTO
        {
            VendorId = 1,
            ApprovalStatusId = 3,
            Remark = ""
        };

        _vendorValidation.Setup(v => v.ValidateVendor(1)).ReturnsAsync(vendor);
        _adminUserValidation.Setup(v => v.ValidateAdminUserByUserId(99)).ReturnsAsync(adminUser);
        _vendorRepo.Setup(r => r.Update(1, It.IsAny<Vendor>())).ReturnsAsync(vendor);
        _approvalRepo.Setup(r => r.Create(It.IsAny<ApprovalHistory>())).ReturnsAsync(new ApprovalHistory());
        _vendorUserRepo.Setup(r => r.GetOwnerVendorUserByVendorId(1))
            .ReturnsAsync(new VendorUser { UserId = 50, VendorId = 1 });

        var result = await _sut.ReviewVendor(request, 99);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ApprovalStatusId, Is.EqualTo(3));

        _notificationSvc.Verify(n => n.SendToUser(
            50,
            "Vendor Rejected",
            "",
            16,
            "Vendor",
            1), Times.Once);
    }

    [Test]
    public async Task ReviewVendor_ShouldThrow_WhenVendorNotFound()
    {
        var request = new RequestReviewOfVendorDTO
        {
            VendorId = 999,
            ApprovalStatusId = 2
        };

        _vendorValidation
            .Setup(v => v.ValidateVendor(999))
            .ThrowsAsync(new DataNotFoundException("Vendor ID Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.ReviewVendor(request, 99));

        Assert.That(ex!.Message, Is.EqualTo("Vendor ID Not Found"));
    }

    [Test]
    public async Task ReviewVendor_ShouldThrow_WhenVendorAlreadyApproved()
    {
        var vendor = MakeVendor(approvalStatus: 2);

        var request = new RequestReviewOfVendorDTO
        {
            VendorId = 1,
            ApprovalStatusId = 2
        };

        _vendorValidation.Setup(v => v.ValidateVendor(1)).ReturnsAsync(vendor);

        var ex = Assert.ThrowsAsync<DataApprovalStatusException>(async () =>
            await _sut.ReviewVendor(request, 99));

        Assert.That(ex!.Message, Is.EqualTo("Vendor Already Reviewed"));
        _vendorRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Vendor>()), Times.Never);
    }

    [Test]
    public async Task ReviewVendor_ShouldThrow_WhenVendorAlreadyRejected()
    {
        var vendor = MakeVendor(approvalStatus: 3);

        var request = new RequestReviewOfVendorDTO
        {
            VendorId = 1,
            ApprovalStatusId = 2
        };

        _vendorValidation.Setup(v => v.ValidateVendor(1)).ReturnsAsync(vendor);

        var ex = Assert.ThrowsAsync<DataApprovalStatusException>(async () =>
            await _sut.ReviewVendor(request, 99));

        Assert.That(ex!.Message, Is.EqualTo("Vendor Already Reviewed"));
        _vendorRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Vendor>()), Times.Never);
    }

    [Test]
    public async Task ReviewVendor_ShouldThrow_WhenAdminUserNotFound()
    {
        var vendor = MakeVendor(approvalStatus: 1);

        var request = new RequestReviewOfVendorDTO
        {
            VendorId = 1,
            ApprovalStatusId = 2
        };

        _vendorValidation.Setup(v => v.ValidateVendor(1)).ReturnsAsync(vendor);
        _adminUserValidation
            .Setup(v => v.ValidateAdminUserByUserId(999))
            .ThrowsAsync(new DataNotFoundException("Admin Not Found"));

        var ex = Assert.ThrowsAsync<DataNotFoundException>(async () =>
            await _sut.ReviewVendor(request, 999));

        Assert.That(ex!.Message, Is.EqualTo("Admin Not Found"));
        _vendorRepo.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Vendor>()), Times.Never);
    }

    [Test]
    public async Task ReviewVendor_ShouldNotSendNotification_WhenOwnerUserNotFound()
    {
        var vendor = MakeVendor(approvalStatus: 1);
        var adminUser = MakeAdminUser();

        var request = new RequestReviewOfVendorDTO
        {
            VendorId = 1,
            ApprovalStatusId = 2,
            Remark = ""
        };

        _vendorValidation.Setup(v => v.ValidateVendor(1)).ReturnsAsync(vendor);
        _adminUserValidation.Setup(v => v.ValidateAdminUserByUserId(99)).ReturnsAsync(adminUser);
        _vendorRepo.Setup(r => r.Update(1, It.IsAny<Vendor>())).ReturnsAsync(vendor);
        _approvalRepo.Setup(r => r.Create(It.IsAny<ApprovalHistory>())).ReturnsAsync(new ApprovalHistory());

        _vendorUserRepo.Setup(r => r.GetOwnerVendorUserByVendorId(1))
            .ReturnsAsync((VendorUser?)null);

        var result = await _sut.ReviewVendor(request, 99);

        Assert.That(result, Is.Not.Null);

        _notificationSvc.Verify(n => n.SendToUser(
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task ReviewVendor_ShouldSetReviewedAt_WhenReviewing()
    {
        var vendor = MakeVendor(approvalStatus: 1);
        var adminUser = MakeAdminUser();

        var request = new RequestReviewOfVendorDTO
        {
            VendorId = 1,
            ApprovalStatusId = 2,
            Remark = ""
        };

        _vendorValidation.Setup(v => v.ValidateVendor(1)).ReturnsAsync(vendor);
        _adminUserValidation.Setup(v => v.ValidateAdminUserByUserId(99)).ReturnsAsync(adminUser);
        _vendorRepo.Setup(r => r.Update(1, It.IsAny<Vendor>())).ReturnsAsync(vendor);
        _approvalRepo.Setup(r => r.Create(It.IsAny<ApprovalHistory>())).ReturnsAsync(new ApprovalHistory());
        _vendorUserRepo.Setup(r => r.GetOwnerVendorUserByVendorId(1)).ReturnsAsync((VendorUser?)null);

        var before = DateTime.Now.AddSeconds(-1);

        await _sut.ReviewVendor(request, 99);

        var after = DateTime.Now.AddSeconds(1);

        _vendorRepo.Verify(r => r.Update(1, It.Is<Vendor>(v =>
            v.ReviewedAt >= before &&
            v.ReviewedAt <= after)), Times.Once);
    }

    [Test]
    public async Task ReviewVendor_ShouldCreateApprovalHistory()
    {
        var vendor = MakeVendor(approvalStatus: 1);
        var adminUser = MakeAdminUser();

        var request = new RequestReviewOfVendorDTO
        {
            VendorId = 1,
            ApprovalStatusId = 2,
            Remark = "Approved after checking documents"
        };

        _vendorValidation.Setup(v => v.ValidateVendor(1)).ReturnsAsync(vendor);
        _adminUserValidation.Setup(v => v.ValidateAdminUserByUserId(99)).ReturnsAsync(adminUser);
        _vendorRepo.Setup(r => r.Update(1, It.IsAny<Vendor>())).ReturnsAsync(vendor);
        _approvalRepo.Setup(r => r.Create(It.IsAny<ApprovalHistory>())).ReturnsAsync(new ApprovalHistory());
        _vendorUserRepo.Setup(r => r.GetOwnerVendorUserByVendorId(1)).ReturnsAsync((VendorUser?)null);

        await _sut.ReviewVendor(request, 99);

        _approvalRepo.Verify(r => r.Create(It.Is<ApprovalHistory>(a =>
            a.EntityId == 1 &&
            a.PreviousStatusId == 1 &&
            a.NewStatusId == 2 &&
            a.Remarks == "Approved after checking documents" &&
            a.ReviewedByAdminId == 10)), Times.Once);
    }

    [Test]
    public async Task ReviewVendor_ShouldRollback_WhenUpdateFails()
    {
        var vendor = MakeVendor(approvalStatus: 1);
        var adminUser = MakeAdminUser();

        var request = new RequestReviewOfVendorDTO
        {
            VendorId = 1,
            ApprovalStatusId = 2,
            Remark = ""
        };

        _vendorValidation.Setup(v => v.ValidateVendor(1)).ReturnsAsync(vendor);
        _adminUserValidation.Setup(v => v.ValidateAdminUserByUserId(99)).ReturnsAsync(adminUser);

        _vendorRepo.Setup(r => r.Update(1, It.IsAny<Vendor>()))
            .ThrowsAsync(new Exception("Update failed"));

        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _sut.ReviewVendor(request, 99));

        Assert.That(ex!.Message, Is.EqualTo("Update failed"));

        _approvalRepo.Verify(r => r.Create(It.IsAny<ApprovalHistory>()), Times.Never);
        _notificationSvc.Verify(n => n.SendToUser(
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>()), Times.Never);
    }
}