using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class FavoriteFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Items_Cart",
                table: "FavoritesItems");

            migrationBuilder.AlterColumn<int>(
                name: "FavoritesItemsId",
                table: "FavoritesItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites",
                table: "FavoritesItems",
                column: "FavoritesId",
                principalTable: "Favorites",
                principalColumn: "FavoritesId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites",
                table: "FavoritesItems");

            migrationBuilder.AlterColumn<int>(
                name: "FavoritesItemsId",
                table: "FavoritesItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Items_Cart",
                table: "FavoritesItems",
                column: "FavoritesItemsId",
                principalTable: "Favorites",
                principalColumn: "FavoritesId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
