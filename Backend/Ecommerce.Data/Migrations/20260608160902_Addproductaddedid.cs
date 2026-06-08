using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addproductaddedid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddedByVendorUserId",
                table: "ProductVariantAttribute",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsExchange",
                table: "ProductVariant",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReturn",
                table: "ProductVariant",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AddedByAdminId",
                table: "ProductSubCategoryAttribute",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AddedByAdminId",
                table: "ProductSubCategory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AddedByAdminId",
                table: "ProductCategory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AddedByAdminId",
                table: "AttributeMaster",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantAttribute_AddedByVendorUserId",
                table: "ProductVariantAttribute",
                column: "AddedByVendorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubCategoryAttribute_AddedByAdminId",
                table: "ProductSubCategoryAttribute",
                column: "AddedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubCategory_AddedByAdminId",
                table: "ProductSubCategory",
                column: "AddedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_AddedByAdminId",
                table: "ProductCategory",
                column: "AddedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeMaster_AddedByAdminId",
                table: "AttributeMaster",
                column: "AddedByAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admin_User_Attribute",
                table: "AttributeMaster",
                column: "AddedByAdminId",
                principalTable: "AdminUser",
                principalColumn: "AdminUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Admin_User_Product_Category",
                table: "ProductCategory",
                column: "AddedByAdminId",
                principalTable: "AdminUser",
                principalColumn: "AdminUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Admin_User_Product_Sub_Category",
                table: "ProductSubCategory",
                column: "AddedByAdminId",
                principalTable: "AdminUser",
                principalColumn: "AdminUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Admin_User_Product_Sub_Category_Attribute",
                table: "ProductSubCategoryAttribute",
                column: "AddedByAdminId",
                principalTable: "AdminUser",
                principalColumn: "AdminUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendor_User_Product_Attribute",
                table: "ProductVariantAttribute",
                column: "AddedByVendorUserId",
                principalTable: "VendorUser",
                principalColumn: "VendorUserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admin_User_Attribute",
                table: "AttributeMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_Admin_User_Product_Category",
                table: "ProductCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Admin_User_Product_Sub_Category",
                table: "ProductSubCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Admin_User_Product_Sub_Category_Attribute",
                table: "ProductSubCategoryAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendor_User_Product_Attribute",
                table: "ProductVariantAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariantAttribute_AddedByVendorUserId",
                table: "ProductVariantAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ProductSubCategoryAttribute_AddedByAdminId",
                table: "ProductSubCategoryAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ProductSubCategory_AddedByAdminId",
                table: "ProductSubCategory");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategory_AddedByAdminId",
                table: "ProductCategory");

            migrationBuilder.DropIndex(
                name: "IX_AttributeMaster_AddedByAdminId",
                table: "AttributeMaster");

            migrationBuilder.DropColumn(
                name: "AddedByVendorUserId",
                table: "ProductVariantAttribute");

            migrationBuilder.DropColumn(
                name: "IsExchange",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "IsReturn",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "AddedByAdminId",
                table: "ProductSubCategoryAttribute");

            migrationBuilder.DropColumn(
                name: "AddedByAdminId",
                table: "ProductSubCategory");

            migrationBuilder.DropColumn(
                name: "AddedByAdminId",
                table: "ProductCategory");

            migrationBuilder.DropColumn(
                name: "AddedByAdminId",
                table: "AttributeMaster");
        }
    }
}
