using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class cancel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CancelReason",
                columns: table => new
                {
                    CancelReasonId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CancelReasonDescription = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelReason", x => x.CancelReasonId);
                });

            migrationBuilder.CreateTable(
                name: "CancelStatus",
                columns: table => new
                {
                    CancelStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CancelStatusName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelStatus", x => x.CancelStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Cancel",
                columns: table => new
                {
                    CancelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CancelReasonId = table.Column<int>(type: "integer", nullable: false),
                    OrderItemId = table.Column<int>(type: "integer", nullable: false),
                    CancelStatusId = table.Column<int>(type: "integer", nullable: false),
                    AdditionalReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CancelledDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CancelQuantity = table.Column<int>(type: "integer", nullable: false),
                    ConvenienceFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cancel", x => x.CancelId);
                    table.ForeignKey(
                        name: "FK_Cancel_CancelReason_CancelReasonId",
                        column: x => x.CancelReasonId,
                        principalTable: "CancelReason",
                        principalColumn: "CancelReasonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cancel_CancelStatus_CancelStatusId",
                        column: x => x.CancelStatusId,
                        principalTable: "CancelStatus",
                        principalColumn: "CancelStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cancel_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CancelRefund",
                columns: table => new
                {
                    CancelRefundId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RefundId = table.Column<int>(type: "integer", nullable: false),
                    CancelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelRefund", x => x.CancelRefundId);
                    table.ForeignKey(
                        name: "FK_CancelRefund_Cancel_CancelId",
                        column: x => x.CancelId,
                        principalTable: "Cancel",
                        principalColumn: "CancelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CancelRefund_Refund_RefundId",
                        column: x => x.RefundId,
                        principalTable: "Refund",
                        principalColumn: "RefundId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CancelReason",
                columns: new[] { "CancelReasonId", "CancelReasonDescription" },
                values: new object[,]
                {
                    { 1, "Ordered by mistake" },
                    { 2, "Found a better price elsewhere" },
                    { 3, "Delivery time is too long" },
                    { 4, "Changed my mind" },
                    { 5, "Incorrect shipping address" },
                    { 6, "Duplicate order placed" },
                    { 7, "Payment issue" },
                    { 8, "Product no longer needed" },
                    { 9, "Want to change product variant" },
                    { 10, "Other" }
                });

            migrationBuilder.InsertData(
                table: "CancelStatus",
                columns: new[] { "CancelStatusId", "CancelStatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Approved" },
                    { 3, "Rejected" },
                    { 4, "Refunded" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cancel_CancelReasonId",
                table: "Cancel",
                column: "CancelReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancel_CancelStatusId",
                table: "Cancel",
                column: "CancelStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancel_OrderItemId",
                table: "Cancel",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CancelReason_CancelReasonDescription",
                table: "CancelReason",
                column: "CancelReasonDescription",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancelRefund_CancelId",
                table: "CancelRefund",
                column: "CancelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancelRefund_RefundId",
                table: "CancelRefund",
                column: "RefundId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CancelStatus_CancelStatusName",
                table: "CancelStatus",
                column: "CancelStatusName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CancelRefund");

            migrationBuilder.DropTable(
                name: "Cancel");

            migrationBuilder.DropTable(
                name: "CancelReason");

            migrationBuilder.DropTable(
                name: "CancelStatus");
        }
    }
}
