using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderDBChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShipmentItems_ShipmentId_OrderIemsId",
                table: "ShipmentItems");

            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "ProductVariant");

            migrationBuilder.RenameColumn(
                name: "OrderIemsId",
                table: "ShipmentItems",
                newName: "OrderItemsId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentItems_OrderIemsId",
                table: "ShipmentItems",
                newName: "IX_ShipmentItems_OrderItemsId");

            migrationBuilder.AlterColumn<int>(
                name: "ShipperId",
                table: "Shipment",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "OrderItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentItems_ShipmentId",
                table: "ShipmentItems",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_InventoryId",
                table: "OrderItems",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Inventory_InventoryId",
                table: "OrderItems",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Inventory_InventoryId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentItems_ShipmentId",
                table: "ShipmentItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_InventoryId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "OrderItemsId",
                table: "ShipmentItems",
                newName: "OrderIemsId");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentItems_OrderItemsId",
                table: "ShipmentItems",
                newName: "IX_ShipmentItems_OrderIemsId");

            migrationBuilder.AlterColumn<int>(
                name: "ShipperId",
                table: "Shipment",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AvailableQuantity",
                table: "ProductVariant",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentItems_ShipmentId_OrderIemsId",
                table: "ShipmentItems",
                columns: new[] { "ShipmentId", "OrderIemsId" },
                unique: true);
        }
    }
}
