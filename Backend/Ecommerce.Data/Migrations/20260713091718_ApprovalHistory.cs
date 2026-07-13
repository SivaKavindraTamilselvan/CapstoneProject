using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApprovalHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalHistory_AdminUser_ReviewedByAdminId",
                table: "ApprovalHistory");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalHistory_ReviewedByAdminId",
                table: "ApprovalHistory");

            migrationBuilder.RenameColumn(
                name: "ReviewedByAdminId",
                table: "ApprovalHistory",
                newName: "ReviewerId");

            migrationBuilder.AddColumn<bool>(
                name: "IsPasswordSet",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "Refund",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReviewerType",
                table: "ApprovalHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PasswordSetTokens",
                columns: table => new
                {
                    PasswordSetTokenId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordSetTokens", x => x.PasswordSetTokenId);
                    table.ForeignKey(
                        name: "FK_PasswordSetTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                column: "IsPasswordSet",
                value: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordSetTokens_Token",
                table: "PasswordSetTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordSetTokens_UserId",
                table: "PasswordSetTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordSetTokens");

            migrationBuilder.DropColumn(
                name: "IsPasswordSet",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "Refund");

            migrationBuilder.DropColumn(
                name: "ReviewerType",
                table: "ApprovalHistory");

            migrationBuilder.RenameColumn(
                name: "ReviewerId",
                table: "ApprovalHistory",
                newName: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistory_ReviewedByAdminId",
                table: "ApprovalHistory",
                column: "ReviewedByAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalHistory_AdminUser_ReviewedByAdminId",
                table: "ApprovalHistory",
                column: "ReviewedByAdminId",
                principalTable: "AdminUser",
                principalColumn: "AdminUserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
