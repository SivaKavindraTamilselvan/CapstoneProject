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
using BankingAPI.Middlewares;


var builder = WebApplication.CreateBuilder(args);

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
        Type        = SecuritySchemeType.Http,
        Scheme      = "bearer",
        BearerFormat = "JWT",
        Name        = "Authorization",
        In          = ParameterLocation.Header,
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = false,
        ValidIssuer              = builder.Configuration["JWT:Issuer"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(
                                       Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? "")),
        ValidateLifetime         = true
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId",   "1");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorAdminOrSuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId",  "1" , "2");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductAdminOrSuperAdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "1");
        policy.RequireClaim("AdminRoleId",  "1" , "3");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOwnerOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "2");
        policy.RequireClaim("VendorRoleId",  "1");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOnwerAndProductVendorOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "2");
        policy.RequireClaim("VendorRoleId",  "1","3","2");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOnwerAndCouponVendorOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "2");
        policy.RequireClaim("VendorRoleId",  "1","8","2");
    });
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VendorOnwerAndInventoryVendorOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "2");
        policy.RequireClaim("VendorRoleId",  "1","7","2");
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
                (role == "2" && (vendorRoleId == "1" || vendorRoleId == "2" || vendorRoleId == "8"));
        });
    });
});

#region Mappers
builder.Services.AddAutoMapper(m=> m.AddProfile(new UserMappingProfile()));
builder.Services.AddAutoMapper(m=>m.AddProfile(new ProductMappingProfile()));
builder.Services.AddAutoMapper(m=>m.AddProfile(new VendorMappingProfile()));
builder.Services.AddAutoMapper(m=>m.AddProfile(new ShipmentMappingProfile()));
builder.Services.AddAutoMapper(m=>m.AddProfile(new OrderMappingProfile()));


#endregion

#region Validation
builder.Services.AddScoped<IVendorValidation,VendorValidation>();
builder.Services.AddScoped<IProductValidation,ProductValidation>();
builder.Services.AddScoped<IProductCategoryValidation,ProductCategoryValidation>();
builder.Services.AddScoped<IUserValidation,UserValidation>();
builder.Services.AddScoped<ICouponValidation,CouponValidation>();
builder.Services.AddScoped<ICartValidation,CartValidation>();
builder.Services.AddScoped<IFavoriteValidation,FavoriteValidation>();
builder.Services.AddScoped<IInventoryValidation,InventoryValidation>();
builder.Services.AddScoped<IOrderValidation,OrderValidation>();
builder.Services.AddScoped<IProductValidation,ProductValidation>();
builder.Services.AddScoped<IShipmentValidation,ShipmentValidation>();
builder.Services.AddScoped<IVendorUserValidation,VendorUserValidation>();
builder.Services.AddScoped<IProductAttributeValidation,ProductAttributeValidation>();
builder.Services.AddScoped<IAdminUserValidation,AdminUserValidation>();
#endregion

#region Repository
builder.Services.AddScoped<IUserRepsository,UserRepsository>();
builder.Services.AddScoped<IAdminUserRepsository,AdminRepsository>();
builder.Services.AddScoped<IVendorRepsository,VendorRepsository>();
builder.Services.AddScoped<IVendorUserRepsository,VendorUserRepsository>();
builder.Services.AddScoped<IProductRepsository,ProductRepsository>();
builder.Services.AddScoped<IProductImageRepsository,ProductImageRepsository>();
builder.Services.AddScoped<IProductCategoryRepsository,ProductCategoryRepsository>();
builder.Services.AddScoped<IProductSubCategoryRepsository,ProductSubCategoryRepsository>();
builder.Services.AddScoped<IProductSubCategoryAttributeRepsository,ProductSubCategoryAttributeRepsository>();
builder.Services.AddScoped<IAttributeRepsository,AttributeRepsository>();
builder.Services.AddScoped<ICartRepsository,CartRepsository>();
builder.Services.AddScoped<ICartItemsRepsository,CartItemsRepsository>();
builder.Services.AddScoped<IFavoriteRepsository,FavoriteRepsository>();
builder.Services.AddScoped<IFavoriteItemsRepsository,FavoriteItemsRepsository>();
builder.Services.AddScoped<IProductVariantRepsository,ProductVariantRepsository>();
builder.Services.AddScoped<IProductVariantAttributeRepsository,ProductVariantAttributeRepsository>();
builder.Services.AddScoped<IApprovalHistoryRepsository,ApprovalHistoryRepsository>();
builder.Services.AddScoped<ICouponProductRepsository,CouponProductRepsository>();
builder.Services.AddScoped<ICouponRepsository,CouponRepsository>();
builder.Services.AddScoped<ICouponUsageRepsository,CouponUsageRepsository>();
builder.Services.AddScoped<IInventoryRepsository,InventoryRepsository>();
builder.Services.AddScoped<IOrderRepsository,OrderRepsository>();
builder.Services.AddScoped<IOrderItemRepsository,OrderItemRepsository>();
builder.Services.AddScoped<IPaymentRepsository,PaymentRepsository>();
builder.Services.AddScoped<IRefundItemRepsository,RefundItemsRepsository>();
builder.Services.AddScoped<IRefundRepsository,RefundRepsository>();
builder.Services.AddScoped<IReturnItemRepsository,ReturnItemsRepsository>();
builder.Services.AddScoped<IReturnRepsository,ReturnRepsository>();
builder.Services.AddScoped<IReviewRepsository,ReviewRepsository>();
builder.Services.AddScoped<IShipmentItemsRepsository,ShipmentItemRepsository>();
builder.Services.AddScoped<IShipmentRepsository,ShipmentRepsository>();
builder.Services.AddScoped<IShipmentTrackingRepsository,ShipmentTrackingRepsository>();
builder.Services.AddScoped<IAddressRepsository,AddressRepsository>();
builder.Services.AddScoped<IShipperRepsository,ShipperRepsository>();
builder.Services.AddScoped<IInventoryRepsository,InventoryRepsository>();
#endregion

#region Services
builder.Services.AddScoped<IAuthentication,AuthenticationService>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IAdminService,AdminService>();
builder.Services.AddScoped<IVendorService,VendorService>();
builder.Services.AddScoped<IUserCartService,UserCartService>();
builder.Services.AddScoped<IUserFavoriteService,UserFavoritesService>();
builder.Services.AddScoped<IVendorProductService,VendorProductService>();
builder.Services.AddScoped<IAdminProductService,AdminProductService>();
builder.Services.AddScoped<IAdminVendorService,AdminVendorService>();
builder.Services.AddScoped<IAddressService,AddressService>();
builder.Services.AddScoped<IVendorProductService,VendorProductService>();
builder.Services.AddScoped<IInventoryService,InventoryService>();
builder.Services.AddScoped<IVendorCouponService,VendorCouponService>();
builder.Services.AddScoped<IUserCouponService,UserCouponService>();
builder.Services.AddScoped<IUserOrderService,UserOrderService>();
builder.Services.AddHttpClient<IShipRocketService, ShiprocketService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVendorOrderService,VendorOrderService>();
builder.Services.AddScoped<IAdminShipmentService,AdminShipmentService>();
builder.Services.AddScoped<IAdminProductAttributeService,AdminProductAttributeService>();
builder.Services.AddScoped<IAdminProductCategoryService,AdminProductCategoryService>();
builder.Services.AddScoped<IAdminProductService,AdminProductService>();
builder.Services.AddScoped<IVendorProductImageService,VendorProductImageService>();
builder.Services.AddScoped<IVendorProductVariantService,VendorProductVariantService>();
builder.Services.AddScoped<IVendorProductService,VendorProductService>();
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddScoped<IShipmentService,ShipmentService>();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
