using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInTheOrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "OrderItemStatus",
                columns: new[] { "OrderItemStatusId", "OrderItemStatusName" },
                values: new object[,]
                {
                    { 9, "Return_Requested" },
                    { 10, "Return_Accepted" },
                    { 11, "Return_Rejected" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderItemStatus",
                keyColumn: "OrderItemStatusId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "OrderItemStatus",
                keyColumn: "OrderItemStatusId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "OrderItemStatus",
                keyColumn: "OrderItemStatusId",
                keyValue: 11);
        }
    }
}
