using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApprovalHistoryFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalHistory_ApprovalStatus_NewStatusId",
                table: "ApprovalHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalHistory_ApprovalStatus_PreviousStatusId",
                table: "ApprovalHistory");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalHistory_ProductApprovalStatus_NewStatusId",
                table: "ApprovalHistory",
                column: "NewStatusId",
                principalTable: "ProductApprovalStatus",
                principalColumn: "ProductApprovalStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalHistory_ProductApprovalStatus_PreviousStatusId",
                table: "ApprovalHistory",
                column: "PreviousStatusId",
                principalTable: "ProductApprovalStatus",
                principalColumn: "ProductApprovalStatusId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalHistory_ProductApprovalStatus_NewStatusId",
                table: "ApprovalHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalHistory_ProductApprovalStatus_PreviousStatusId",
                table: "ApprovalHistory");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalHistory_ApprovalStatus_NewStatusId",
                table: "ApprovalHistory",
                column: "NewStatusId",
                principalTable: "ApprovalStatus",
                principalColumn: "ApprovalStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalHistory_ApprovalStatus_PreviousStatusId",
                table: "ApprovalHistory",
                column: "PreviousStatusId",
                principalTable: "ApprovalStatus",
                principalColumn: "ApprovalStatusId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
