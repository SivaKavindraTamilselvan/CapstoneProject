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
        policy.RequireClaim("VendorRoleId",  "1","3");
    });
});

#region Mappers
builder.Services.AddAutoMapper(m=> m.AddProfile(new MappingProfile()));
#endregion


builder.Services.AddScoped<IUserRepsository,UserRepsository>();
builder.Services.AddScoped<IAdminUserRepsository,AdminRepsository>();
builder.Services.AddScoped<IVendorRepsository,VendorRepsository>();
builder.Services.AddScoped<IVendorUserRepsository,VendorUserRepsository>();
builder.Services.AddScoped<IProductRepsository,ProductRepsository>();
builder.Services.AddScoped<IProductImageRepsository,ProductImageRepsository>();

builder.Services.AddScoped<IAuthentication,AuthenticationService>();
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddScoped<IAdminService,AdminService>();
builder.Services.AddScoped<IVendorService,VendorService>();
builder.Services.AddScoped<IProductService,ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
