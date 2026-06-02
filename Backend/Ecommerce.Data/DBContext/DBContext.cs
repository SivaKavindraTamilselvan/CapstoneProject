using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data;

public class EcommerceContext : DbContext
{
    public EcommerceContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

    }
    public DbSet<User> User { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // master tables

        // roles
        modelBuilder.Entity<Role>(r =>
        {
            r.HasKey(r => r.RoleId).HasName("PK_Role");
            r.HasIndex(r => r.RoleName).IsUnique();
            r.HasData(new Role() { RoleId = 1, RoleName = "Admin" });
            r.HasData(new Role() { RoleId = 2, RoleName = "User" });
            r.HasData(new Role() { RoleId = 3, RoleName = "Vendor" });
        });
        modelBuilder.Entity<AdminRole>(ar =>
        {
            ar.HasKey(ar => ar.AdminRoleId).HasName("PK_Admin_Role");
            ar.HasIndex(ar => ar.AdminRoleName).IsUnique();
            ar.HasData(new AdminRole() { AdminRoleId = 1, AdminRoleName = "Overall_Admin" });
            ar.HasData(new AdminRole() { AdminRoleId = 2, AdminRoleName = "Vendor_Admin" });
            ar.HasData(new AdminRole() { AdminRoleId = 3, AdminRoleName = "Product_Admin" });
            ar.HasData(new AdminRole() { AdminRoleId = 4, AdminRoleName = "Order_Admin" });
            ar.HasData(new AdminRole() { AdminRoleId = 5, AdminRoleName = "Coupons_Logistic_Admin" });
            ar.HasData(new AdminRole() { AdminRoleId = 6, AdminRoleName = "Return_Admin" });
            ar.HasData(new AdminRole() { AdminRoleId = 7, AdminRoleName = "Refund_Admin" });
            ar.HasData(new AdminRole() { AdminRoleId = 8, AdminRoleName = "Exchange_Admin" });
            ar.HasData(new AdminRole() { AdminRoleId = 9, AdminRoleName = "Payment_Admin" });
        });
        modelBuilder.Entity<VendorRole>(vr =>
        {
            vr.HasKey(vr => vr.VendorRoleId).HasName("PK_Vendor_Role");
            vr.HasIndex(vr => vr.VendorRoleName).IsUnique();
            vr.HasData(new VendorRole() { VendorRoleId = 1, VendorRoleName = "Owner" });
            vr.HasData(new VendorRole() { VendorRoleId = 2, VendorRoleName = "Manager" });
            vr.HasData(new VendorRole() { VendorRoleId = 3, VendorRoleName = "Product_Manager" });
            vr.HasData(new VendorRole() { VendorRoleId = 4, VendorRoleName = "Order_Manager" });
            vr.HasData(new VendorRole() { VendorRoleId = 5, VendorRoleName = "Return_Manager" });
            vr.HasData(new VendorRole() { VendorRoleId = 6, VendorRoleName = "Refund_Manager" });
            vr.HasData(new VendorRole() { VendorRoleId = 7, VendorRoleName = "Inventory_Manager" });
            vr.HasData(new VendorRole() { VendorRoleId = 8, VendorRoleName = "Coupon_Manager" });
        });

        // Approval Status For Vendor and Product
        modelBuilder.Entity<ApprovalStatus>(s =>
        {
            s.HasKey(s => s.ApprovalStatusId).HasName("PK_Approval_Status");
            s.HasIndex(s => s.ApprovalStatusName).IsUnique();
            s.HasData(new ApprovalStatus() { ApprovalStatusId = 1, ApprovalStatusName = "Pending" });
            s.HasData(new ApprovalStatus() { ApprovalStatusId = 2, ApprovalStatusName = "Accepted" });
            s.HasData(new ApprovalStatus() { ApprovalStatusId = 3, ApprovalStatusName = "Rejected" });
        });

        // Product Status Is Added 
        modelBuilder.Entity<ProductStatus>(s =>
        {
            s.HasKey(s => s.ProductStatusId).HasName("PK_Product_Status");
            s.Property(s => s.ProductStatusName).IsRequired().HasMaxLength(50);
            s.HasIndex(s => s.ProductStatusName).IsUnique();
            s.HasData(new ProductStatus() { ProductStatusId = 1, ProductStatusName = "Draft" });
            s.HasData(new ProductStatus() { ProductStatusId = 2, ProductStatusName = "Active" });
            s.HasData(new ProductStatus() { ProductStatusId = 3, ProductStatusName = "Temporarily_Not_Available" });
            s.HasData(new ProductStatus() { ProductStatusId = 4, ProductStatusName = "Archived" });
        });
        modelBuilder.Entity<ProductVariantStatus>(s =>
        {
            s.HasKey(s => s.ProductVariantStatusId).HasName("PK_Product_Variant_Status");
            s.Property(s => s.ProductVariantStatusName).IsRequired().HasMaxLength(50);
            s.HasIndex(s => s.ProductVariantStatusName).IsUnique();
            s.HasData(new ProductVariantStatus() { ProductVariantStatusId = 1, ProductVariantStatusName = "Temporarily_Not_Available" });
            s.HasData(new ProductVariantStatus() { ProductVariantStatusId = 2, ProductVariantStatusName = "Active" });
            s.HasData(new ProductVariantStatus() { ProductVariantStatusId = 4, ProductVariantStatusName = "Archived" });
        });
        // display order for the product image
        modelBuilder.Entity<DisplayOrder>(r =>
        {
            r.HasKey(r => r.DisplayOrderId).HasName("PK_Display_Order");
            r.HasIndex(r => r.DisplayOrderName).IsUnique();
            r.HasData(new DisplayOrder() { DisplayOrderId = 1, DisplayOrderName = "Front" });
            r.HasData(new DisplayOrder() { DisplayOrderId = 2, DisplayOrderName = "Back" });
            r.HasData(new DisplayOrder() { DisplayOrderId = 3, DisplayOrderName = "Left" });
            r.HasData(new DisplayOrder() { DisplayOrderId = 4, DisplayOrderName = "Right" });
        });

        // product category and subcategory
        modelBuilder.Entity<ProductCategory>(p =>
        {
            p.HasKey(p => p.ProductCategoryId).HasName("PK_Product_Category");
            p.Property(p => p.ProductCategoryName).IsRequired();
            p.HasIndex(p => p.ProductCategoryName).IsUnique();
            p.HasData(new ProductCategory { ProductCategoryId = 1, ProductCategoryName = "Electronics" });
            p.HasData(new ProductCategory { ProductCategoryId = 2, ProductCategoryName = "Fashion" });
            p.HasData(new ProductCategory { ProductCategoryId = 3, ProductCategoryName = "Beauty & Personal Care" });
            p.HasData(new ProductCategory { ProductCategoryId = 4, ProductCategoryName = "Home & Kitchen" });
            p.HasData(new ProductCategory { ProductCategoryId = 5, ProductCategoryName = "Books" });
            p.HasData(new ProductCategory { ProductCategoryId = 6, ProductCategoryName = "Sports & Fitness" });
        });
        modelBuilder.Entity<ProductSubCategory>(p =>
        {
            p.HasKey(p => p.ProductSubCategoryId).HasName("PK_Product_Sub_Category");
            p.Property(p => p.ProductSubCategoryName).IsRequired();
            p.HasIndex(p => new { p.ProductCategoryId, p.ProductSubCategoryName }).IsUnique();
            p.HasOne(p => p.ProductCategory).WithMany(p => p.ProductSubCategories).HasForeignKey(p => p.ProductCategoryId).HasConstraintName("FK_Product_Category");
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 1, ProductSubCategoryName = "Mobile Phones", ProductCategoryId = 1 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 2, ProductSubCategoryName = "Laptops", ProductCategoryId = 1 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 3, ProductSubCategoryName = "Tablets", ProductCategoryId = 1 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 4, ProductSubCategoryName = "Smart Watches", ProductCategoryId = 1 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 5, ProductSubCategoryName = "Headphones", ProductCategoryId = 1 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 6, ProductSubCategoryName = "Speakers", ProductCategoryId = 1 });

            p.HasData(new ProductSubCategory { ProductSubCategoryId = 7, ProductSubCategoryName = "T-Shirts", ProductCategoryId = 2 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 8, ProductSubCategoryName = "Shirts", ProductCategoryId = 2 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 9, ProductSubCategoryName = "Jeans", ProductCategoryId = 2 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 10, ProductSubCategoryName = "Trousers", ProductCategoryId = 2 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 11, ProductSubCategoryName = "Shoes", ProductCategoryId = 2 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 12, ProductSubCategoryName = "Sandals", ProductCategoryId = 2 });

            p.HasData(new ProductSubCategory { ProductSubCategoryId = 13, ProductSubCategoryName = "Face Wash", ProductCategoryId = 3 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 14, ProductSubCategoryName = "Moisturizer", ProductCategoryId = 3 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 15, ProductSubCategoryName = "Shampoo", ProductCategoryId = 3 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 16, ProductSubCategoryName = "Conditioner", ProductCategoryId = 3 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 17, ProductSubCategoryName = "Sunscreen", ProductCategoryId = 3 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 18, ProductSubCategoryName = "Perfumes", ProductCategoryId = 3 });

            p.HasData(new ProductSubCategory { ProductSubCategoryId = 19, ProductSubCategoryName = "Cookware", ProductCategoryId = 4 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 20, ProductSubCategoryName = "Furniture", ProductCategoryId = 4 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 21, ProductSubCategoryName = "Storage Containers", ProductCategoryId = 4 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 22, ProductSubCategoryName = "Home Decor", ProductCategoryId = 4 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 23, ProductSubCategoryName = "Bedsheets", ProductCategoryId = 4 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 24, ProductSubCategoryName = "Kitchen Appliances", ProductCategoryId = 4 });

            p.HasData(new ProductSubCategory { ProductSubCategoryId = 25, ProductSubCategoryName = "Academic Books", ProductCategoryId = 5 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 26, ProductSubCategoryName = "Novels", ProductCategoryId = 5 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 27, ProductSubCategoryName = "Comics", ProductCategoryId = 5 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 28, ProductSubCategoryName = "Biographies", ProductCategoryId = 5 });

            p.HasData(new ProductSubCategory { ProductSubCategoryId = 29, ProductSubCategoryName = "Cricket Equipment", ProductCategoryId = 6 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 30, ProductSubCategoryName = "Football Equipment", ProductCategoryId = 6 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 31, ProductSubCategoryName = "Gym Equipment", ProductCategoryId = 6 });
            p.HasData(new ProductSubCategory { ProductSubCategoryId = 32, ProductSubCategoryName = "Yoga Accessories", ProductCategoryId = 6 });
        });
        modelBuilder.Entity<AttributeMaster>(a =>
        {
            a.HasKey(a => a.AttributeMasterId).HasName("PK_Product_Attribute_Master");
            a.Property(a => a.AttributeName).IsRequired();
            a.HasIndex(a => a.AttributeName).IsUnique();
            a.HasData(new AttributeMaster { AttributeMasterId = 1, AttributeName = "Brand" });
            a.HasData(new AttributeMaster { AttributeMasterId = 2, AttributeName = "Color" });
            a.HasData(new AttributeMaster { AttributeMasterId = 3, AttributeName = "Size" });
            a.HasData(new AttributeMaster { AttributeMasterId = 4, AttributeName = "Storage" });
            a.HasData(new AttributeMaster { AttributeMasterId = 5, AttributeName = "RAM" });
            a.HasData(new AttributeMaster { AttributeMasterId = 6, AttributeName = "Screen Size" });
            a.HasData(new AttributeMaster { AttributeMasterId = 7, AttributeName = "Volume" });
            a.HasData(new AttributeMaster { AttributeMasterId = 8, AttributeName = "Material" });
            a.HasData(new AttributeMaster { AttributeMasterId = 9, AttributeName = "Weight" });
            a.HasData(new AttributeMaster { AttributeMasterId = 10, AttributeName = "Skin Type" });
            a.HasData(new AttributeMaster { AttributeMasterId = 11, AttributeName = "Capacity" });
            a.HasData(new AttributeMaster { AttributeMasterId = 12, AttributeName = "Processor" });
            a.HasData(new AttributeMaster { AttributeMasterId = 13, AttributeName = "Operating System" });
            a.HasData(new AttributeMaster { AttributeMasterId = 14, AttributeName = "Display Type" });
            a.HasData(new AttributeMaster { AttributeMasterId = 15, AttributeName = "Battery Capacity" });
            a.HasData(new AttributeMaster { AttributeMasterId = 16, AttributeName = "Gender" });
            a.HasData(new AttributeMaster { AttributeMasterId = 17, AttributeName = "Author" });
        });
        modelBuilder.Entity<ProductSubCategoryAttribute>(psa =>
        {
            psa.HasKey(psa => psa.ProductSubCategoryAttributeId).HasName("PK_Product_Sub_Category_Attribute");
            psa.HasOne(psa => psa.ProductSubCategory).WithMany(psc => psc.ProductSubCategoryAttributes).HasForeignKey(psa => psa.ProductSubCategoryId).HasConstraintName("FK_ProductSubCategoryAttribute_ProductSubCategory");
            psa.HasOne(psa => psa.AttributeMaster).WithMany(a => a.ProductSubCategoryAttributes).HasForeignKey(psa => psa.AttributeMasterId).HasConstraintName("FK_ProductSubCategoryAttribute_AttributeMaster");
            psa.HasIndex(psa => new { psa.ProductSubCategoryId, psa.AttributeMasterId }).IsUnique();

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 1, ProductSubCategoryId = 1, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 2, ProductSubCategoryId = 1, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 3, ProductSubCategoryId = 1, AttributeMasterId = 4 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 4, ProductSubCategoryId = 1, AttributeMasterId = 5 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 5, ProductSubCategoryId = 1, AttributeMasterId = 6 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 6, ProductSubCategoryId = 1, AttributeMasterId = 15 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 7, ProductSubCategoryId = 2, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 8, ProductSubCategoryId = 2, AttributeMasterId = 4 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 9, ProductSubCategoryId = 2, AttributeMasterId = 5 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 10, ProductSubCategoryId = 2, AttributeMasterId = 12 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 11, ProductSubCategoryId = 2, AttributeMasterId = 13 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 12, ProductSubCategoryId = 3, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 13, ProductSubCategoryId = 3, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 14, ProductSubCategoryId = 3, AttributeMasterId = 4 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 15, ProductSubCategoryId = 3, AttributeMasterId = 5 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 16, ProductSubCategoryId = 3, AttributeMasterId = 6 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 17, ProductSubCategoryId = 3, AttributeMasterId = 13 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 18, ProductSubCategoryId = 3, AttributeMasterId = 15 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 19, ProductSubCategoryId = 4, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 20, ProductSubCategoryId = 4, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 21, ProductSubCategoryId = 4, AttributeMasterId = 15 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 22, ProductSubCategoryId = 5, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 23, ProductSubCategoryId = 5, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 24, ProductSubCategoryId = 5, AttributeMasterId = 9 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 25, ProductSubCategoryId = 5, AttributeMasterId = 15 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 26, ProductSubCategoryId = 6, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 27, ProductSubCategoryId = 6, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 28, ProductSubCategoryId = 6, AttributeMasterId = 9 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 29, ProductSubCategoryId = 6, AttributeMasterId = 15 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 30, ProductSubCategoryId = 7, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 31, ProductSubCategoryId = 7, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 32, ProductSubCategoryId = 7, AttributeMasterId = 3 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 33, ProductSubCategoryId = 7, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 34, ProductSubCategoryId = 7, AttributeMasterId = 16 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 35, ProductSubCategoryId = 8, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 36, ProductSubCategoryId = 8, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 37, ProductSubCategoryId = 8, AttributeMasterId = 3 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 38, ProductSubCategoryId = 8, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 39, ProductSubCategoryId = 8, AttributeMasterId = 16 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 40, ProductSubCategoryId = 9, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 41, ProductSubCategoryId = 9, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 42, ProductSubCategoryId = 9, AttributeMasterId = 3 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 43, ProductSubCategoryId = 9, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 44, ProductSubCategoryId = 9, AttributeMasterId = 16 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 45, ProductSubCategoryId = 10, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 46, ProductSubCategoryId = 10, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 47, ProductSubCategoryId = 10, AttributeMasterId = 3 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 48, ProductSubCategoryId = 10, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 49, ProductSubCategoryId = 10, AttributeMasterId = 16 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 50, ProductSubCategoryId = 11, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 51, ProductSubCategoryId = 11, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 52, ProductSubCategoryId = 11, AttributeMasterId = 3 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 53, ProductSubCategoryId = 11, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 54, ProductSubCategoryId = 11, AttributeMasterId = 16 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 55, ProductSubCategoryId = 12, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 56, ProductSubCategoryId = 12, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 57, ProductSubCategoryId = 12, AttributeMasterId = 3 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 58, ProductSubCategoryId = 12, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 59, ProductSubCategoryId = 12, AttributeMasterId = 16 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 60, ProductSubCategoryId = 13, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 61, ProductSubCategoryId = 13, AttributeMasterId = 7 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 62, ProductSubCategoryId = 13, AttributeMasterId = 10 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 63, ProductSubCategoryId = 14, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 64, ProductSubCategoryId = 14, AttributeMasterId = 7 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 65, ProductSubCategoryId = 14, AttributeMasterId = 10 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 66, ProductSubCategoryId = 15, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 67, ProductSubCategoryId = 15, AttributeMasterId = 7 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 68, ProductSubCategoryId = 16, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 69, ProductSubCategoryId = 16, AttributeMasterId = 7 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 70, ProductSubCategoryId = 17, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 71, ProductSubCategoryId = 17, AttributeMasterId = 7 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 72, ProductSubCategoryId = 17, AttributeMasterId = 10 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 73, ProductSubCategoryId = 18, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 74, ProductSubCategoryId = 18, AttributeMasterId = 7 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 75, ProductSubCategoryId = 19, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 76, ProductSubCategoryId = 19, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 77, ProductSubCategoryId = 19, AttributeMasterId = 11 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 78, ProductSubCategoryId = 20, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 79, ProductSubCategoryId = 20, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 80, ProductSubCategoryId = 20, AttributeMasterId = 9 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 81, ProductSubCategoryId = 21, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 82, ProductSubCategoryId = 21, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 83, ProductSubCategoryId = 21, AttributeMasterId = 11 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 84, ProductSubCategoryId = 22, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 85, ProductSubCategoryId = 22, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 86, ProductSubCategoryId = 22, AttributeMasterId = 2 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 87, ProductSubCategoryId = 23, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 88, ProductSubCategoryId = 23, AttributeMasterId = 2 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 89, ProductSubCategoryId = 23, AttributeMasterId = 3 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 90, ProductSubCategoryId = 23, AttributeMasterId = 8 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 91, ProductSubCategoryId = 24, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 92, ProductSubCategoryId = 24, AttributeMasterId = 9 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 93, ProductSubCategoryId = 24, AttributeMasterId = 11 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 94, ProductSubCategoryId = 25, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 95, ProductSubCategoryId = 25, AttributeMasterId = 17 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 96, ProductSubCategoryId = 26, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 97, ProductSubCategoryId = 26, AttributeMasterId = 17 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 98, ProductSubCategoryId = 27, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 99, ProductSubCategoryId = 27, AttributeMasterId = 17 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 100, ProductSubCategoryId = 28, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 101, ProductSubCategoryId = 28, AttributeMasterId = 17 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 102, ProductSubCategoryId = 29, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 103, ProductSubCategoryId = 29, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 104, ProductSubCategoryId = 29, AttributeMasterId = 9 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 105, ProductSubCategoryId = 30, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 106, ProductSubCategoryId = 30, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 107, ProductSubCategoryId = 30, AttributeMasterId = 9 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 108, ProductSubCategoryId = 31, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 109, ProductSubCategoryId = 31, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 110, ProductSubCategoryId = 31, AttributeMasterId = 9 });

            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 111, ProductSubCategoryId = 32, AttributeMasterId = 1 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 112, ProductSubCategoryId = 32, AttributeMasterId = 8 });
            psa.HasData(new ProductSubCategoryAttribute { ProductSubCategoryAttributeId = 113, ProductSubCategoryId = 32, AttributeMasterId = 9 });
        });


        modelBuilder.Entity<ModeOfPayment>(r =>
        {
            r.HasKey(r => r.ModeOfPaymentId).HasName("PK_Mode_Of_Payment");
            r.HasIndex(r => r.ModeOfPaymentName).IsUnique();
            r.HasData(new ModeOfPayment() { ModeOfPaymentId = 1, ModeOfPaymentName = "Cash On Delivery" });
            r.HasData(new ModeOfPayment() { ModeOfPaymentId = 2, ModeOfPaymentName = "Credit Card" });
            r.HasData(new ModeOfPayment() { ModeOfPaymentId = 3, ModeOfPaymentName = "Debit Card" });
            r.HasData(new ModeOfPayment() { ModeOfPaymentId = 4, ModeOfPaymentName = "UPI" });
        });

        modelBuilder.Entity<OrderItemStatus>(os =>
        {
            os.HasKey(os => os.OrderItemStatusId).HasName("PK_Order_Items_Status");
            os.HasIndex(os => os.OrderItemStatusName).IsUnique();
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 1, OrderItemStatusName = "Pending" });
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 2, OrderItemStatusName = "Packed" });
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 3, OrderItemStatusName = "Processed" });
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 4, OrderItemStatusName = "Delivered" });
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 5, OrderItemStatusName = "Delayed" });
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 6, OrderItemStatusName = "Returned" });
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 7, OrderItemStatusName = "Cancelled" });
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 8, OrderItemStatusName = "Refunded" });
        });
        modelBuilder.Entity<OrderStatus>(o =>
        {
            o.HasKey(o => o.OrderStatusId).HasName("PK_Order_Status");
            o.HasIndex(o => o.OrderStatusName).IsUnique();
            o.HasData(new OrderStatus() { OrderStatusId = 1, OrderStatusName = "Pending" });
            o.HasData(new OrderStatus() { OrderStatusId = 2, OrderStatusName = "Confirmed" });
            o.HasData(new OrderStatus() { OrderStatusId = 3, OrderStatusName = "Completed" });
            o.HasData(new OrderStatus() { OrderStatusId = 4, OrderStatusName = "Cancelled" });
        });
        modelBuilder.Entity<ShipmentStatus>(sh =>
        {
            sh.HasKey(sh => sh.ShipmentStatusId).HasName("PK_Shipment_Status");
            sh.HasIndex(sh => sh.ShipmentStatusName).IsUnique();
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 1, ShipmentStatusName = "Pending" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 2, ShipmentStatusName = "Picked_Up" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 3, ShipmentStatusName = "In_Transit" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 4, ShipmentStatusName = "Out_Of_Delivery" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 5, ShipmentStatusName = "Delivered" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 6, ShipmentStatusName = "Failed" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 7, ShipmentStatusName = "Returned" });
        });
        modelBuilder.Entity<PaymentStatus>(p =>
        {
            p.HasKey(p => p.PaymentStatusId).HasName("PK_Payment_Status");
            p.HasIndex(p => p.PaymentStatusName).IsUnique();
            p.HasData(new PaymentStatus() { PaymentStatusId = 1, PaymentStatusName = "Pending" });
            p.HasData(new PaymentStatus() { PaymentStatusId = 2, PaymentStatusName = "Success" });
            p.HasData(new PaymentStatus() { PaymentStatusId = 3, PaymentStatusName = "Failed" });
            p.HasData(new PaymentStatus() { PaymentStatusId = 4, PaymentStatusName = "Refunded" });
            p.HasData(new PaymentStatus() { PaymentStatusId = 5, PaymentStatusName = "Cancelled" });
        });
        modelBuilder.Entity<RefundStatus>(p =>
        {
            p.HasKey(p => p.RefundStatusId).HasName("PK_Refund_Status");
            p.HasIndex(p => p.RefundStatusName).IsUnique();
            p.HasData(new RefundStatus() { RefundStatusId = 1, RefundStatusName = "Pending" });
            p.HasData(new RefundStatus() { RefundStatusId = 2, RefundStatusName = "Processed" });
            p.HasData(new RefundStatus() { RefundStatusId = 3, RefundStatusName = "Failed" });
            p.HasData(new RefundStatus() { RefundStatusId = 4, RefundStatusName = "Cancelled" });
            p.HasData(new RefundStatus() { RefundStatusId = 5, RefundStatusName = "Completed" });
        });

        modelBuilder.Entity<ReviewDescription>(rd =>
        {
            rd.HasKey(rd => rd.ReviewDescriptionId).HasName("PK_Review_Description");
            rd.Property(rd => rd.ReviewDescriptionName).IsRequired().HasMaxLength(200);
            rd.HasIndex(rd => rd.ReviewDescriptionName).IsUnique();
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 1, ReviewDescriptionName = "Excellent Quality" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 2, ReviewDescriptionName = "Good Product" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 3, ReviewDescriptionName = "Value For Money" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 4, ReviewDescriptionName = "Fast Delivery" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 5, ReviewDescriptionName = "Packaging Was Good" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 6, ReviewDescriptionName = "Looks Same As Image" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 7, ReviewDescriptionName = "Product Size Issue" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 8, ReviewDescriptionName = "Poor Quality" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 9, ReviewDescriptionName = "Damaged Product" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 10, ReviewDescriptionName = "Late Delivery" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 11, ReviewDescriptionName = "Color Mismatch" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 12, ReviewDescriptionName = "Not Worth The Price" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 13, ReviewDescriptionName = "Easy To Use" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 14, ReviewDescriptionName = "Highly Recommended" });
            rd.HasData(new ReviewDescription() { ReviewDescriptionId = 15, ReviewDescriptionName = "Will Buy Again" });
        });
        modelBuilder.Entity<ReturnStatus>(rs =>
       {
           rs.HasKey(rs => rs.ReturnStatusId).HasName("PK_Return_Status");
           rs.Property(rs => rs.ReturnStatusName).IsRequired().HasMaxLength(100);
           rs.HasIndex(rs => rs.ReturnStatusName).IsUnique();
           rs.HasData(new ReturnStatus() { ReturnStatusId = 1, ReturnStatusName = "Requested" });
           rs.HasData(new ReturnStatus() { ReturnStatusId = 2, ReturnStatusName = "Approved" });
           rs.HasData(new ReturnStatus() { ReturnStatusId = 3, ReturnStatusName = "Rejected" });
           rs.HasData(new ReturnStatus() { ReturnStatusId = 4, ReturnStatusName = "Pickup_Scheduled" });
           rs.HasData(new ReturnStatus() { ReturnStatusId = 5, ReturnStatusName = "Picked_Up" });
           rs.HasData(new ReturnStatus() { ReturnStatusId = 6, ReturnStatusName = "Received" });
           rs.HasData(new ReturnStatus() { ReturnStatusId = 7, ReturnStatusName = "Inspection_Completed" });
           rs.HasData(new ReturnStatus() { ReturnStatusId = 8, ReturnStatusName = "Refund_Processed" });
           rs.HasData(new ReturnStatus() { ReturnStatusId = 9, ReturnStatusName = "Completed" });
           rs.HasData(new ReturnStatus() { ReturnStatusId = 10, ReturnStatusName = "Cancelled" });
       });
        modelBuilder.Entity<Star>(s =>
        {
            s.HasKey(s => s.StarId).HasName("PK_Star");
            s.Property(s => s.StarName).IsRequired().HasMaxLength(20);
            s.Property(s => s.StarDescription).IsRequired().HasMaxLength(200);
            s.HasIndex(s => s.StarName).IsUnique();
            s.HasData(new Star() { StarId = 1, StarName = "1 Star", StarDescription = "Very Poor" });
            s.HasData(new Star() { StarId = 2, StarName = "2 Stars", StarDescription = "Poor" });
            s.HasData(new Star() { StarId = 3, StarName = "3 Stars", StarDescription = "Average" });
            s.HasData(new Star() { StarId = 4, StarName = "4 Stars", StarDescription = "Good" });
            s.HasData(new Star() { StarId = 5, StarName = "5 Stars", StarDescription = "Excellent" });
        });
        modelBuilder.Entity<ReturnReason>(rr =>
        {
            rr.HasKey(rr => rr.ReturnReasonId).HasName("PK_Return_Reason");
            rr.Property(rr => rr.ReturnReasonDescription).IsRequired().HasMaxLength(300);
            rr.HasIndex(rr => rr.ReturnReasonDescription).IsUnique();
            rr.HasData(new ReturnReason() { ReturnReasonId = 1, ReturnReasonDescription = "Damaged Product" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 2, ReturnReasonDescription = "Wrong Product Delivered" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 3, ReturnReasonDescription = "Product Not As Described" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 4, ReturnReasonDescription = "Defective Product" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 5, ReturnReasonDescription = "Size Does Not Fit" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 6, ReturnReasonDescription = "Color Mismatch" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 7, ReturnReasonDescription = "Received Incomplete Item" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 8, ReturnReasonDescription = "Ordered By Mistake" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 9, ReturnReasonDescription = "Delivery Took Too Long" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 10, ReturnReasonDescription = "Found Better Alternative" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 11, ReturnReasonDescription = "Quality Not Satisfactory" });
            rr.HasData(new ReturnReason() { ReturnReasonId = 12, ReturnReasonDescription = "Changed Mind" });
        });

        // Users Tables
        modelBuilder.Entity<User>(u =>
        {
            u.HasKey(u => u.UserId).HasName("PK_User");
            u.HasIndex(u => u.Email).IsUnique();
            u.Property(u => u.Email).IsRequired();
            u.HasIndex(u => u.PhoneNumber).IsUnique();
            u.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(10);
            u.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            u.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            u.Property(u => u.Password).IsRequired();
            u.Property(u => u.HashedKey).IsRequired();
            u.Property(u => u.IsActive).HasDefaultValue(true);
            u.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            u.Property(u => u.CreatedAt).HasColumnType("timestamp without time zone");
            u.Property(u => u.UpdatedAt).HasColumnType("timestamp without time zone");
            u.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId).HasConstraintName("FK_User_Role").OnDelete(DeleteBehavior.Restrict);
            u.HasData(new User() { UserId = 1, FirstName = "Siva Kavindra", LastName = "TamilSelvan", Email = "admin@gmail.com", PhoneNumber = "9442378188", Password = Convert.FromBase64String("Jili2T5b6MMERzrBRREEqsE1Jm0ZBiCOsPzG2GWx34Y="), HashedKey = Convert.FromBase64String("eB4WjX9SBhbbtZHCpLMIhR8LTREGduA6dSGxBVdO2P7Rk05aKpnkH6RwypmDELECQpiPbrS251487Z5bVsUFtg=="), RoleId = 1, CreatedAt = new DateTime(2026, 5, 25), IsActive = true });
        });
        modelBuilder.Entity<Address>(a =>
        {
            a.HasKey(a => a.AddressId).HasName("PK_Address");
            a.Property(a => a.ContactName).IsRequired().HasMaxLength(50);
            a.Property(a => a.ContactPhoneNumber).IsRequired().HasMaxLength(10);
            a.Property(a => a.AddressLine).IsRequired().HasMaxLength(300);
            a.Property(a => a.LandMark).IsRequired().HasMaxLength(150);
            a.Property(a => a.City).IsRequired().HasMaxLength(100);
            a.Property(a => a.State).IsRequired().HasMaxLength(100);
            a.Property(a => a.Country).IsRequired().HasDefaultValue("India");
            a.Property(a => a.PinCode).IsRequired().HasMaxLength(6);
            a.Property(a => a.IsDefault).HasDefaultValue(false);
            a.Property(a => a.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            a.Property(a => a.CreatedAt).HasColumnType("timestamp without time zone");
            a.Property(a => a.UpdatedAt).HasColumnType("timestamp without time zone");
            a.HasOne(u => u.User).WithMany(a => a.Addresses).HasForeignKey(a => a.UserId).HasConstraintName("FK_User_Address").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<AdminUser>(au =>
        {
            au.HasKey(au => au.AdminUserId).HasName("PK_AdminUsers");
            au.Property(au => au.CreatedAt).HasColumnType("timestamp without time zone");
            au.Property(au => au.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            au.HasOne(au => au.User).WithOne(u => u.AdminUsers).HasForeignKey<AdminUser>(au => au.UserId).HasConstraintName("FK_AdminUser_User").OnDelete(DeleteBehavior.Restrict);
            au.HasOne(r => r.AdminRole).WithMany(au => au.AdminUsers).HasForeignKey(au => au.AdminRoleId).HasConstraintName("FK_Admin_Role").OnDelete(DeleteBehavior.Restrict);
            au.HasOne(r => r.AssignedByAdmin).WithMany().HasForeignKey(au => au.AssignedByAdminUserId).HasConstraintName("FK_Admin_Assigned_By_Admin").OnDelete(DeleteBehavior.Restrict);
            au.HasData(new AdminUser() { AdminUserId = 1, UserId = 1, CreatedAt = new DateTime(2026, 5, 1), IsActive = true, AdminRoleId = 1 });
        });
        modelBuilder.Entity<Vendor>(v =>
        {
            v.HasKey(v => v.VendorId).HasName("PK_Vendor");
            v.Property(v => v.ApprovalStatusId).HasDefaultValue(1);
            v.Property(v => v.VendorCompanyName).IsRequired();
            v.HasIndex(v => v.VendorCompanyName).IsUnique();
            v.Property(v => v.CompanyEmail).IsRequired();
            v.HasIndex(v => v.CompanyEmail).IsUnique();
            v.Property(v => v.CompanyPhoneNumber).IsRequired().HasMaxLength(10);
            v.HasIndex(v => v.CompanyPhoneNumber).IsUnique();
            v.Property(v => v.GSTNumber).IsRequired();
            v.HasIndex(v => v.GSTNumber).IsUnique();
            v.Property(v => v.ApprovalStatusId).HasDefaultValue(1);
            v.Property(v => v.CreatedAt).HasColumnType("timestamp without time zone");
            v.Property(v => v.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            v.Property(vu => vu.ReviewedAt).HasColumnType("timestamp without time zone");
            v.HasOne(v => v.ApprovalStatus).WithMany(a => a.Vendors).HasForeignKey(v => v.ApprovalStatusId).HasConstraintName("FK_Vendor_Status");
            v.HasOne(v => v.ReviwedByAdmin).WithMany(a => a.Vendors).HasForeignKey(v => v.ReviewedByAdminId).HasConstraintName("FK_Admin_Review");
        });
        modelBuilder.Entity<VendorUser>(vu =>
        {
            vu.HasKey(vu => vu.VendorUserId).HasName("PK_Vendor_User");
            vu.Property(vu => vu.IsActive).HasDefaultValue(true);
            vu.Property(vu => vu.CreatedAt).HasColumnType("timestamp without time zone");
            vu.Property(vu => vu.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            vu.HasOne(vu => vu.Vendor).WithMany(v => v.VendorUsers).HasForeignKey(vu => vu.VendorId).HasConstraintName("FK_Vendor");
            vu.HasOne(vu => vu.User).WithOne(u => u.VendorUser).HasForeignKey<VendorUser>(vu => vu.UserId).HasConstraintName("FK_Vendor_User");
            vu.HasOne(vu => vu.VendorRole).WithMany(a => a.VendorUsers).HasForeignKey(v => v.VendorRoleId).HasConstraintName("FK_Vendor_User_Role");
            vu.HasOne(v => v.AddedByVendor).WithMany().HasForeignKey(v => v.AddedByVendorUserId).HasConstraintName("FK_Vendor_User_Review");
        });
        modelBuilder.Entity<Shipper>(s =>
        {
            s.HasKey(s => s.ShipperId).HasName("PK_Shipper");
            s.Property(s => s.CompanyName).IsRequired();
            s.HasIndex(s => s.CompanyName).IsUnique();
            s.Property(s => s.APIBaseURL).IsRequired();
            s.Property(s => s.IsActive).HasDefaultValue(true);
            s.HasOne(s => s.CreatedByAdmin).WithMany(au => au.Shippers).HasForeignKey(s => s.CreatedByAdminId).HasConstraintName("FK_Shipper_Admin");
            s.Property(s => s.CreatedAt).HasColumnType("timestamp without time zone");
            s.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Product
        modelBuilder.Entity<Product>(p =>
        {
            p.HasKey(p => p.ProductId).HasName("PK_Product");
            p.Property(p => p.ProductName).IsRequired().HasMaxLength(100);
            p.Property(p => p.Description).IsRequired().HasMaxLength(1000);
            p.Property(p => p.ProductStatusId).HasDefaultValue(1);
            p.Property(p => p.ApprovalStatusId).HasDefaultValue(1);
            p.HasOne(p => p.Vendor).WithMany(v => v.Products).HasForeignKey(p => p.VendorId).HasConstraintName("FK_Vendor_Products");
            p.HasOne(p => p.ProductSubCategory).WithMany(ps => ps.Products).HasForeignKey(p => p.ProductSubCategoryId).HasConstraintName("FK_Product_Sub_Category");
            p.HasOne(p => p.ApprovalStatus).WithMany(p => p.Products).HasForeignKey(p => p.ApprovalStatusId).HasConstraintName("FK_Product_Approval_Status");
            p.HasOne(p => p.ReviewedByAdmin).WithMany(au => au.Products).HasForeignKey(p => p.ReviewedByAdminId).HasConstraintName("FK_Product_Review_By_Admin");
            p.Property(p => p.CreatedAt).HasColumnType("timestamp without time zone");
            p.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            p.Property(p => p.ApprovedAt).HasColumnType("timestamp without time zone");
        });
        modelBuilder.Entity<ProductVariant>(pv =>
        {
            pv.HasKey(pv => pv.ProductVariantId).HasName("PK_Product_Variant");
            pv.Property(pv => pv.AvailableQuantity).HasDefaultValue(0);
            pv.Property(pv => pv.SKU).IsRequired();
            pv.HasIndex(pv => pv.SKU).IsUnique();
            pv.Property(pv => pv.LengthInCm).IsRequired().HasMaxLength(15);
            pv.Property(pv => pv.WidthInCm).IsRequired().HasMaxLength(15);
            pv.Property(pv => pv.WeightInKgs).IsRequired().HasMaxLength(15);
            pv.Property(pv => pv.Price).IsRequired();
            pv.Property(pv => pv.ProductVariantStatusId).HasDefaultValue(1);
            pv.Property(pv => pv.CreatedAt).HasColumnType("timestamp without time zone");
            pv.Property(pv => pv.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        modelBuilder.Entity<ProductImage>(pi =>
        {
            pi.HasKey(pi => pi.ProductImageId).HasName("PK_Product_Image");
            pi.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(1000);
            pi.HasIndex(pi => pi.ImageUrl).IsUnique();
            pi.Property(pi => pi.IsMainImage).HasDefaultValue(false);
            pi.Property(pi => pi.CreatedAt).HasColumnType("timestamp without time zone");
            pi.Property(pi => pi.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            pi.HasOne(pi => pi.Product).WithMany(p => p.ProductImages).HasForeignKey(pi => pi.ProductId).HasConstraintName("FK_Product_Image_Product").OnDelete(DeleteBehavior.Restrict);
            pi.HasOne(pi => pi.ProductVariant).WithMany(pv => pv.ProductImages).HasForeignKey(pi => pi.ProductVariantId).HasConstraintName("FK_Product_Image_Product_Variant").OnDelete(DeleteBehavior.Restrict);
            pi.HasOne(pi => pi.DisplayOrder).WithMany(d => d.ProductImages).HasForeignKey(pi => pi.DisplayOrderId).HasConstraintName("FK_Product_Image_Display_Order").OnDelete(DeleteBehavior.Restrict);
        });

        // Orders

        modelBuilder.Entity<Order>(o =>
        {
            o.HasKey(o => o.OrderId).HasName("PK_Order");
            o.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
            o.HasIndex(o => o.OrderNumber).IsUnique();
            o.Property(o => o.TotalProductAmount).HasDefaultValue(0);
            o.Property(o => o.TotalShippingAmount).HasDefaultValue(0);
            o.Property(o => o.TotalCouponAmount).HasDefaultValue(0);
            o.Property(o => o.FinalAmount).HasDefaultValue(0);
            o.Property(o => o.OrderDate).HasColumnType("timestamp without time zone");
            o.Property(o => o.CreatedAt).HasColumnType("timestamp without time zone");
            o.Property(o => o.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            o.Property(o => o.UpdatedAt).HasColumnType("timestamp without time zone");
            o.HasOne(o => o.Users).WithMany(u => u.Orders).HasForeignKey(o => o.UserId).HasConstraintName("FK_Order_User").OnDelete(DeleteBehavior.Restrict);
            o.HasOne(o => o.Address).WithMany(a => a.Orders).HasForeignKey(o => o.AddressId).HasConstraintName("FK_Order_Address").OnDelete(DeleteBehavior.Restrict);
            o.HasOne(o => o.OrderStatus).WithMany(os => os.Orders).HasForeignKey(o => o.OrderStatusId).HasConstraintName("FK_Order_Status").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<OrderItems>(oi =>
        {
            oi.HasKey(oi => oi.OrderItemsId).HasName("PK_Order_Items");
            oi.Property(oi => oi.Quantity).IsRequired();
            oi.Property(oi => oi.UnitPrice).IsRequired();
            oi.Property(oi => oi.Discount).HasDefaultValue(0);
            oi.HasOne(oi => oi.Order).WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId).HasConstraintName("FK_Order_Items_Order").OnDelete(DeleteBehavior.Restrict);
            oi.HasOne(oi => oi.ProductVariant).WithMany(pv => pv.OrderItems).HasForeignKey(oi => oi.ProductVariantId).HasConstraintName("FK_Order_Items_Product_Variant").OnDelete(DeleteBehavior.Restrict);
            oi.HasOne(oi => oi.OrderItemStatus).WithMany(os => os.OrderItems).HasForeignKey(oi => oi.OrderItemStatusId).HasConstraintName("FK_Order_Items_Status").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Cart>(c =>
        {
            c.HasKey(c => c.CartId).HasName("PK_Cart");
            c.HasIndex(c => c.UserId).IsUnique();
            c.HasOne(c => c.Users).WithOne(u => u.Cart).HasForeignKey<Cart>(c => c.UserId).HasConstraintName("FK_Cart_User").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<CartItems>(ci =>
        {
            ci.HasKey(ci => ci.CartItemsId).HasName("PK_Cart_Items");
            ci.Property(ci => ci.Qunatity).HasDefaultValue(1);
            ci.HasIndex(ci => new { ci.CartId, ci.ProductVariantId }).IsUnique();
            ci.HasOne(ci => ci.Cart).WithMany(c => c.CartItems).HasForeignKey(ci => ci.CartId).HasConstraintName("FK_Cart_Items_Cart").OnDelete(DeleteBehavior.Cascade);
            ci.HasOne(ci => ci.ProductVariant).WithMany(pv => pv.CartItems).HasForeignKey(ci => ci.ProductVariantId).HasConstraintName("FK_Cart_Items_Product_Variant").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Favorites>(c =>
        {
            c.HasKey(c => c.FavoritesId).HasName("PK_Favorites");
            c.HasIndex(c => c.UserId).IsUnique();
            c.HasOne(c => c.Users).WithOne(u => u.Favorites).HasForeignKey<Favorites>(c => c.UserId).HasConstraintName("FK_Favorites_User").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<FavoritesItems>(ci =>
        {
            ci.HasKey(ci => ci.FavoritesItemsId).HasName("PK_Favorites_Items");
            ci.HasIndex(ci => new { ci.FavoritesId, ci.ProductVariantId }).IsUnique();
            ci.HasOne(ci => ci.Favorites).WithMany(c => c.FavoritesItems).HasForeignKey(ci => ci.FavoritesId).HasConstraintName("FK_Favorites").OnDelete(DeleteBehavior.Cascade);
            ci.HasOne(ci => ci.ProductVariant).WithMany(pv => pv.FavoritesItems).HasForeignKey(ci => ci.ProductVariantId).HasConstraintName("FK_Favorites_Items_Product_Variant").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Payment>(p =>
        {
            p.HasKey(p => p.PaymentId).HasName("PK_Payment");
            p.Property(p => p.PaymentStatusId).HasDefaultValue(1);
            p.Property(p => p.PaymentGatewayOrderId).HasMaxLength(200);
            p.Property(p => p.PaymentGatewayTransactionId).HasMaxLength(200);
            p.Property(p => p.FailureReason).HasMaxLength(500);
            p.Property(p => p.Amount).IsRequired();
            p.Property(p => p.PaymentDate).HasColumnType("timestamp without time zone");
            p.Property(p => p.CreatedAt).HasColumnType("timestamp without time zone");
            p.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            p.Property(p => p.UpdatedAt).HasColumnType("timestamp without time zone");
            p.HasOne(p => p.Order).WithMany(o => o.Payments).HasForeignKey(p => p.OrderId).HasConstraintName("FK_Payment_Order").OnDelete(DeleteBehavior.Restrict);
            p.HasOne(p => p.Refund).WithMany(r => r.Payments).HasForeignKey(p => p.RefundId).HasConstraintName("FK_Payment_Refund").OnDelete(DeleteBehavior.Restrict);
            p.HasOne(p => p.ModeOfPayment).WithMany(m => m.Payments).HasForeignKey(p => p.ModeOfPaymentId).HasConstraintName("FK_Payment_Mode_Of_Payment").OnDelete(DeleteBehavior.Restrict);
            p.HasOne(p => p.PaymentStatus).WithMany(ps => ps.Payments).HasForeignKey(p => p.PaymentStatusId).HasConstraintName("FK_Payment_Status").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Shipment>(s =>
        {
            s.HasKey(s => s.ShipmentId).HasName("PK_Shipment");
            s.Property(s => s.ShipmentStatusId).HasDefaultValue(1);
            s.Property(s => s.TrackingNumber).IsRequired().HasMaxLength(200);
            s.HasIndex(s => s.TrackingNumber).IsUnique();
            s.Property(s => s.ShippingCharge).HasDefaultValue(0);
            s.Property(s => s.ExpectedDeliveryDate).HasColumnType("timestamp without time zone");
            s.Property(s => s.ShippedDate).HasColumnType("timestamp without time zone");
            s.Property(s => s.DeliveryDate).HasColumnType("timestamp without time zone");
            s.Property(s => s.CreatedAt).HasColumnType("timestamp without time zone");
            s.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            s.HasOne(s => s.Shipper).WithMany(sh => sh.Shipments).HasForeignKey(s => s.ShipperId).HasConstraintName("FK_Shipment_Shipper").OnDelete(DeleteBehavior.Restrict);
            s.HasOne(s => s.Order).WithMany(o => o.Shipments).HasForeignKey(s => s.OrderId).HasConstraintName("FK_Shipment_Order").OnDelete(DeleteBehavior.Restrict);
            s.HasOne(s => s.PickupAddress).WithMany(a => a.Shipments).HasForeignKey(s => s.PickupAddressId).HasConstraintName("FK_Shipment_Pickup_Address").OnDelete(DeleteBehavior.Restrict);
            s.HasOne(s => s.ShipmentStatus).WithMany(ss => ss.Shipments).HasForeignKey(s => s.ShipmentStatusId).HasConstraintName("FK_Shipment_Status").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<ShipmentItems>(si =>
        {
            si.HasKey(si => si.ShipmentItemsId).HasName("PK_Shipment_Items");
            si.HasIndex(si => new { si.ShipmentId, si.OrderIemsId }).IsUnique();
            si.HasOne(si => si.Shipment).WithMany(s => s.ShipmentItems).HasForeignKey(si => si.ShipmentId).HasConstraintName("FK_Shipment_Items_Shipment").OnDelete(DeleteBehavior.Cascade);
            si.HasOne(si => si.OrderItems).WithMany(oi => oi.ShipmentItems).HasForeignKey(si => si.OrderIemsId).HasConstraintName("FK_Shipment_Items_Order_Items").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<ShipmentTracking>(st =>
        {
            st.HasKey(st => st.ShipmentTrackingId).HasName("PK_Shipment_Tracking");
            st.Property(st => st.Location).IsRequired().HasMaxLength(300);
            st.Property(st => st.Remarks).HasMaxLength(1000);
            st.Property(st => st.UpdatedAt).HasColumnType("timestamp without time zone");
            st.Property(st => st.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            st.HasOne(st => st.Shipment).WithMany(s => s.ShipmentTrackings).HasForeignKey(st => st.ShipmentId).HasConstraintName("FK_Shipment_Tracking_Shipment").OnDelete(DeleteBehavior.Cascade);
            st.HasOne(st => st.ShipmentStatus).WithMany(ss => ss.ShipmentTrackings).HasForeignKey(st => st.ShipmentStatusId).HasConstraintName("FK_Shipment_Tracking_Status").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Inventory>(i =>
        {
            i.HasKey(i => i.InventoryId).HasName("PK_Inventory");
            i.Property(i => i.AvailableQuantity).HasDefaultValue(0);
            i.Property(i => i.ReservedQuantity).HasDefaultValue(0);
            i.Property(i => i.UpdatedAt).HasColumnType("timestamp without time zone");
            i.Property(i => i.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            i.HasIndex(i => new { i.ProductVariantId, i.AddressId }).IsUnique();
            i.HasOne(i => i.ProductVariant).WithMany(pv => pv.Inventories).HasForeignKey(i => i.ProductVariantId).HasConstraintName("FK_Inventory_Product_Variant").OnDelete(DeleteBehavior.Restrict);
            i.HasOne(i => i.Address).WithMany(a => a.Inventories).HasForeignKey(i => i.AddressId).HasConstraintName("FK_Inventory_Address").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Reviews>(r =>
        {
            r.HasKey(r => r.ReviewId).HasName("PK_Reviews");
            r.Property(r => r.AdditionalReviewDescription).HasMaxLength(1000);
            r.Property(r => r.CreatedAt).HasColumnType("timestamp without time zone");
            r.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            r.HasIndex(r => r.OrderDetailsId).IsUnique();
            r.HasOne(r => r.OrderItems).WithOne(oi => oi.Reviews).HasForeignKey<Reviews>(r => r.OrderDetailsId).HasConstraintName("FK_Reviews_Order_Items").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.ReviewDescription).WithMany(rd => rd.Reviews).HasForeignKey(r => r.ReviewDescriptionId).HasConstraintName("FK_Reviews_Review_Description").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.Star).WithMany(s => s.Reviews).HasForeignKey(r => r.StarId).HasConstraintName("FK_Reviews_Star").OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Return>(r =>
        {
            r.HasKey(r => r.ReturnId).HasName("PK_Return");
            r.Property(r => r.ReturnStatusId).HasDefaultValue(1);
            r.Property(r => r.AdditionalReason).HasMaxLength(1000);
            r.Property(r => r.RequestedDate).HasColumnType("timestamp without time zone");
            r.Property(r => r.RequestedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            r.Property(r => r.ReviewedDate).HasColumnType("timestamp without time zone");
            r.HasOne(r => r.ReturnReason).WithMany(rr => rr.Returns).HasForeignKey(r => r.ReturnReasonId).HasConstraintName("FK_Return_Reason").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.Order).WithMany(o => o.Returns).HasForeignKey(r => r.OrderId).HasConstraintName("FK_Return_Order").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.ReturnStatus).WithMany(rs => rs.Returns).HasForeignKey(r => r.ReturnStatusId).HasConstraintName("FK_Return_Status").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.ReviewedByAdmin).WithMany(a => a.Returns).HasForeignKey(r => r.ReviewedByAdminId).HasConstraintName("FK_Return_Admin").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<ReturnItems>(ri =>
        {
            ri.HasKey(ri => ri.ReturnItemsId).HasName("PK_Return_Items");
            ri.Property(ri => ri.ReturnQuantity).HasDefaultValue(1);
            ri.HasIndex(ri => new { ri.ReturnId, ri.OrderItemsId }).IsUnique();
            ri.HasOne(ri => ri.Return).WithMany(r => r.ReturnItems).HasForeignKey(ri => ri.ReturnId).HasConstraintName("FK_Return_Items_Return").OnDelete(DeleteBehavior.Cascade);
            ri.HasOne(ri => ri.OrderItems).WithMany(oi => oi.ReturnItems).HasForeignKey(ri => ri.OrderItemsId).HasConstraintName("FK_Return_Items_Order_Items").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Refund>(r =>
        {
            r.HasKey(r => r.RefundId).HasName("PK_Refund");
            r.Property(r => r.RefundStatusId).HasDefaultValue(1);
            r.Property(r => r.RequestedDate).HasColumnType("timestamp without time zone");
            r.Property(r => r.RequestedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            r.Property(r => r.ProcessedDate).HasColumnType("timestamp without time zone");
            r.HasOne(r => r.Order).WithMany(o => o.Refunds).HasForeignKey(r => r.OrderId).HasConstraintName("FK_Refund_Order").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.RefundStatus).WithMany(rs => rs.Refunds).HasForeignKey(r => r.RefundStatusId).HasConstraintName("FK_Refund_Status").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<RefundItems>(ri =>
        {
            ri.HasKey(ri => ri.RefundItemsId).HasName("PK_Refund_Items");
            ri.Property(ri => ri.RefundAmount).IsRequired();
            ri.Property(ri => ri.RefundQuantity).HasDefaultValue(1);
            ri.HasIndex(ri => new { ri.RefundId, ri.OrderItemsId }).IsUnique();
            ri.HasOne(ri => ri.Refund).WithMany(r => r.RefundItems).HasForeignKey(ri => ri.RefundId).HasConstraintName("FK_Refund_Items_Refund").OnDelete(DeleteBehavior.Cascade);
            ri.HasOne(ri => ri.OrderItems).WithMany(oi => oi.RefundItems).HasForeignKey(ri => ri.OrderItemsId).HasConstraintName("FK_Refund_Items_Order_Items").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Coupons>(c =>
        {
            c.HasKey(c => c.CouponId).HasName("PK_Coupons");
            c.Property(c => c.CouponCode).IsRequired().HasMaxLength(100);
            c.HasIndex(c => c.CouponCode).IsUnique();
            c.Property(c => c.DiscountValue).IsRequired();
            c.Property(c => c.MinimumOrderAmount).HasDefaultValue(0);
            c.Property(c => c.MinimumNumberOfUsage).HasDefaultValue(1);
            c.Property(c => c.IsActive).HasDefaultValue(true);
            c.Property(c => c.StartDate).HasColumnType("timestamp without time zone");
            c.Property(c => c.EndDate).HasColumnType("timestamp without time zone");
            c.Property(c => c.CreatedAt).HasColumnType("timestamp without time zone");
            c.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        modelBuilder.Entity<CouponUsage>(cu =>
        {
            cu.HasKey(cu => cu.CouponUsageId).HasName("PK_Coupon_Usage");
            cu.Property(cu => cu.UsedAt).HasColumnType("timestamp without time zone");
            cu.Property(cu => cu.UsedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            cu.HasIndex(cu => new { cu.OrderId, cu.CouponId }).IsUnique();
            cu.HasOne(cu => cu.Coupons).WithMany(c => c.CouponUsages).HasForeignKey(cu => cu.CouponId).HasConstraintName("FK_Coupon_Usage_Coupon").OnDelete(DeleteBehavior.Restrict);
            cu.HasOne(cu => cu.Order).WithMany(o => o.CouponUsages).HasForeignKey(cu => cu.OrderId).HasConstraintName("FK_Coupon_Usage_Order").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<CouponsProduct>(cp =>
        {
            cp.HasKey(cp => cp.CouponsProductId).HasName("PK_Coupons_Product");
            cp.HasIndex(cp => new { cp.CouponId, cp.ProductId }).IsUnique();
            cp.HasOne(cp => cp.Coupons).WithMany(c => c.CouponsProducts).HasForeignKey(cp => cp.CouponId).HasConstraintName("FK_Coupons_Product_Coupon").OnDelete(DeleteBehavior.Cascade);
            cp.HasOne(cp => cp.Product).WithMany(p => p.CouponsProducts).HasForeignKey(cp => cp.ProductId).HasConstraintName("FK_Coupons_Product_Product").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<CouponsVendor>(cv =>
        {
            cv.HasKey(cv => cv.CouponsVendorId).HasName("PK_Coupons_Vendor");
            cv.HasIndex(cv => new { cv.CouponId, cv.VendorId }).IsUnique();
            cv.HasOne(cv => cv.Coupons).WithMany(c => c.CouponsVendors).HasForeignKey(cv => cv.CouponId).HasConstraintName("FK_Coupons_Vendor_Coupon").OnDelete(DeleteBehavior.Cascade);
            cv.HasOne(cv => cv.Vendor).WithMany(v => v.CouponsVendors).HasForeignKey(cv => cv.VendorId).HasConstraintName("FK_Coupons_Vendor_Vendor").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<LogChanges>(lc =>
        {
            lc.HasKey(lc => lc.LogChangesId).HasName("PK_Log_Changes");
            lc.Property(lc => lc.TableName).IsRequired().HasMaxLength(200);
            lc.Property(lc => lc.Actions).IsRequired().HasMaxLength(50);
            lc.Property(lc => lc.OldValue).HasColumnType("text");
            lc.Property(lc => lc.NewValue).HasColumnType("text");
            lc.Property(lc => lc.ChangedAt).HasColumnType("timestamp without time zone");
            lc.Property(lc => lc.ChangedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            lc.HasIndex(lc => lc.TableName);
            lc.HasIndex(lc => lc.RecordId);
            lc.HasIndex(lc => lc.ChangedAt);
            lc.HasOne(lc => lc.Users).WithMany(u => u.LogChanges).HasForeignKey(lc => lc.UserId).HasConstraintName("FK_Log_Changes_User").OnDelete(DeleteBehavior.Restrict);
        });
    }
}