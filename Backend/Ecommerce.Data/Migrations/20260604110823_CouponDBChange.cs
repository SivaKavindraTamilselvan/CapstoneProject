using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class CouponDBChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CouponsVendor");

            migrationBuilder.AddColumn<int>(
                name: "AddedByVendorUserId",
                table: "CouponsProduct",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CouponsProduct",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CouponsProduct",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CouponsProduct",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponDescription",
                table: "Coupons",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByAdminUserId",
                table: "Coupons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByVendorUserId",
                table: "Coupons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Coupons",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CouponsProduct_AddedByVendorUserId",
                table: "CouponsProduct",
                column: "AddedByVendorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_CreatedByAdminUserId",
                table: "Coupons",
                column: "CreatedByAdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_CreatedByVendorUserId",
                table: "Coupons",
                column: "CreatedByVendorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Created_By_Admin_User",
                table: "Coupons",
                column: "CreatedByAdminUserId",
                principalTable: "AdminUser",
                principalColumn: "AdminUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Created_By_Vendor_User",
                table: "Coupons",
                column: "CreatedByVendorUserId",
                principalTable: "VendorUser",
                principalColumn: "VendorUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Product_Added_By_Vendor_User",
                table: "CouponsProduct",
                column: "AddedByVendorUserId",
                principalTable: "VendorUser",
                principalColumn: "VendorUserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Created_By_Admin_User",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Created_By_Vendor_User",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Product_Added_By_Vendor_User",
                table: "CouponsProduct");

            migrationBuilder.DropIndex(
                name: "IX_CouponsProduct_AddedByVendorUserId",
                table: "CouponsProduct");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_CreatedByAdminUserId",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_CreatedByVendorUserId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "AddedByVendorUserId",
                table: "CouponsProduct");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CouponsProduct");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CouponsProduct");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CouponsProduct");

            migrationBuilder.DropColumn(
                name: "CouponDescription",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CreatedByAdminUserId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CreatedByVendorUserId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Coupons");

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

            migrationBuilder.CreateIndex(
                name: "IX_CouponsVendor_CouponId_VendorId",
                table: "CouponsVendor",
                columns: new[] { "CouponId", "VendorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CouponsVendor_VendorId",
                table: "CouponsVendor",
                column: "VendorId");
        }
    }
}
