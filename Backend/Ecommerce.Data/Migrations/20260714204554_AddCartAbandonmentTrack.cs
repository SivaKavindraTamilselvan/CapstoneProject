using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCartAbandonmentTrack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifiedAbandoned",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CartItems");

            migrationBuilder.AddColumn<bool>(
                name: "NotifiedAbandoned",
                table: "Cart",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Cart",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifiedAbandoned",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Cart");

            migrationBuilder.AddColumn<bool>(
                name: "NotifiedAbandoned",
                table: "CartItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CartItems",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
