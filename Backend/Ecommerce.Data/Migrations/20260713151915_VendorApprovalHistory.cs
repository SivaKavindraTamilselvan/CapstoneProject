using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class VendorApprovalHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendorApprovalHistory",
                columns: table => new
                {
                    ApprovalHistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    PreviousStatusId = table.Column<int>(type: "integer", nullable: false),
                    NewStatusId = table.Column<int>(type: "integer", nullable: false),
                    ReviewerId = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorApprovalHistory", x => x.ApprovalHistoryId);
                    table.ForeignKey(
                        name: "FK_VendorApprovalHistory_AdminUser_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AdminUser",
                        principalColumn: "AdminUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorApprovalHistory_ApprovalStatus_NewStatusId",
                        column: x => x.NewStatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "ApprovalStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorApprovalHistory_ApprovalStatus_PreviousStatusId",
                        column: x => x.PreviousStatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "ApprovalStatusId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorApprovalHistory_NewStatusId",
                table: "VendorApprovalHistory",
                column: "NewStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorApprovalHistory_PreviousStatusId",
                table: "VendorApprovalHistory",
                column: "PreviousStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorApprovalHistory_ReviewerId",
                table: "VendorApprovalHistory",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorApprovalHistory");
        }
    }
}
