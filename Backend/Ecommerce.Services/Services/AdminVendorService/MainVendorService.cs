using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using Ecommerce.API.Hubs;
using Microsoft.Extensions.Logging;

public partial class AdminVendorService : IAdminVendorService
{
    private readonly ILogger<AdminVendorService> _logger;
    private readonly IVendorRepsository _vendorRepsository;
    private readonly IAdminUserRepsository _adminUserRepsository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    public AdminVendorService(ILogger<AdminVendorService> logger,INotificationService notificationService,IMapper mapper,IVendorRepsository vendorRepsository,IAdminUserRepsository adminUserRepsository)
    {
        _notificationService = notificationService;
        _vendorRepsository = vendorRepsository;
        _adminUserRepsository = adminUserRepsository;
        _mapper = mapper;
        _logger = logger;
    }
}