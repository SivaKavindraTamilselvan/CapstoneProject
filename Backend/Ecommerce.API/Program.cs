using System.Text;
using Ecommerce.Services;
using Ecommerce.Data;
using Ecommerce.Mappers;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using Ecommerce.API.Hubs;
using Ecommerce.API.Services;
using Ecommerce.Models;
using Ecommerce.DTOs;
using Ecommerce.Middlewares;
using Microsoft.Extensions.FileProviders;
using Ecommerce.Repositories;
using System.Threading.RateLimiting;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = builder.Configuration["KeyVaultName"];
if (!string.IsNullOrEmpty(keyVaultName))
{
    var vaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    var credential = new DefaultAzureCredential();
    var secretClient = new SecretClient(vaultUri, credential);

    // FIXED: was "ConnectionStrings:DefaultConnection", now matches GetConnectionString("Default")
    builder.Configuration["ConnectionStrings:Default"] =
        secretClient.GetSecret("PostgresConnectionString").Value.Value;

    builder.Configuration["BlobStorage:ConnectionString"] =
        secretClient.GetSecret("BlobStorageConnectionString").Value.Value;
    builder.Configuration["Jwt:Key"] =
        secretClient.GetSecret("JwtKey").Value.Value;
    builder.Configuration["Razorpay:KeyId"] =
        secretClient.GetSecret("RazorpayKeyId").Value.Value;
    builder.Configuration["Razorpay:KeySecret"] =
        secretClient.GetSecret("RazorpayKeySecret").Value.Value;
    builder.Configuration["Shiprocket:Email"] =
        secretClient.GetSecret("ShiprocketEmail").Value.Value;
    builder.Configuration["Shiprocket:Password"] =
        secretClient.GetSecret("ShiprocketPassword").Value.Value;
    builder.Configuration["EmailSettings:SmtpHost"] =
        secretClient.GetSecret("EmailSmtpHost").Value.Value;
    builder.Configuration["EmailSettings:SmtpPort"] =
        secretClient.GetSecret("EmailSmtpPort").Value.Value;
    builder.Configuration["EmailSettings:SenderName"] =
        secretClient.GetSecret("EmailSenderName").Value.Value;
    builder.Configuration["EmailSettings:SenderEmail"] =
        secretClient.GetSecret("EmailSenderEmail").Value.Value;
    builder.Configuration["EmailSettings:Username"] =
        secretClient.GetSecret("EmailUsername").Value.Value;
    builder.Configuration["EmailSettings:Password"] =
        secretClient.GetSecret("EmailPassword").Value.Value;
    builder.Configuration["EmailSettings:UseSsl"] =
        secretClient.GetSecret("EmailUseSsl").Value.Value;
    builder.Configuration["AiValidation:BaseUrl"] =
        secretClient.GetSecret("AiValidationServiceUrl").Value.Value;
}

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.AddPolicy("AuthPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

Log.Logger = new LoggerConfiguration()
.ReadFrom.Configuration(builder.Configuration)
.CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<EcommerceContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Description = "Enter JWT token only (without 'Bearer' prefix)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:7163",
                "http://localhost:4200",
                "http://localhost:5173",
                "null",
                "https://blue-dune-032aab810.7.azurestaticapps.net")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// FIXED: only ONE registration for AiProductValidationService, with BaseAddress set.
// The duplicate bare AddHttpClient<AiProductValidationService>() further down has been removed.
builder.Services.AddHttpClient<AiProductValidationService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AiValidation:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? "")),
        ValidateLifetime = true
    };
    opts.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/notificationhub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
// REMOVED: duplicate builder.Services.AddHttpClient<AiProductValidationService>(); (was overriding BaseAddress above)
builder.Services.AddScoped<IPasswordSetTokenRepsository, PasswordSetTokenRepsository>();
builder.Services.AddScoped<IIdempotencyKeyRepsository, IdempotencyKeyRepsository>();
builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();

#region AdminAuthorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId", "1");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductAdminOrSuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId", "1", "3");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorAdminOrSuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId", "1", "2");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OrderAdminOrSuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId", "1", "4");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CouponLogisticAdminOrSuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId", "1", "5");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ReturnAdminOrSuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId", "1", "6");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RefundAdminOrSuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId", "1", "7");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ExchangeAdminOrSuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId", "1", "8");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PaymentAdminOrSuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId", "1", "9");
    });
});

#endregion

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOwnerOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "3");
        policy.RequireClaim("VendorRoleId", "1");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOnwerAndProductVendorOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "3");
        policy.RequireClaim("VendorRoleId", "1", "3", "2");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOnwerAndCouponVendorOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "3");
        policy.RequireClaim("VendorRoleId", "1", "8", "2");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOnwerAndInventoryVendorOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "3");
        policy.RequireClaim("VendorRoleId", "1", "7", "2");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOnwerAndOrderVendorOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "3");
        policy.RequireClaim("VendorRoleId", "1", "4", "2");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOwnerAndReturnVendorOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "3");
        policy.RequireClaim("VendorRoleId", "1", "5", "2");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RefundOwnerAndReturnVendorOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "3");
        policy.RequireClaim("VendorRoleId", "1", "6", "2");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CouponPolicy", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            var adminRoleId = context.User.FindFirst("AdminRoleId")?.Value;
            var vendorRoleId = context.User.FindFirst("VendorRoleId")?.Value;

            return
                (role == "1" && (adminRoleId == "1" || adminRoleId == "5"))
                ||
                (role == "3" && (vendorRoleId == "1" || vendorRoleId == "2" || vendorRoleId == "8"));
        });
    });
});

#region Mappers
builder.Services.AddAutoMapper(m => m.AddProfile(new AuthenticationMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new UserMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new AddressMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new ProductMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new VendorMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new ShipmentMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new OrderMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new CancelMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new CouponMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new ImageMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new ProductCategoryMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new RefundMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new ReturnMappingProfile()));
builder.Services.AddAutoMapper(m => m.AddProfile(new InventoryMappingProfile()));
#endregion

#region Validation
builder.Services.AddScoped<IVendorValidation, VendorValidation>();
builder.Services.AddScoped<IProductValidation, ProductValidation>();
builder.Services.AddScoped<IProductCategoryValidation, ProductCategoryValidation>();
builder.Services.AddScoped<IUserValidation, UserValidation>();
builder.Services.AddScoped<ICouponValidation, CouponValidation>();
builder.Services.AddScoped<ICartValidation, CartValidation>();
builder.Services.AddScoped<IFavoriteValidation, FavoriteValidation>();
builder.Services.AddScoped<IInventoryValidation, InventoryValidation>();
builder.Services.AddScoped<IOrderValidation, OrderValidation>();
builder.Services.AddScoped<IShipmentValidation, ShipmentValidation>();
builder.Services.AddScoped<IVendorUserValidation, VendorUserValidation>();
builder.Services.AddScoped<IProductAttributeValidation, ProductAttributeValidation>();
builder.Services.AddScoped<IAdminUserValidation, AdminUserValidation>();
builder.Services.AddScoped<IRegistrationValidation, RegsitrationValidation>();
#endregion

#region Repository
builder.Services.AddScoped<IUserRepsository, UserRepsository>();
builder.Services.AddScoped<IAdminUserRepsository, AdminRepsository>();
builder.Services.AddScoped<IVendorRepsository, VendorRepsository>();
builder.Services.AddScoped<IVendorUserRepsository, VendorUserRepsository>();
builder.Services.AddScoped<IProductRepsository, ProductRepsository>();
builder.Services.AddScoped<IProductImageRepsository, ProductImageRepsository>();
builder.Services.AddScoped<IProductCategoryRepsository, ProductCategoryRepsository>();
builder.Services.AddScoped<IProductSubCategoryRepsository, ProductSubCategoryRepsository>();
builder.Services.AddScoped<IProductSubCategoryAttributeRepsository, ProductSubCategoryAttributeRepsository>();
builder.Services.AddScoped<IAttributeRepsository, AttributeRepsository>();
builder.Services.AddScoped<ICartRepsository, CartRepsository>();
builder.Services.AddScoped<ICartItemsRepsository, CartItemsRepsository>();
builder.Services.AddScoped<IFavoriteRepsository, FavoriteRepsository>();
builder.Services.AddScoped<IFavoriteItemsRepsository, FavoriteItemsRepsository>();
builder.Services.AddScoped<IProductVariantRepsository, ProductVariantRepsository>();
builder.Services.AddScoped<IProductVariantAttributeRepsository, ProductVariantAttributeRepsository>();
builder.Services.AddScoped<IApprovalHistoryRepsository, ApprovalHistoryRepsository>();
builder.Services.AddScoped<ICouponProductRepsository, CouponProductRepsository>();
builder.Services.AddScoped<ICouponRepsository, CouponRepsository>();
builder.Services.AddScoped<ICouponUsageRepsository, CouponUsageRepsository>();
builder.Services.AddScoped<IInventoryRepsository, InventoryRepsository>();
builder.Services.AddScoped<IOrderRepsository, OrderRepsository>();
builder.Services.AddScoped<IOrderItemRepsository, OrderItemRepsository>();
builder.Services.AddScoped<IPaymentRepsository, PaymentRepsository>();
builder.Services.AddScoped<IRefundRepsository, RefundRepsository>();
builder.Services.AddScoped<IReturnRepsository, ReturnRepsository>();
builder.Services.AddScoped<IReviewRepsository, ReviewRepsository>();
builder.Services.AddScoped<IShipmentItemsRepsository, ShipmentItemRepsository>();
builder.Services.AddScoped<IShipmentRepsository, ShipmentRepsository>();
builder.Services.AddScoped<IShipmentTrackingRepsository, ShipmentTrackingRepsository>();
builder.Services.AddScoped<IAddressRepsository, AddressRepsository>();
builder.Services.AddScoped<IAdminReturnService, AdminReturnService>();
builder.Services.AddScoped<IReturnRefundRepsository, ReturnRefundRepsository>();
builder.Services.AddScoped<INotificationRepsository, NotificationRepsository>();
builder.Services.AddScoped<IVendorApprovalRepsository, VendorApprovalHistories>();
#endregion

#region Services
builder.Services.AddScoped<IAuthentication, AuthenticationService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IUserCartService, UserCartService>();
builder.Services.AddScoped<IUserFavoriteService, UserFavoritesService>();
builder.Services.AddScoped<IVendorProductService, VendorProductService>();
builder.Services.AddScoped<IAdminProductService, AdminProductService>();
builder.Services.AddScoped<IAdminVendorService, AdminVendorService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IUserCouponService, UserCouponService>();
builder.Services.AddScoped<IUserOrderService, UserOrderService>();
builder.Services.AddHttpClient<IShipRocketService, ShiprocketService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVendorOrderService, VendorOrderService>();
builder.Services.AddScoped<IAdminShipmentService, AdminShipmentService>();
builder.Services.AddScoped<IAdminProductAttributeService, AdminProductAttributeService>();
builder.Services.AddScoped<IAdminProductCategoryService, AdminProductCategoryService>();
builder.Services.AddScoped<IVendorProductImageService, VendorProductImageService>();
builder.Services.AddScoped<IVendorProductVariantService, VendorProductVariantService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IVendorReturnService, VendorReturnService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserProductService, UserProductService>();
builder.Services.AddScoped<IUserProductCategoryService, UserProductCategoryService>();
builder.Services.AddScoped<IUserReturnService, UserReturnService>();
builder.Services.AddScoped<IAdminRefundService, AdminRefundService>();
builder.Services.AddScoped<ICancelRefundRepsository, CancelRefundRepsository>();
builder.Services.AddScoped<ICancelRepsository, CancelRepsository>();
builder.Services.AddScoped<ICancelService, CancelService>();
builder.Services.AddScoped<IAdminInventoryService, AdminInventoryService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IDashBoardService, AdminDashboardService>();
builder.Services.AddScoped<IVendorDashboardService, VendorDashboardService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();
app.UseCors();

app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// FIXED: removed duplicate/stray app.UseStaticFiles() calls and the leftover commented-out block.
// Only one static files registration now, with the missing-directory crash fixed.
var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "images");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/images"
});

app.MapControllers();

app.MapHub<NotificationHub>("/notificationhub");

app.Run();