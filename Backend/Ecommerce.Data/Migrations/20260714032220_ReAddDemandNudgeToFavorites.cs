using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReAddDemandNudgeToFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FavoritesItems",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastNotifiedAt",
                table: "FavoritesItems",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LastNotifiedPrice",
                table: "FavoritesItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NotificationCount",
                table: "FavoritesItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FavoritesItems");

            migrationBuilder.DropColumn(
                name: "LastNotifiedAt",
                table: "FavoritesItems");

            migrationBuilder.DropColumn(
                name: "LastNotifiedPrice",
                table: "FavoritesItems");

            migrationBuilder.DropColumn(
                name: "NotificationCount",
                table: "FavoritesItems");
        }
    }
}
