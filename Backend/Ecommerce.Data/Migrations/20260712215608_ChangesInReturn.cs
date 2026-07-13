using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInReturn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConvenienceFee",
                table: "Return",
                type: "numeric",
                nullable: true);

            migrationBuilder.InsertData(
                table: "ReturnStatus",
                columns: new[] { "ReturnStatusId", "ReturnStatusName" },
                values: new object[] { 11, "Dispute_Return" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReturnStatus",
                keyColumn: "ReturnStatusId",
                keyValue: 11);

            migrationBuilder.DropColumn(
                name: "ConvenienceFee",
                table: "Return");
        }
    }
}
