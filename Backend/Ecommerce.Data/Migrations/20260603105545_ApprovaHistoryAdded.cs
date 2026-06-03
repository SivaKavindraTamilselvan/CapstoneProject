using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApprovaHistoryAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Approval_Status",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Review_By_Admin",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Sub_Category",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendor_Products",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "ReviewedByAdminId",
                table: "Product",
                newName: "AdminUserId");

            migrationBuilder.RenameColumn(
                name: "ApprovedAt",
                table: "Product",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "ApprovalStatusId",
                table: "Product",
                newName: "ProductApprovalStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ReviewedByAdminId",
                table: "Product",
                newName: "IX_Product_AdminUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ApprovalStatusId",
                table: "Product",
                newName: "IX_Product_ProductApprovalStatusId");

            migrationBuilder.AddColumn<int>(
                name: "ProductApprovalStatusId",
                table: "ProductVariant",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductVariant",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorUserId",
                table: "ProductVariant",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddedByVendorUserId",
                table: "ProductImage",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductImage",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddedByVendorUserId",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ApprovalHistory",
                columns: table => new
                {
                    ApprovalHistoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    PreviousStatusId = table.Column<int>(type: "integer", nullable: false),
                    NewStatusId = table.Column<int>(type: "integer", nullable: false),
                    ReviewedByAdminId = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    ProductVariantId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalHistory", x => x.ApprovalHistoryId);
                    table.ForeignKey(
                        name: "FK_ApprovalHistory_AdminUser_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "AdminUser",
                        principalColumn: "AdminUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalHistory_ApprovalStatus_NewStatusId",
                        column: x => x.NewStatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "ApprovalStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalHistory_ApprovalStatus_PreviousStatusId",
                        column: x => x.PreviousStatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "ApprovalStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalHistory_ProductVariant_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariant",
                        principalColumn: "ProductVariantId");
                    table.ForeignKey(
                        name: "FK_ApprovalHistory_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "ProductApprovalStatus",
                columns: table => new
                {
                    ProductApprovalStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductApprovalStatusName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductApprovalStatus", x => x.ProductApprovalStatusId);
                });

            migrationBuilder.InsertData(
                table: "ProductApprovalStatus",
                columns: new[] { "ProductApprovalStatusId", "ProductApprovalStatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Vendor_Approved" },
                    { 3, "Vendor_Rejected" },
                    { 4, "Admin_Approved" },
                    { 5, "Admin_Rejected" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_ProductApprovalStatusId",
                table: "ProductVariant",
                column: "ProductApprovalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariant_VendorUserId",
                table: "ProductVariant",
                column: "VendorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_AddedByVendorUserId",
                table: "ProductImage",
                column: "AddedByVendorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_AddedByVendorUserId",
                table: "Product",
                column: "AddedByVendorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistory_NewStatusId",
                table: "ApprovalHistory",
                column: "NewStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistory_PreviousStatusId",
                table: "ApprovalHistory",
                column: "PreviousStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistory_ProductId",
                table: "ApprovalHistory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistory_ProductVariantId",
                table: "ApprovalHistory",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistory_ReviewedByAdminId",
                table: "ApprovalHistory",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductApprovalStatus_ProductApprovalStatusName",
                table: "ProductApprovalStatus",
                column: "ProductApprovalStatusName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Added_Vendor_User",
                table: "Product",
                column: "AddedByVendorUserId",
                principalTable: "VendorUser",
                principalColumn: "VendorUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AdminUser_AdminUserId",
                table: "Product",
                column: "AdminUserId",
                principalTable: "AdminUser",
                principalColumn: "AdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Approval_Status",
                table: "Product",
                column: "ProductApprovalStatusId",
                principalTable: "ProductApprovalStatus",
                principalColumn: "ProductApprovalStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Sub_Category",
                table: "Product",
                column: "ProductSubCategoryId",
                principalTable: "ProductSubCategory",
                principalColumn: "ProductSubCategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendor_Products",
                table: "Product",
                column: "VendorId",
                principalTable: "Vendor",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Images_Added_Vendor_User",
                table: "ProductImage",
                column: "AddedByVendorUserId",
                principalTable: "VendorUser",
                principalColumn: "VendorUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariant_VendorUser_VendorUserId",
                table: "ProductVariant",
                column: "VendorUserId",
                principalTable: "VendorUser",
                principalColumn: "VendorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Approval_Status",
                table: "ProductVariant",
                column: "ProductApprovalStatusId",
                principalTable: "ProductApprovalStatus",
                principalColumn: "ProductApprovalStatusId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Added_Vendor_User",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_AdminUser_AdminUserId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Approval_Status",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Sub_Category",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendor_Products",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Images_Added_Vendor_User",
                table: "ProductImage");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariant_VendorUser_VendorUserId",
                table: "ProductVariant");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Approval_Status",
                table: "ProductVariant");

            migrationBuilder.DropTable(
                name: "ApprovalHistory");

            migrationBuilder.DropTable(
                name: "ProductApprovalStatus");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariant_ProductApprovalStatusId",
                table: "ProductVariant");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariant_VendorUserId",
                table: "ProductVariant");

            migrationBuilder.DropIndex(
                name: "IX_ProductImage_AddedByVendorUserId",
                table: "ProductImage");

            migrationBuilder.DropIndex(
                name: "IX_Product_AddedByVendorUserId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ProductApprovalStatusId",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "VendorUserId",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "AddedByVendorUserId",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "AddedByVendorUserId",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Product",
                newName: "ApprovedAt");

            migrationBuilder.RenameColumn(
                name: "ProductApprovalStatusId",
                table: "Product",
                newName: "ApprovalStatusId");

            migrationBuilder.RenameColumn(
                name: "AdminUserId",
                table: "Product",
                newName: "ReviewedByAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ProductApprovalStatusId",
                table: "Product",
                newName: "IX_Product_ApprovalStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_AdminUserId",
                table: "Product",
                newName: "IX_Product_ReviewedByAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Approval_Status",
                table: "Product",
                column: "ApprovalStatusId",
                principalTable: "ApprovalStatus",
                principalColumn: "ApprovalStatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Review_By_Admin",
                table: "Product",
                column: "ReviewedByAdminId",
                principalTable: "AdminUser",
                principalColumn: "AdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Sub_Category",
                table: "Product",
                column: "ProductSubCategoryId",
                principalTable: "ProductSubCategory",
                principalColumn: "ProductSubCategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendor_Products",
                table: "Product",
                column: "VendorId",
                principalTable: "Vendor",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
