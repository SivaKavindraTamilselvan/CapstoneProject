using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReturnDBChangeFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Return_Admin",
                table: "Return");

            migrationBuilder.AddForeignKey(
                name: "FK_Return_Vendor_Review",
                table: "Return",
                column: "ReviewedByAdminId",
                principalTable: "VendorUser",
                principalColumn: "VendorUserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Return_Vendor_Review",
                table: "Return");

            migrationBuilder.AddForeignKey(
                name: "FK_Return_Admin",
                table: "Return",
                column: "ReviewedByAdminId",
                principalTable: "AdminUser",
                principalColumn: "AdminUserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
