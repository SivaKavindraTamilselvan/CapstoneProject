using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReturnDBChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Return_Order",
                table: "Return");

            migrationBuilder.DropTable(
                name: "ReturnItems");

            migrationBuilder.DropIndex(
                name: "IX_Return_OrderId",
                table: "Return");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Return",
                newName: "ReturnQuantity");

            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "Return",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "OrderStatusId",
                table: "Order",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Return_OrderItemId",
                table: "Return",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Return_Order_Item",
                table: "Return",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "OrderItemsId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Return_Order_Item",
                table: "Return");

            migrationBuilder.DropIndex(
                name: "IX_Return_OrderItemId",
                table: "Return");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "Return");

            migrationBuilder.RenameColumn(
                name: "ReturnQuantity",
                table: "Return",
                newName: "OrderId");

            migrationBuilder.AlterColumn<int>(
                name: "OrderStatusId",
                table: "Order",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 1);

            migrationBuilder.CreateTable(
                name: "ReturnItems",
                columns: table => new
                {
                    ReturnItemsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderItemsId = table.Column<int>(type: "integer", nullable: false),
                    ReturnId = table.Column<int>(type: "integer", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Return_OrderId",
                table: "Return",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_OrderItemsId",
                table: "ReturnItems",
                column: "OrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_ReturnId_OrderItemsId",
                table: "ReturnItems",
                columns: new[] { "ReturnId", "OrderItemsId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Return_Order",
                table: "Return",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
