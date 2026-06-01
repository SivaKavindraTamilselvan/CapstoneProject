using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminRole",
                columns: table => new
                {
                    AdminRoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdminRoleName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin_Role", x => x.AdminRoleId);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalStatus",
                columns: table => new
                {
                    ApprovalStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApprovalStatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approval_Status", x => x.ApprovalStatusId);
                });

            migrationBuilder.CreateTable(
                name: "AttributeMaster",
                columns: table => new
                {
                    AttributeMasterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AttributeName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Attribute_Master", x => x.AttributeMasterId);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    CouponId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CouponCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DiscountValue = table.Column<decimal>(type: "numeric", nullable: false),
                    MinimumOrderAmount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MinimumNumberOfUsage = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.CouponId);
                });

            migrationBuilder.CreateTable(
                name: "DisplayOrder",
                columns: table => new
                {
                    DisplayOrderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayOrderName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Display_Order", x => x.DisplayOrderId);
                });

            migrationBuilder.CreateTable(
                name: "ModeOfPayment",
                columns: table => new
                {
                    ModeOfPaymentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModeOfPaymentName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mode_Of_Payment", x => x.ModeOfPaymentId);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemStatus",
                columns: table => new
                {
                    OrderItemStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderItemStatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Items_Status", x => x.OrderItemStatusId);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    OrderStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderStatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Status", x => x.OrderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatus",
                columns: table => new
                {
                    PaymentStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentStatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment_Status", x => x.PaymentStatusId);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategory",
                columns: table => new
                {
                    ProductCategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductCategoryName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Category", x => x.ProductCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "ProductStatus",
                columns: table => new
                {
                    ProductStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductStatusName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Status", x => x.ProductStatusId);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantStatus",
                columns: table => new
                {
                    ProductVariantStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductVariantStatusName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Variant_Status", x => x.ProductVariantStatusId);
                });

            migrationBuilder.CreateTable(
                name: "RefundStatus",
                columns: table => new
                {
                    RefundStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RefundStatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refund_Status", x => x.RefundStatusId);
                });

            migrationBuilder.CreateTable(
                name: "ReturnReason",
                columns: table => new
                {
                    ReturnReasonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReturnReasonDescription = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Return_Reason", x => x.ReturnReasonId);
                });

            migrationBuilder.CreateTable(
                name: "ReturnStatus",
                columns: table => new
                {
                    ReturnStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReturnStatusName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Return_Status", x => x.ReturnStatusId);
                });

            migrationBuilder.CreateTable(
                name: "ReviewDescription",
                columns: table => new
                {
                    ReviewDescriptionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewDescriptionName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review_Description", x => x.ReviewDescriptionId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentStatus",
                columns: table => new
                {
                    ShipmentStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShipmentStatusName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment_Status", x => x.ShipmentStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Star",
                columns: table => new
                {
                    StarId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StarName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StarDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Star", x => x.StarId);
                });

            migrationBuilder.CreateTable(
                name: "VendorRole",
                columns: table => new
                {
                    VendorRoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendorRoleName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendor_Role", x => x.VendorRoleId);
                });

            migrationBuilder.CreateTable(
                name: "ProductSubCategory",
                columns: table => new
                {
                    ProductSubCategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductSubCategoryName = table.Column<string>(type: "text", nullable: false),
                    ProductCategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Sub_Category", x => x.ProductSubCategoryId);
                    table.ForeignKey(
                        name: "FK_Product_Category",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategory",
                        principalColumn: "ProductCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Password = table.Column<byte[]>(type: "bytea", nullable: false),
                    HashedKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_Role",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductSubCategoryAttribute",
                columns: table => new
                {
                    ProductSubCategoryAttributeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductSubCategoryId = table.Column<int>(type: "integer", nullable: false),
                    AttributeMasterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Sub_Category_Attribute", x => x.ProductSubCategoryAttributeId);
                    table.ForeignKey(
                        name: "FK_ProductSubCategoryAttribute_AttributeMaster",
                        column: x => x.AttributeMasterId,
                        principalTable: "AttributeMaster",
                        principalColumn: "AttributeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSubCategoryAttribute_ProductSubCategory",
                        column: x => x.ProductSubCategoryId,
                        principalTable: "ProductSubCategory",
                        principalColumn: "ProductSubCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ContactName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContactPhoneNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    AddressLine = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    LandMark = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false, defaultValue: "India"),
                    PinCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_User_Address",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdminUser",
                columns: table => new
                {
                    AdminUserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AdminRoleId = table.Column<int>(type: "integer", nullable: false),
                    AssignedByAdminUserId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.AdminUserId);
                    table.ForeignKey(
                        name: "FK_AdminUser_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Admin_Assigned_By_Admin",
                        column: x => x.AssignedByAdminUserId,
                        principalTable: "AdminUser",
                        principalColumn: "AdminUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Admin_Role",
                        column: x => x.AdminRoleId,
                        principalTable: "AdminRole",
                        principalColumn: "AdminRoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Cart_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    FavoritesId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.FavoritesId);
                    table.ForeignKey(
                        name: "FK_Favorites_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogChanges",
                columns: table => new
                {
                    LogChangesId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TableName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RecordId = table.Column<int>(type: "integer", nullable: false),
                    Actions = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: false),
                    NewValue = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log_Changes", x => x.LogChangesId);
                    table.ForeignKey(
                        name: "FK_Log_Changes_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TotalProductAmount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    TotalShippingAmount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    TotalCouponAmount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    FinalAmount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    AddressId = table.Column<int>(type: "integer", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OrderStatusId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Order_Address",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Status",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatus",
                        principalColumn: "OrderStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipper",
                columns: table => new
                {
                    ShipperId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyName = table.Column<string>(type: "text", nullable: false),
                    APIBaseURL = table.Column<string>(type: "text", nullable: false),
                    CreatedByAdminId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipper", x => x.ShipperId);
                    table.ForeignKey(
                        name: "FK_Shipper_Admin",
                        column: x => x.CreatedByAdminId,
                        principalTable: "AdminUser",
                        principalColumn: "AdminUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vendor",
                columns: table => new
                {
                    VendorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactPersonName = table.Column<string>(type: "text", nullable: false),
                    CompanyEmail = table.Column<string>(type: "text", nullable: false),
                    CompanyPhoneNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    VendorCompanyName = table.Column<string>(type: "text", nullable: false),
                    GSTNumber = table.Column<string>(type: "text", nullable: false),
                    ApprovalStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ReviewedByAdminId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendor", x => x.VendorId);
                    table.ForeignKey(
                        name: "FK_Admin_Review",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "AdminUser",
                        principalColumn: "AdminUserId");
                    table.ForeignKey(
                        name: "FK_Vendor_Status",
                        column: x => x.ApprovalStatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "ApprovalStatusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CouponUsage",
                columns: table => new
                {
                    CouponUsageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CouponId = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon_Usage", x => x.CouponUsageId);
                    table.ForeignKey(
                        name: "FK_Coupon_Usage_Coupon",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "CouponId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coupon_Usage_Order",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Refund",
                columns: table => new
                {
                    RefundId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    RefundStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    RequestedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ProcessedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refund", x => x.RefundId);
                    table.ForeignKey(
                        name: "FK_Refund_Order",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refund_Status",
                        column: x => x.RefundStatusId,
                        principalTable: "RefundStatus",
                        principalColumn: "RefundStatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Return",
                columns: table => new
                {
                    ReturnId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReturnReasonId = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ReturnStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    AdditionalReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RequestedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ReviewedByAdminId = table.Column<int>(type: "integer", nullable: false),
                    ReviewedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Return", x => x.ReturnId);
                    table.ForeignKey(
                        name: "FK_Return_Admin",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "AdminUser",
                        principalColumn: "AdminUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Return_Order",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Return_Reason",
                        column: x => x.ReturnReasonId,
                        principalTable: "ReturnReason",
                        principalColumn: "ReturnReasonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Return_Status",
                        column: x => x.ReturnStatusId,
                        principalTable: "ReturnStatus",
                        principalColumn: "ReturnStatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipment",
                columns: table => new
                {
                    ShipmentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShipperId = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    PickupAddressId = table.Column<int>(type: "integer", nullable: false),
                    TrackingNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ShippingCharge = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    ShipmentStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ShippedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment", x => x.ShipmentId);
                    table.ForeignKey(
                        name: "FK_Shipment_Order",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipment_Pickup_Address",
                        column: x => x.PickupAddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipment_Shipper",
                        column: x => x.ShipperId,
                        principalTable: "Shipper",
                        principalColumn: "ShipperId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipment_Status",
                        column: x => x.ShipmentStatusId,
                        principalTable: "ShipmentStatus",
                        principalColumn: "ShipmentStatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CouponsVendor",
                columns: table => new
                {
                    CouponsVendorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CouponId = table.Column<int>(type: "integer", nullable: false),
                    VendorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons_Vendor", x => x.CouponsVendorId);
                    table.ForeignKey(
                        name: "FK_Coupons_Vendor_Coupon",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "CouponId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Coupons_Vendor_Vendor",
                        column: x => x.VendorId,
                        principalTable: "Vendor",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendorId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductSubCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ApprovalStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ReviewedByAdminId = table.Column<int>(type: "integer", nullable: true),
                    ProductStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ProductVariantStatusId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Product_Approval_Status",
                        column: x => x.ApprovalStatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "ApprovalStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_ProductStatus_ProductStatusId",
                        column: x => x.ProductStatusId,
                        principalTable: "ProductStatus",
                        principalColumn: "ProductStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_ProductVariantStatus_ProductVariantStatusId",
                        column: x => x.ProductVariantStatusId,
                        principalTable: "ProductVariantStatus",
                        principalColumn: "ProductVariantStatusId");
                    table.ForeignKey(
                        name: "FK_Product_Review_By_Admin",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "AdminUser",
                        principalColumn: "AdminUserId");
                    table.ForeignKey(
                        name: "FK_Product_Sub_Category",
                        column: x => x.ProductSubCategoryId,
                        principalTable: "ProductSubCategory",
                        principalColumn: "ProductSubCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vendor_Products",
                        column: x => x.VendorId,
                        principalTable: "Vendor",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorUser",
                columns: table => new
                {
                    VendorUserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendorId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    VendorRoleId = table.Column<int>(type: "integer", nullable: false),
                    AddedByVendorUserId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendor_User", x => x.VendorUserId);
                    table.ForeignKey(
                        name: "FK_Vendor",
                        column: x => x.VendorId,
                        principalTable: "Vendor",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vendor_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vendor_User_Review",
                        column: x => x.AddedByVendorUserId,
                        principalTable: "VendorUser",
                        principalColumn: "VendorUserId");
                    table.ForeignKey(
                        name: "FK_Vendor_User_Role",
                        column: x => x.VendorRoleId,
                        principalTable: "VendorRole",
                        principalColumn: "VendorRoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    RefundId = table.Column<int>(type: "integer", nullable: true),
                    ModeOfPaymentId = table.Column<int>(type: "integer", nullable: false),
                    PaymentGatewayOrderId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PaymentGatewayTransactionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PaymentStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    PaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payment_Mode_Of_Payment",
                        column: x => x.ModeOfPaymentId,
                        principalTable: "ModeOfPayment",
                        principalColumn: "ModeOfPaymentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_Order",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_Refund",
                        column: x => x.RefundId,
                        principalTable: "Refund",
                        principalColumn: "RefundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_Status",
                        column: x => x.PaymentStatusId,
                        principalTable: "PaymentStatus",
                        principalColumn: "PaymentStatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentTracking",
                columns: table => new
                {
                    ShipmentTrackingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShipmentId = table.Column<int>(type: "integer", nullable: false),
                    ShipmentStatusId = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment_Tracking", x => x.ShipmentTrackingId);
                    table.ForeignKey(
                        name: "FK_Shipment_Tracking_Shipment",
                        column: x => x.ShipmentId,
                        principalTable: "Shipment",
                        principalColumn: "ShipmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shipment_Tracking_Status",
                        column: x => x.ShipmentStatusId,
                        principalTable: "ShipmentStatus",
                        principalColumn: "ShipmentStatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CouponsProduct",
                columns: table => new
                {
                    CouponsProductId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CouponId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons_Product", x => x.CouponsProductId);
                    table.ForeignKey(
                        name: "FK_Coupons_Product_Coupon",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "CouponId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Coupons_Product_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariant",
                columns: table => new
                {
                    ProductVariantId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    WeightInKgs = table.Column<decimal>(type: "numeric", maxLength: 15, nullable: false),
                    LengthInCm = table.Column<decimal>(type: "numeric", maxLength: 15, nullable: false),
                    WidthInCm = table.Column<decimal>(type: "numeric", maxLength: 15, nullable: false),
                    HeightInCm = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductVariantStatusId = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Variant", x => x.ProductVariantId);
                    table.ForeignKey(
                        name: "FK_ProductVariant_ProductVariantStatus_ProductVariantStatusId",
                        column: x => x.ProductVariantStatusId,
                        principalTable: "ProductVariantStatus",
                        principalColumn: "ProductVariantStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariant_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItemsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartId = table.Column<int>(type: "integer", nullable: false),
                    ProductVariantId = table.Column<int>(type: "integer", nullable: false),
                    Qunatity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart_Items", x => x.CartItemsId);
                    table.ForeignKey(
                        name: "FK_Cart_Items_Cart",
                        column: x => x.CartId,
                        principalTable: "Cart",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cart_Items_Product_Variant",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariant",
                        principalColumn: "ProductVariantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FavoritesItems",
                columns: table => new
                {
                    FavoritesItemsId = table.Column<int>(type: "integer", nullable: false),
                    FavoritesId = table.Column<int>(type: "integer", nullable: false),
                    ProductVariantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites_Items", x => x.FavoritesItemsId);
                    table.ForeignKey(
                        name: "FK_Favorites_Items_Cart",
                        column: x => x.FavoritesItemsId,
                        principalTable: "Favorites",
                        principalColumn: "FavoritesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Items_Product_Variant",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariant",
                        principalColumn: "ProductVariantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductVariantId = table.Column<int>(type: "integer", nullable: false),
                    AddressId = table.Column<int>(type: "integer", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ReservedQuantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_Inventory_Address",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_Product_Variant",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariant",
                        principalColumn: "ProductVariantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductVariantId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    OrderItemStatusId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Items", x => x.OrderItemsId);
                    table.ForeignKey(
                        name: "FK_Order_Items_Order",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Items_Product_Variant",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariant",
                        principalColumn: "ProductVariantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Items_Status",
                        column: x => x.OrderItemStatusId,
                        principalTable: "OrderItemStatus",
                        principalColumn: "OrderItemStatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    ProductImageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductVariantId = table.Column<int>(type: "integer", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DisplayOrderId = table.Column<int>(type: "integer", nullable: false),
                    IsMainImage = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Image", x => x.ProductImageId);
                    table.ForeignKey(
                        name: "FK_Product_Image_Display_Order",
                        column: x => x.DisplayOrderId,
                        principalTable: "DisplayOrder",
                        principalColumn: "DisplayOrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Image_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Image_Product_Variant",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariant",
                        principalColumn: "ProductVariantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariantAttribute",
                columns: table => new
                {
                    ProductVariantAttributeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductVariantId = table.Column<int>(type: "integer", nullable: false),
                    AttributeMasterId = table.Column<int>(type: "integer", nullable: false),
                    AttributeValue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantAttribute", x => x.ProductVariantAttributeId);
                    table.ForeignKey(
                        name: "FK_ProductVariantAttribute_AttributeMaster_AttributeMasterId",
                        column: x => x.AttributeMasterId,
                        principalTable: "AttributeMaster",
                        principalColumn: "AttributeMasterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariantAttribute_ProductVariant_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariant",
                        principalColumn: "ProductVariantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundItems",
                columns: table => new
                {
                    RefundItemsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderItemsId = table.Column<int>(type: "integer", nullable: false),
                    RefundId = table.Column<int>(type: "integer", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    RefundQuantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refund_Items", x => x.RefundItemsId);
                    table.ForeignKey(
                        name: "FK_Refund_Items_Order_Items",
                        column: x => x.OrderItemsId,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refund_Items_Refund",
                        column: x => x.RefundId,
                        principalTable: "Refund",
                        principalColumn: "RefundId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReturnItems",
                columns: table => new
                {
                    ReturnItemsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReturnId = table.Column<int>(type: "integer", nullable: false),
                    OrderItemsId = table.Column<int>(type: "integer", nullable: false),
                    ReturnQuantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Return_Items", x => x.ReturnItemsId);
                    table.ForeignKey(
                        name: "FK_Return_Items_Order_Items",
                        column: x => x.OrderItemsId,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Return_Items_Return",
                        column: x => x.ReturnId,
                        principalTable: "Return",
                        principalColumn: "ReturnId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderDetailsId = table.Column<int>(type: "integer", nullable: false),
                    ReviewDescriptionId = table.Column<int>(type: "integer", nullable: false),
                    AdditionalReviewDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StarId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Order_Items",
                        column: x => x.OrderDetailsId,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Review_Description",
                        column: x => x.ReviewDescriptionId,
                        principalTable: "ReviewDescription",
                        principalColumn: "ReviewDescriptionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Star",
                        column: x => x.StarId,
                        principalTable: "Star",
                        principalColumn: "StarId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentItems",
                columns: table => new
                {
                    ShipmentItemsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShipmentId = table.Column<int>(type: "integer", nullable: false),
                    OrderIemsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment_Items", x => x.ShipmentItemsId);
                    table.ForeignKey(
                        name: "FK_Shipment_Items_Order_Items",
                        column: x => x.OrderIemsId,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipment_Items_Shipment",
                        column: x => x.ShipmentId,
                        principalTable: "Shipment",
                        principalColumn: "ShipmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AdminRole",
                columns: new[] { "AdminRoleId", "AdminRoleName" },
                values: new object[,]
                {
                    { 1, "Overall_Admin" },
                    { 2, "Vendor_Admin" },
                    { 3, "Product_Admin" },
                    { 4, "Order_Admin" },
                    { 5, "Coupons_Logistic_Admin" },
                    { 6, "Return_Admin" },
                    { 7, "Refund_Admin" },
                    { 8, "Exchange_Admin" },
                    { 9, "Payment_Admin" }
                });

            migrationBuilder.InsertData(
                table: "ApprovalStatus",
                columns: new[] { "ApprovalStatusId", "ApprovalStatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Accepted" },
                    { 3, "Rejected" }
                });

            migrationBuilder.InsertData(
                table: "AttributeMaster",
                columns: new[] { "AttributeMasterId", "AttributeName" },
                values: new object[,]
                {
                    { 1, "Brand" },
                    { 2, "Color" },
                    { 3, "Size" },
                    { 4, "Storage" },
                    { 5, "RAM" },
                    { 6, "Screen Size" },
                    { 7, "Volume" },
                    { 8, "Material" },
                    { 9, "Weight" },
                    { 10, "Skin Type" },
                    { 11, "Capacity" },
                    { 12, "Processor" },
                    { 13, "Operating System" },
                    { 14, "Display Type" },
                    { 15, "Battery Capacity" },
                    { 16, "Gender" },
                    { 17, "Author" }
                });

            migrationBuilder.InsertData(
                table: "DisplayOrder",
                columns: new[] { "DisplayOrderId", "DisplayOrderName" },
                values: new object[,]
                {
                    { 1, "Front" },
                    { 2, "Back" },
                    { 3, "Left" },
                    { 4, "Right" }
                });

            migrationBuilder.InsertData(
                table: "ModeOfPayment",
                columns: new[] { "ModeOfPaymentId", "ModeOfPaymentName" },
                values: new object[,]
                {
                    { 1, "Cash On Delivery" },
                    { 2, "Credit Card" },
                    { 3, "Debit Card" },
                    { 4, "UPI" }
                });

            migrationBuilder.InsertData(
                table: "OrderItemStatus",
                columns: new[] { "OrderItemStatusId", "OrderItemStatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Packed" },
                    { 3, "Processed" },
                    { 4, "Delivered" },
                    { 5, "Delayed" },
                    { 6, "Returned" },
                    { 7, "Cancelled" },
                    { 8, "Refunded" }
                });

            migrationBuilder.InsertData(
                table: "OrderStatus",
                columns: new[] { "OrderStatusId", "OrderStatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Confirmed" },
                    { 3, "Completed" },
                    { 4, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "PaymentStatus",
                columns: new[] { "PaymentStatusId", "PaymentStatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Success" },
                    { 3, "Failed" },
                    { 4, "Refunded" },
                    { 5, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "ProductCategory",
                columns: new[] { "ProductCategoryId", "ProductCategoryName" },
                values: new object[,]
                {
                    { 1, "Electronics" },
                    { 2, "Fashion" },
                    { 3, "Beauty & Personal Care" },
                    { 4, "Home & Kitchen" },
                    { 5, "Books" },
                    { 6, "Sports & Fitness" }
                });

            migrationBuilder.InsertData(
                table: "ProductStatus",
                columns: new[] { "ProductStatusId", "ProductStatusName" },
                values: new object[,]
                {
                    { 1, "Draft" },
                    { 2, "Active" },
                    { 3, "Temporarily_Not_Available" },
                    { 4, "Archived" }
                });

            migrationBuilder.InsertData(
                table: "ProductVariantStatus",
                columns: new[] { "ProductVariantStatusId", "ProductVariantStatusName" },
                values: new object[,]
                {
                    { 1, "Temporarily_Not_Available" },
                    { 2, "Active" },
                    { 4, "Archived" }
                });

            migrationBuilder.InsertData(
                table: "RefundStatus",
                columns: new[] { "RefundStatusId", "RefundStatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Processed" },
                    { 3, "Failed" },
                    { 4, "Cancelled" },
                    { 5, "Completed" }
                });

            migrationBuilder.InsertData(
                table: "ReturnReason",
                columns: new[] { "ReturnReasonId", "ReturnReasonDescription" },
                values: new object[,]
                {
                    { 1, "Damaged Product" },
                    { 2, "Wrong Product Delivered" },
                    { 3, "Product Not As Described" },
                    { 4, "Defective Product" },
                    { 5, "Size Does Not Fit" },
                    { 6, "Color Mismatch" },
                    { 7, "Received Incomplete Item" },
                    { 8, "Ordered By Mistake" },
                    { 9, "Delivery Took Too Long" },
                    { 10, "Found Better Alternative" },
                    { 11, "Quality Not Satisfactory" },
                    { 12, "Changed Mind" }
                });

            migrationBuilder.InsertData(
                table: "ReturnStatus",
                columns: new[] { "ReturnStatusId", "ReturnStatusName" },
                values: new object[,]
                {
                    { 1, "Requested" },
                    { 2, "Approved" },
                    { 3, "Rejected" },
                    { 4, "Pickup_Scheduled" },
                    { 5, "Picked_Up" },
                    { 6, "Received" },
                    { 7, "Inspection_Completed" },
                    { 8, "Refund_Processed" },
                    { 9, "Completed" },
                    { 10, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "ReviewDescription",
                columns: new[] { "ReviewDescriptionId", "ReviewDescriptionName" },
                values: new object[,]
                {
                    { 1, "Excellent Quality" },
                    { 2, "Good Product" },
                    { 3, "Value For Money" },
                    { 4, "Fast Delivery" },
                    { 5, "Packaging Was Good" },
                    { 6, "Looks Same As Image" },
                    { 7, "Product Size Issue" },
                    { 8, "Poor Quality" },
                    { 9, "Damaged Product" },
                    { 10, "Late Delivery" },
                    { 11, "Color Mismatch" },
                    { 12, "Not Worth The Price" },
                    { 13, "Easy To Use" },
                    { 14, "Highly Recommended" },
                    { 15, "Will Buy Again" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" },
                    { 3, "Vendor" }
                });

            migrationBuilder.InsertData(
                table: "ShipmentStatus",
                columns: new[] { "ShipmentStatusId", "ShipmentStatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Picked_Up" },
                    { 3, "In_Transit" },
                    { 4, "Out_Of_Delivery" },
                    { 5, "Delivered" },
                    { 6, "Failed" },
                    { 7, "Returned" }
                });

            migrationBuilder.InsertData(
                table: "Star",
                columns: new[] { "StarId", "StarDescription", "StarName" },
                values: new object[,]
                {
                    { 1, "Very Poor", "1 Star" },
                    { 2, "Poor", "2 Stars" },
                    { 3, "Average", "3 Stars" },
                    { 4, "Good", "4 Stars" },
                    { 5, "Excellent", "5 Stars" }
                });

            migrationBuilder.InsertData(
                table: "VendorRole",
                columns: new[] { "VendorRoleId", "VendorRoleName" },
                values: new object[,]
                {
                    { 1, "Owner" },
                    { 2, "Manager" },
                    { 3, "Product_Manager" },
                    { 4, "Order_Manager" },
                    { 5, "Return_Manager" },
                    { 6, "Refund_Manager" },
                    { 7, "Inventory_Manager" },
                    { 8, "Coupon_Manager" }
                });

            migrationBuilder.InsertData(
                table: "ProductSubCategory",
                columns: new[] { "ProductSubCategoryId", "ProductCategoryId", "ProductSubCategoryName" },
                values: new object[,]
                {
                    { 1, 1, "Mobile Phones" },
                    { 2, 1, "Laptops" },
                    { 3, 1, "Tablets" },
                    { 4, 1, "Smart Watches" },
                    { 5, 1, "Headphones" },
                    { 6, 1, "Speakers" },
                    { 7, 2, "T-Shirts" },
                    { 8, 2, "Shirts" },
                    { 9, 2, "Jeans" },
                    { 10, 2, "Trousers" },
                    { 11, 2, "Shoes" },
                    { 12, 2, "Sandals" },
                    { 13, 3, "Face Wash" },
                    { 14, 3, "Moisturizer" },
                    { 15, 3, "Shampoo" },
                    { 16, 3, "Conditioner" },
                    { 17, 3, "Sunscreen" },
                    { 18, 3, "Perfumes" },
                    { 19, 4, "Cookware" },
                    { 20, 4, "Furniture" },
                    { 21, 4, "Storage Containers" },
                    { 22, 4, "Home Decor" },
                    { 23, 4, "Bedsheets" },
                    { 24, 4, "Kitchen Appliances" },
                    { 25, 5, "Academic Books" },
                    { 26, 5, "Novels" },
                    { 27, 5, "Comics" },
                    { 28, 5, "Biographies" },
                    { 29, 6, "Cricket Equipment" },
                    { 30, 6, "Football Equipment" },
                    { 31, 6, "Gym Equipment" },
                    { 32, 6, "Yoga Accessories" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "CreatedAt", "Email", "FirstName", "HashedKey", "IsActive", "LastName", "Password", "PhoneNumber", "RoleId", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gmail.com", "Siva Kavindra", new byte[] { 120, 30, 22, 141, 127, 82, 6, 22, 219, 181, 145, 194, 164, 179, 8, 133, 31, 11, 77, 17, 6, 118, 224, 58, 117, 33, 177, 5, 87, 78, 216, 254, 209, 147, 78, 90, 42, 153, 228, 31, 164, 112, 202, 153, 131, 16, 177, 2, 66, 152, 143, 110, 180, 182, 231, 94, 60, 237, 158, 91, 86, 197, 5, 182 }, true, "TamilSelvan", new byte[] { 38, 41, 98, 217, 62, 91, 232, 195, 4, 71, 58, 193, 69, 17, 4, 170, 193, 53, 38, 109, 25, 6, 32, 142, 176, 252, 198, 216, 101, 177, 223, 134 }, "9442378188", 1, null });

            migrationBuilder.InsertData(
                table: "AdminUser",
                columns: new[] { "AdminUserId", "AdminRoleId", "AssignedByAdminUserId", "CreatedAt", "IsActive", "UserId" },
                values: new object[] { 1, 1, null, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 1 });

            migrationBuilder.InsertData(
                table: "ProductSubCategoryAttribute",
                columns: new[] { "ProductSubCategoryAttributeId", "AttributeMasterId", "ProductSubCategoryId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 4, 1 },
                    { 4, 5, 1 },
                    { 5, 6, 1 },
                    { 6, 15, 1 },
                    { 7, 1, 2 },
                    { 8, 4, 2 },
                    { 9, 5, 2 },
                    { 10, 12, 2 },
                    { 11, 13, 2 },
                    { 12, 1, 3 },
                    { 13, 2, 3 },
                    { 14, 4, 3 },
                    { 15, 5, 3 },
                    { 16, 6, 3 },
                    { 17, 13, 3 },
                    { 18, 15, 3 },
                    { 19, 1, 4 },
                    { 20, 2, 4 },
                    { 21, 15, 4 },
                    { 22, 1, 5 },
                    { 23, 2, 5 },
                    { 24, 9, 5 },
                    { 25, 15, 5 },
                    { 26, 1, 6 },
                    { 27, 2, 6 },
                    { 28, 9, 6 },
                    { 29, 15, 6 },
                    { 30, 1, 7 },
                    { 31, 2, 7 },
                    { 32, 3, 7 },
                    { 33, 8, 7 },
                    { 34, 16, 7 },
                    { 35, 1, 8 },
                    { 36, 2, 8 },
                    { 37, 3, 8 },
                    { 38, 8, 8 },
                    { 39, 16, 8 },
                    { 40, 1, 9 },
                    { 41, 2, 9 },
                    { 42, 3, 9 },
                    { 43, 8, 9 },
                    { 44, 16, 9 },
                    { 45, 1, 10 },
                    { 46, 2, 10 },
                    { 47, 3, 10 },
                    { 48, 8, 10 },
                    { 49, 16, 10 },
                    { 50, 1, 11 },
                    { 51, 2, 11 },
                    { 52, 3, 11 },
                    { 53, 8, 11 },
                    { 54, 16, 11 },
                    { 55, 1, 12 },
                    { 56, 2, 12 },
                    { 57, 3, 12 },
                    { 58, 8, 12 },
                    { 59, 16, 12 },
                    { 60, 1, 13 },
                    { 61, 7, 13 },
                    { 62, 10, 13 },
                    { 63, 1, 14 },
                    { 64, 7, 14 },
                    { 65, 10, 14 },
                    { 66, 1, 15 },
                    { 67, 7, 15 },
                    { 68, 1, 16 },
                    { 69, 7, 16 },
                    { 70, 1, 17 },
                    { 71, 7, 17 },
                    { 72, 10, 17 },
                    { 73, 1, 18 },
                    { 74, 7, 18 },
                    { 75, 1, 19 },
                    { 76, 8, 19 },
                    { 77, 11, 19 },
                    { 78, 1, 20 },
                    { 79, 8, 20 },
                    { 80, 9, 20 },
                    { 81, 1, 21 },
                    { 82, 8, 21 },
                    { 83, 11, 21 },
                    { 84, 1, 22 },
                    { 85, 8, 22 },
                    { 86, 2, 22 },
                    { 87, 1, 23 },
                    { 88, 2, 23 },
                    { 89, 3, 23 },
                    { 90, 8, 23 },
                    { 91, 1, 24 },
                    { 92, 9, 24 },
                    { 93, 11, 24 },
                    { 94, 1, 25 },
                    { 95, 17, 25 },
                    { 96, 1, 26 },
                    { 97, 17, 26 },
                    { 98, 1, 27 },
                    { 99, 17, 27 },
                    { 100, 1, 28 },
                    { 101, 17, 28 },
                    { 102, 1, 29 },
                    { 103, 8, 29 },
                    { 104, 9, 29 },
                    { 105, 1, 30 },
                    { 106, 8, 30 },
                    { 107, 9, 30 },
                    { 108, 1, 31 },
                    { 109, 8, 31 },
                    { 110, 9, 31 },
                    { 111, 1, 32 },
                    { 112, 8, 32 },
                    { 113, 9, 32 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserId",
                table: "Address",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminRole_AdminRoleName",
                table: "AdminRole",
                column: "AdminRoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_AdminRoleId",
                table: "AdminUser",
                column: "AdminRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_AssignedByAdminUserId",
                table: "AdminUser",
                column: "AssignedByAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_UserId",
                table: "AdminUser",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStatus_ApprovalStatusName",
                table: "ApprovalStatus",
                column: "ApprovalStatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttributeMaster_AttributeName",
                table: "AttributeMaster",
                column: "AttributeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserId",
                table: "Cart",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_ProductVariantId",
                table: "CartItems",
                columns: new[] { "CartId", "ProductVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductVariantId",
                table: "CartItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_CouponCode",
                table: "Coupons",
                column: "CouponCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CouponsProduct_CouponId_ProductId",
                table: "CouponsProduct",
                columns: new[] { "CouponId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CouponsProduct_ProductId",
                table: "CouponsProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponsVendor_CouponId_VendorId",
                table: "CouponsVendor",
                columns: new[] { "CouponId", "VendorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CouponsVendor_VendorId",
                table: "CouponsVendor",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsage_CouponId",
                table: "CouponUsage",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponUsage_OrderId_CouponId",
                table: "CouponUsage",
                columns: new[] { "OrderId", "CouponId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisplayOrder_DisplayOrderName",
                table: "DisplayOrder",
                column: "DisplayOrderName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoritesItems_FavoritesId_ProductVariantId",
                table: "FavoritesItems",
                columns: new[] { "FavoritesId", "ProductVariantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoritesItems_ProductVariantId",
                table: "FavoritesItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_AddressId",
                table: "Inventory",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductVariantId_AddressId",
                table: "Inventory",
                columns: new[] { "ProductVariantId", "AddressId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogChanges_ChangedAt",
                table: "LogChanges",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LogChanges_RecordId",
                table: "LogChanges",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "IX_LogChanges_TableName",
                table: "LogChanges",
                column: "TableName");

            migrationBuilder.CreateIndex(
                name: "IX_LogChanges_UserId",
                table: "LogChanges",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeOfPayment_ModeOfPaymentName",
                table: "ModeOfPayment",
                column: "ModeOfPaymentName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_AddressId",
                table: "Order",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderNumber",
                table: "Order",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderStatusId",
                table: "Order",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                table: "Order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderItemStatusId",
                table: "OrderItems",
                column: "OrderItemStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductVariantId",
                table: "OrderItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemStatus_OrderItemStatusName",
                table: "OrderItemStatus",
                column: "OrderItemStatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatus_OrderStatusName",
                table: "OrderStatus",
                column: "OrderStatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ModeOfPaymentId",
                table: "Payment",
                column: "ModeOfPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrderId",
                table: "Payment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentStatusId",
                table: "Payment",
                column: "PaymentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_RefundId",
                table: "Payment",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentStatus_PaymentStatusName",
                table: "PaymentStatus",
                column: "PaymentStatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_ApprovalStatusId",
                table: "Product",
                column: "ApprovalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductStatusId",
                table: "Product",
                column: "ProductStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductSubCategoryId",
                table: "Product",
                column: "ProductSubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductVariantStatusId",
                table: "Product",
                column: "ProductVariantStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ReviewedByAdminId",
                table: "Product",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_VendorId",
                table: "Product",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_ProductCategoryName",
                table: "ProductCategory",
                column: "ProductCategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_DisplayOrderId",
                table: "ProductImage",
                column: "DisplayOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ImageUrl",
                table: "ProductImage",
                column: "ImageUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductVariantId",
                table: "ProductImage",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStatus_ProductStatusName",
                table: "ProductStatus",
                column: "ProductStatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubCategory_ProductCategoryId_ProductSubCategoryName",
                table: "ProductSubCategory",
                columns: new[] { "ProductCategoryId", "ProductSubCategoryName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubCategoryAttribute_AttributeMasterId",
                table: "ProductSubCategoryAttribute",
                column: "AttributeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubCategoryAttribute_ProductSubCategoryId_AttributeM~",
                table: "ProductSubCategoryAttribute",
                columns: new[] { "ProductSubCategoryId", "AttributeMasterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_ProductId",
                table: "ProductVariant",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_ProductVariantStatusId",
                table: "ProductVariant",
                column: "ProductVariantStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_SKU",
                table: "ProductVariant",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttribute_AttributeMasterId",
                table: "ProductVariantAttribute",
                column: "AttributeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId",
                table: "ProductVariantAttribute",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantStatus_ProductVariantStatusName",
                table: "ProductVariantStatus",
                column: "ProductVariantStatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Refund_OrderId",
                table: "Refund",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Refund_RefundStatusId",
                table: "Refund",
                column: "RefundStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundItems_OrderItemsId",
                table: "RefundItems",
                column: "OrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundItems_RefundId_OrderItemsId",
                table: "RefundItems",
                columns: new[] { "RefundId", "OrderItemsId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefundStatus_RefundStatusName",
                table: "RefundStatus",
                column: "RefundStatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Return_OrderId",
                table: "Return",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Return_ReturnReasonId",
                table: "Return",
                column: "ReturnReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Return_ReturnStatusId",
                table: "Return",
                column: "ReturnStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Return_ReviewedByAdminId",
                table: "Return",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_OrderItemsId",
                table: "ReturnItems",
                column: "OrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_ReturnId_OrderItemsId",
                table: "ReturnItems",
                columns: new[] { "ReturnId", "OrderItemsId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnReason_ReturnReasonDescription",
                table: "ReturnReason",
                column: "ReturnReasonDescription",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnStatus_ReturnStatusName",
                table: "ReturnStatus",
                column: "ReturnStatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewDescription_ReviewDescriptionName",
                table: "ReviewDescription",
                column: "ReviewDescriptionName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_OrderDetailsId",
                table: "Reviews",
                column: "OrderDetailsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewDescriptionId",
                table: "Reviews",
                column: "ReviewDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_StarId",
                table: "Reviews",
                column: "StarId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_RoleName",
                table: "Role",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_OrderId",
                table: "Shipment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_PickupAddressId",
                table: "Shipment",
                column: "PickupAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_ShipmentStatusId",
                table: "Shipment",
                column: "ShipmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_ShipperId",
                table: "Shipment",
                column: "ShipperId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_TrackingNumber",
                table: "Shipment",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentItems_OrderIemsId",
                table: "ShipmentItems",
                column: "OrderIemsId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentItems_ShipmentId_OrderIemsId",
                table: "ShipmentItems",
                columns: new[] { "ShipmentId", "OrderIemsId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentStatus_ShipmentStatusName",
                table: "ShipmentStatus",
                column: "ShipmentStatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTracking_ShipmentId",
                table: "ShipmentTracking",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTracking_ShipmentStatusId",
                table: "ShipmentTracking",
                column: "ShipmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipper_CompanyName",
                table: "Shipper",
                column: "CompanyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipper_CreatedByAdminId",
                table: "Shipper",
                column: "CreatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Star_StarName",
                table: "Star",
                column: "StarName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_PhoneNumber",
                table: "User",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_ApprovalStatusId",
                table: "Vendor",
                column: "ApprovalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_CompanyEmail",
                table: "Vendor",
                column: "CompanyEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_CompanyPhoneNumber",
                table: "Vendor",
                column: "CompanyPhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_GSTNumber",
                table: "Vendor",
                column: "GSTNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_ReviewedByAdminId",
                table: "Vendor",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendor_VendorCompanyName",
                table: "Vendor",
                column: "VendorCompanyName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorRole_VendorRoleName",
                table: "VendorRole",
                column: "VendorRoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorUser_AddedByVendorUserId",
                table: "VendorUser",
                column: "AddedByVendorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorUser_UserId",
                table: "VendorUser",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorUser_VendorId",
                table: "VendorUser",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorUser_VendorRoleId",
                table: "VendorUser",
                column: "VendorRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "CouponsProduct");

            migrationBuilder.DropTable(
                name: "CouponsVendor");

            migrationBuilder.DropTable(
                name: "CouponUsage");

            migrationBuilder.DropTable(
                name: "FavoritesItems");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "LogChanges");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "ProductSubCategoryAttribute");

            migrationBuilder.DropTable(
                name: "ProductVariantAttribute");

            migrationBuilder.DropTable(
                name: "RefundItems");

            migrationBuilder.DropTable(
                name: "ReturnItems");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ShipmentItems");

            migrationBuilder.DropTable(
                name: "ShipmentTracking");

            migrationBuilder.DropTable(
                name: "VendorUser");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "ModeOfPayment");

            migrationBuilder.DropTable(
                name: "PaymentStatus");

            migrationBuilder.DropTable(
                name: "DisplayOrder");

            migrationBuilder.DropTable(
                name: "AttributeMaster");

            migrationBuilder.DropTable(
                name: "Refund");

            migrationBuilder.DropTable(
                name: "Return");

            migrationBuilder.DropTable(
                name: "ReviewDescription");

            migrationBuilder.DropTable(
                name: "Star");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Shipment");

            migrationBuilder.DropTable(
                name: "VendorRole");

            migrationBuilder.DropTable(
                name: "RefundStatus");

            migrationBuilder.DropTable(
                name: "ReturnReason");

            migrationBuilder.DropTable(
                name: "ReturnStatus");

            migrationBuilder.DropTable(
                name: "ProductVariant");

            migrationBuilder.DropTable(
                name: "OrderItemStatus");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Shipper");

            migrationBuilder.DropTable(
                name: "ShipmentStatus");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropTable(
                name: "ProductStatus");

            migrationBuilder.DropTable(
                name: "ProductVariantStatus");

            migrationBuilder.DropTable(
                name: "ProductSubCategory");

            migrationBuilder.DropTable(
                name: "Vendor");

            migrationBuilder.DropTable(
                name: "ProductCategory");

            migrationBuilder.DropTable(
                name: "AdminUser");

            migrationBuilder.DropTable(
                name: "ApprovalStatus");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "AdminRole");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
