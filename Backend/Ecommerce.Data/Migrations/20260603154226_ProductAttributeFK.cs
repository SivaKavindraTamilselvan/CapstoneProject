using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProductAttributeFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantAttribute_AttributeMaster_AttributeMasterId",
                table: "ProductVariantAttribute");

            migrationBuilder.RenameColumn(
                name: "AttributeMasterId",
                table: "ProductVariantAttribute",
                newName: "ProductSubCategoryAttributeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId_AttributeMasterId",
                table: "ProductVariantAttribute",
                newName: "IX_ProductVariantAttribute_ProductVariantId_ProductSubCategory~");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariantAttribute_AttributeMasterId",
                table: "ProductVariantAttribute",
                newName: "IX_ProductVariantAttribute_ProductSubCategoryAttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantAttribute_ProductSubCategoryAttribute_Product~",
                table: "ProductVariantAttribute",
                column: "ProductSubCategoryAttributeId",
                principalTable: "ProductSubCategoryAttribute",
                principalColumn: "ProductSubCategoryAttributeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantAttribute_ProductSubCategoryAttribute_Product~",
                table: "ProductVariantAttribute");

            migrationBuilder.RenameColumn(
                name: "ProductSubCategoryAttributeId",
                table: "ProductVariantAttribute",
                newName: "AttributeMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariantAttribute_ProductVariantId_ProductSubCategory~",
                table: "ProductVariantAttribute",
                newName: "IX_ProductVariantAttribute_ProductVariantId_AttributeMasterId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariantAttribute_ProductSubCategoryAttributeId",
                table: "ProductVariantAttribute",
                newName: "IX_ProductVariantAttribute_AttributeMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantAttribute_AttributeMaster_AttributeMasterId",
                table: "ProductVariantAttribute",
                column: "AttributeMasterId",
                principalTable: "AttributeMaster",
                principalColumn: "AttributeMasterId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
