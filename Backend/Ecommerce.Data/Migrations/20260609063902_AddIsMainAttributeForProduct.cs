using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsMainAttributeForProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MainProductSubCategoryAttributeId",
                table: "ProductVariant",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_MainProductSubCategoryAttributeId",
                table: "ProductVariant",
                column: "MainProductSubCategoryAttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Variant_Main_Attribute",
                table: "ProductVariant",
                column: "MainProductSubCategoryAttributeId",
                principalTable: "ProductSubCategoryAttribute",
                principalColumn: "ProductSubCategoryAttributeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Variant_Main_Attribute",
                table: "ProductVariant");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariant_MainProductSubCategoryAttributeId",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "MainProductSubCategoryAttributeId",
                table: "ProductVariant");
        }
    }
}
