using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProductDBChangesDone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariant_VendorUser_VendorUserId",
                table: "ProductVariant");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantAttribute_AttributeMaster_AttributeMasterId",
                table: "ProductVariantAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantAttribute_ProductVariant_ProductVariantId",
                table: "ProductVariantAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId",
                table: "ProductVariantAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariant_VendorUserId",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "VendorUserId",
                table: "ProductVariant");

            migrationBuilder.AlterColumn<string>(
                name: "AttributeValue",
                table: "ProductVariantAttribute",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProductVariantAttribute",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "AddedByVendorUserId",
                table: "ProductVariant",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId_AttributeMasterId",
                table: "ProductVariantAttribute",
                columns: new[] { "ProductVariantId", "AttributeMasterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_AddedByVendorUserId",
                table: "ProductVariant",
                column: "AddedByVendorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Variant_Added_Vendor_User",
                table: "ProductVariant",
                column: "AddedByVendorUserId",
                principalTable: "VendorUser",
                principalColumn: "VendorUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantAttribute_AttributeMaster_AttributeMasterId",
                table: "ProductVariantAttribute",
                column: "AttributeMasterId",
                principalTable: "AttributeMaster",
                principalColumn: "AttributeMasterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantAttribute_ProductVariant_ProductVariantId",
                table: "ProductVariantAttribute",
                column: "ProductVariantId",
                principalTable: "ProductVariant",
                principalColumn: "ProductVariantId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Variant_Added_Vendor_User",
                table: "ProductVariant");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantAttribute_AttributeMaster_AttributeMasterId",
                table: "ProductVariantAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantAttribute_ProductVariant_ProductVariantId",
                table: "ProductVariantAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId_AttributeMasterId",
                table: "ProductVariantAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariant_AddedByVendorUserId",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProductVariantAttribute");

            migrationBuilder.DropColumn(
                name: "AddedByVendorUserId",
                table: "ProductVariant");

            migrationBuilder.AlterColumn<string>(
                name: "AttributeValue",
                table: "ProductVariantAttribute",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "VendorUserId",
                table: "ProductVariant",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId",
                table: "ProductVariantAttribute",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_VendorUserId",
                table: "ProductVariant",
                column: "VendorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariant_VendorUser_VendorUserId",
                table: "ProductVariant",
                column: "VendorUserId",
                principalTable: "VendorUser",
                principalColumn: "VendorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantAttribute_AttributeMaster_AttributeMasterId",
                table: "ProductVariantAttribute",
                column: "AttributeMasterId",
                principalTable: "AttributeMaster",
                principalColumn: "AttributeMasterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantAttribute_ProductVariant_ProductVariantId",
                table: "ProductVariantAttribute",
                column: "ProductVariantId",
                principalTable: "ProductVariant",
                principalColumn: "ProductVariantId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
