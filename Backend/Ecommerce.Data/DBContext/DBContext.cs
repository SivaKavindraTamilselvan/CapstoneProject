using System.Security.Cryptography.X509Certificates;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data;

public class EcommerceContext : DbContext
{
    public EcommerceContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

    }
    public DbSet<AdminRole> AdminRoles { get; set; }
    public DbSet<Cancel> Cancel { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Vendor> Vendor { get; set; }
    public DbSet<VendorUser> VendorUser { get; set; }
    public DbSet<AdminUser> AdminUser { get; set; }
    public DbSet<Address> Address { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<ProductCategory> ProductCategory { get; set; }
    public DbSet<ProductSubCategory> ProductSubCategory { get; set; }
    public DbSet<ProductSubCategoryAttribute> ProductSubCategoryAttribute { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductVariant> ProductVariant { get; set; }
    public DbSet<ProductImage> ProductImage { get; set; }
    public DbSet<AttributeMaster> AttributeMaster { get; set; }
    public DbSet<Cart> Cart { get; set; }
    public DbSet<CartItems> CartItems { get; set; }
    public DbSet<Favorites> Favorites { get; set; }
    public DbSet<FavoritesItems> FavoritesItems { get; set; }
    public DbSet<Coupons> Coupons { get; set; }
    public DbSet<CouponsProduct> CouponsProduct { get; set; }
    public DbSet<OrderItems> OrderItems { get; set; }
    public DbSet<ShipmentItems> ShipmentItems { get; set; }
    public DbSet<Shipment> Shipment { get; set; }
    public DbSet<Return> Return { get; set; }
    public DbSet<Inventory> Inventory { get; set; }
    public DbSet<CouponUsage> CouponUsage { get; set; }
    public DbSet<Refund> Refund { get; set; }
    public DbSet<CancelRefund> CancelRefund { get; set; }
    public DbSet<ProductVariantAttribute> ProductVariantAttribute { get; set; }
    public DbSet<Notification> Notification { get; set; }
    public DbSet<NotificationType> NotificationType { get; set; }
    public DbSet<Reviews> Reviews { get; set; }
    public DbSet<ApprovalHistory> ApprovalHistory { get; set; }
    public DbSet<PasswordSetToken> PasswordSetTokens { get; set; }
    public DbSet<IdempotencyKey> IdempotencyKeys { get; set; }



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
            s.HasData(new ApprovalStatus() { ApprovalStatusId = 4, ApprovalStatusName = "Deleted_By_Admin" });
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
        modelBuilder.Entity<ProductApprovalStatus>(entity =>
        {
            entity.HasKey(p => p.ProductApprovalStatusId);
            entity.Property(p => p.ProductApprovalStatusName).IsRequired().HasMaxLength(50);
            entity.HasIndex(p => p.ProductApprovalStatusName).IsUnique();
            entity.HasData(new ProductApprovalStatus { ProductApprovalStatusId = 1, ProductApprovalStatusName = "Pending" });
            entity.HasData(new ProductApprovalStatus { ProductApprovalStatusId = 2, ProductApprovalStatusName = "Vendor_Approved" });
            entity.HasData(new ProductApprovalStatus { ProductApprovalStatusId = 3, ProductApprovalStatusName = "Vendor_Rejected" });
            entity.HasData(new ProductApprovalStatus { ProductApprovalStatusId = 4, ProductApprovalStatusName = "Admin_Approved" });
            entity.HasData(new ProductApprovalStatus { ProductApprovalStatusId = 5, ProductApprovalStatusName = "Admin_Rejected" });
            entity.HasData(new ProductApprovalStatus { ProductApprovalStatusId = 6, ProductApprovalStatusName = "Deleted_By_Admin" });
        });
        // product category and subcategory
        modelBuilder.Entity<ProductCategory>(p =>
        {
            p.HasKey(p => p.ProductCategoryId).HasName("PK_Product_Category");
            p.Property(p => p.ProductCategoryId).ValueGeneratedOnAdd();
            p.Property(p => p.ProductCategoryName).IsRequired();
            p.HasIndex(p => p.ProductCategoryName).IsUnique();
            p.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
            p.Property(p => p.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            p.HasOne(v => v.AddedByAdminUser).WithMany(a => a.ProductCategories).HasForeignKey(v => v.AddedByAdminId).HasConstraintName("FK_Admin_User_Product_Category");

        });
        modelBuilder.Entity<ProductSubCategory>(p =>
        {
            p.HasKey(p => p.ProductSubCategoryId).HasName("PK_Product_Sub_Category");
            p.Property(p => p.ProductSubCategoryId).ValueGeneratedOnAdd();
            p.Property(p => p.ProductSubCategoryName).IsRequired();
            p.Property(p => p.ProductCategoryId).IsRequired();
            p.Property(p => p.CommissionPercentage).IsRequired();
            p.HasIndex(p => new { p.ProductCategoryId, p.ProductSubCategoryName }).IsUnique();
            p.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
            p.Property(p => p.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            p.HasOne(p => p.ProductCategory).WithMany(p => p.ProductSubCategories).HasForeignKey(p => p.ProductCategoryId).HasConstraintName("FK_Product_Category");
            p.HasOne(v => v.AddedByAdminUser).WithMany(a => a.ProductSubCategories).HasForeignKey(v => v.AddedByAdminId).HasConstraintName("FK_Admin_User_Product_Sub_Category");

        });
        modelBuilder.Entity<ProductSubCategoryAttribute>(psa =>
        {
            psa.HasKey(psa => psa.ProductSubCategoryAttributeId).HasName("PK_Product_Sub_Category_Attribute");
            psa.Property(p => p.ProductSubCategoryAttributeId).ValueGeneratedOnAdd();
            psa.HasIndex(psa => new { psa.ProductSubCategoryId, psa.AttributeMasterId }).IsUnique();
            psa.Property(psa => psa.IsActive).IsRequired().HasDefaultValue(true);
            psa.Property(psa => psa.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            psa.HasOne(psa => psa.ProductSubCategory).WithMany(psc => psc.ProductSubCategoryAttributes).HasForeignKey(psa => psa.ProductSubCategoryId).HasConstraintName("FK_ProductSubCategoryAttribute_ProductSubCategory");
            psa.HasOne(psa => psa.AttributeMaster).WithMany(a => a.ProductSubCategoryAttributes).HasForeignKey(psa => psa.AttributeMasterId).HasConstraintName("FK_ProductSubCategoryAttribute_AttributeMaster");
            psa.HasOne(v => v.AddedByAdminUser).WithMany(a => a.ProductSubCategoryAttributes).HasForeignKey(v => v.AddedByAdminId).HasConstraintName("FK_Admin_User_Product_Sub_Category_Attribute");
        });
        modelBuilder.Entity<AttributeMaster>(a =>
        {
            a.HasKey(a => a.AttributeMasterId).HasName("PK_Product_Attribute_Master");
            a.Property(p => p.AttributeMasterId).ValueGeneratedOnAdd();
            a.Property(a => a.AttributeName).IsRequired();
            a.HasIndex(a => a.AttributeName).IsUnique();
            a.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
            a.Property(a => a.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            a.HasOne(v => v.AddedByAdminUser).WithMany(a => a.AttributeMasters).HasForeignKey(v => v.AddedByAdminId).HasConstraintName("FK_Admin_User_Attribute");
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
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 9, OrderItemStatusName = "Return_Requested" });
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 10, OrderItemStatusName = "Return_Accepted" });
            os.HasData(new OrderItemStatus() { OrderItemStatusId = 11, OrderItemStatusName = "Return_Rejected" });
        });
        modelBuilder.Entity<OrderStatus>(o =>
        {
            o.HasKey(o => o.OrderStatusId).HasName("PK_Order_Status");
            o.HasIndex(o => o.OrderStatusName).IsUnique();
            o.HasData(new OrderStatus() { OrderStatusId = 1, OrderStatusName = "Pending" });
            o.HasData(new OrderStatus() { OrderStatusId = 2, OrderStatusName = "Confirmed" });
            o.HasData(new OrderStatus() { OrderStatusId = 3, OrderStatusName = "Completed" });
            o.HasData(new OrderStatus() { OrderStatusId = 4, OrderStatusName = "Cancelled" });
            o.HasData(new OrderStatus() { OrderStatusId = 5, OrderStatusName = "Failed" });
        });
        modelBuilder.Entity<ShipmentStatus>(sh =>
        {
            sh.HasKey(sh => sh.ShipmentStatusId).HasName("PK_Shipment_Status");
            sh.HasIndex(sh => sh.ShipmentStatusName).IsUnique();
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 1, ShipmentStatusName = "Pending" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 2, ShipmentStatusName = "Payment_Success" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 3, ShipmentStatusName = "Payment_Failed" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 4, ShipmentStatusName = "Ready_For_Pick_Up" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 5, ShipmentStatusName = "Picked_Up" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 6, ShipmentStatusName = "In_Transit" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 7, ShipmentStatusName = "Out_Of_Delivery" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 8, ShipmentStatusName = "Delivered" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 9, ShipmentStatusName = "Failed" });
            sh.HasData(new ShipmentStatus() { ShipmentStatusId = 10, ShipmentStatusName = "Returned" });
        });
        modelBuilder.Entity<ShipmentType>(sh =>
        {
            sh.HasKey(sh => sh.ShipmentTypeId).HasName("PK_Shipment_Type");
            sh.HasIndex(sh => sh.ShipmentTypeName).IsUnique();
            sh.HasData(new ShipmentType() { ShipmentTypeId = 1, ShipmentTypeName = "Order" });
            sh.HasData(new ShipmentType() { ShipmentTypeId = 2, ShipmentTypeName = "Return" });
            sh.HasData(new ShipmentType() { ShipmentTypeId = 3, ShipmentTypeName = "Exchange" });
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
            p.HasData(new RefundStatus() { RefundStatusId = 1, RefundStatusName = "Vendor_Requested" });
            p.HasData(new RefundStatus() { RefundStatusId = 2, RefundStatusName = "Pending" });
            p.HasData(new RefundStatus() { RefundStatusId = 3, RefundStatusName = "Admin_Reviewed" });
            p.HasData(new RefundStatus() { RefundStatusId = 4, RefundStatusName = "Processed" });
            p.HasData(new RefundStatus() { RefundStatusId = 5, RefundStatusName = "Failed" });
            p.HasData(new RefundStatus() { RefundStatusId = 6, RefundStatusName = "Cancelled" });
            p.HasData(new RefundStatus() { RefundStatusId = 7, RefundStatusName = "Completed" });
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
           rs.HasData(new ReturnStatus() { ReturnStatusId = 11, ReturnStatusName = "Dispute_Return" });
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
            a.Property(a => a.IsActive).HasDefaultValue(true);
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
        // Product
        modelBuilder.Entity<Product>(p =>
        {
            p.HasKey(p => p.ProductId).HasName("PK_Product");

            p.Property(p => p.VendorId).IsRequired();
            p.HasOne(p => p.Vendor).WithMany(v => v.Products).HasForeignKey(p => p.VendorId).HasConstraintName("FK_Vendor_Products").OnDelete(DeleteBehavior.Restrict);

            p.Property(p => p.ProductName).IsRequired().HasMaxLength(100);
            p.Property(p => p.Description).IsRequired().HasMaxLength(1000);

            p.Property(pv => pv.MainProductSubCategoryAttributeId).IsRequired();
            p.HasOne(p => p.MainProductSubCategoryAttribute).WithMany(a => a.Products).HasForeignKey(p => p.MainProductSubCategoryAttributeId).HasConstraintName("FK_Product_Main_Attribute").OnDelete(DeleteBehavior.Restrict);


            p.Property(p => p.ProductSubCategoryId).IsRequired();
            p.HasOne(p => p.ProductSubCategory).WithMany(ps => ps.Products).HasForeignKey(p => p.ProductSubCategoryId).HasConstraintName("FK_Product_Sub_Category").OnDelete(DeleteBehavior.Restrict);

            p.Property(p => p.ProductApprovalStatusId).HasDefaultValue(1);
            p.HasOne(p => p.ProductApprovalStatus).WithMany(p => p.Products).HasForeignKey(p => p.ProductApprovalStatusId).HasConstraintName("FK_Product_Approval_Status").OnDelete(DeleteBehavior.Restrict);

            p.Property(p => p.AddedByVendorUserId).IsRequired();
            p.HasOne(p => p.AddedByVendorUser).WithMany(a => a.Products).HasForeignKey(p => p.AddedByVendorUserId).HasConstraintName("FK_Product_Added_Vendor_User").OnDelete(DeleteBehavior.Restrict);

            p.Property(p => p.ProductStatusId).HasDefaultValue(1);
            p.HasOne(p => p.ProductStatus).WithMany(ps => ps.Products).HasForeignKey(p => p.ProductStatusId).HasConstraintName("FK_Product_Status").OnDelete(DeleteBehavior.Restrict);

            p.Property(p => p.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            p.Property(p => p.UpdatedAt).HasColumnType("timestamp without time zone");

        });
        modelBuilder.Entity<ProductVariant>(pv =>
        {
            pv.HasKey(pv => pv.ProductVariantId).HasName("PK_Product_Variant");

            pv.Property(pv => pv.ProductId).IsRequired();
            pv.HasOne(pv => pv.Product).WithMany(p => p.ProductVariants).HasForeignKey(pv => pv.ProductId).HasConstraintName("FK_Product_Variant_Product").OnDelete(DeleteBehavior.Restrict);

            pv.Property(pv => pv.SKU).IsRequired();
            pv.HasIndex(pv => pv.SKU).IsUnique();

            pv.Property(pv => pv.Price).IsRequired();
            pv.Property(pv => pv.IsExchange).IsRequired();
            pv.Property(pv => pv.IsReturn).IsRequired();

            pv.Property(pv => pv.WeightInKgs).IsRequired().HasMaxLength(15);
            pv.Property(pv => pv.LengthInCm).IsRequired().HasMaxLength(15);
            pv.Property(pv => pv.WidthInCm).IsRequired().HasMaxLength(15);
            pv.Property(pv => pv.HeightInCm).IsRequired().HasMaxLength(15);

            pv.Property(pv => pv.AddedByVendorUserId).IsRequired();
            pv.HasOne(p => p.AddedByVendorUser).WithMany(a => a.ProductVariants).HasForeignKey(p => p.AddedByVendorUserId).HasConstraintName("FK_Product_Variant_Added_Vendor_User").OnDelete(DeleteBehavior.Restrict);

            pv.Property(pv => pv.ProductVariantStatusId).HasDefaultValue(1);
            pv.HasOne(pv => pv.ProductVariantStatus).WithMany(ps => ps.ProductVariants).HasForeignKey(pv => pv.ProductVariantStatusId).HasConstraintName("FK_Product_Variant_Status").OnDelete(DeleteBehavior.Restrict);

            pv.Property(p => p.ProductApprovalStatusId).HasDefaultValue(1);
            pv.HasOne(p => p.ProductApprovalStatus).WithMany(p => p.ProductVariants).HasForeignKey(p => p.ProductApprovalStatusId).HasConstraintName("FK_Product_Approval_Status").OnDelete(DeleteBehavior.Restrict);

            pv.Property(pv => pv.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            pv.Property(pv => pv.UpdatedAt).HasColumnType("timestamp without time zone");

        });
        modelBuilder.Entity<ProductImage>(pi =>
        {
            pi.HasKey(pi => pi.ProductImageId).HasName("PK_Product_Image");

            pi.Property(pi => pi.ProductId).IsRequired();
            pi.HasOne(pi => pi.Product).WithMany(p => p.ProductImages).HasForeignKey(pi => pi.ProductId).HasConstraintName("FK_Product_Image_Product").OnDelete(DeleteBehavior.Restrict);

            pi.HasOne(pi => pi.ProductVariant).WithMany(pv => pv.ProductImages).HasForeignKey(pi => pi.ProductVariantId).HasConstraintName("FK_Product_Image_Product_Variant").OnDelete(DeleteBehavior.Restrict);

            pi.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(1000);
            pi.HasIndex(pi => pi.ImageUrl).IsUnique();

            pi.Property(pi => pi.DisplayOrderId).IsRequired();
            pi.HasOne(pi => pi.DisplayOrder).WithMany(d => d.ProductImages).HasForeignKey(pi => pi.DisplayOrderId).HasConstraintName("FK_Product_Image_Display_Order").OnDelete(DeleteBehavior.Restrict);

            pi.Property(pi => pi.AddedByVendorUserId).IsRequired();
            pi.HasOne(pi => pi.AddedByVendorUser).WithMany(a => a.ProductImages).HasForeignKey(p => p.AddedByVendorUserId).HasConstraintName("FK_Product_Images_Added_Vendor_User").OnDelete(DeleteBehavior.Restrict);

            pi.Property(pi => pi.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            pi.Property(pi => pi.UpdatedAt).HasColumnType("timestamp without time zone");
        });
        modelBuilder.Entity<ProductVariantAttribute>(pva =>
        {
            pva.HasKey(pva => pva.ProductVariantAttributeId);

            pva.HasIndex(pva => new { pva.ProductVariantId, pva.ProductSubCategoryAttributeId }).IsUnique();

            pva.Property(pva => pva.ProductVariantId).IsRequired();
            pva.HasOne(pva => pva.ProductVariant).WithMany(pv => pv.ProductVariantAttributes).HasForeignKey(pva => pva.ProductVariantId).OnDelete(DeleteBehavior.Restrict);

            pva.Property(pva => pva.ProductSubCategoryAttributeId).IsRequired();
            pva.HasOne(pva => pva.ProductSubCategoryAttribute).WithMany(am => am.ProductVariantAttributes).HasForeignKey(pva => pva.ProductSubCategoryAttributeId).OnDelete(DeleteBehavior.Restrict);

            pva.Property(pva => pva.IsActive).IsRequired().HasDefaultValue(true);
            pva.Property(pva => pva.AttributeValue).IsRequired().HasMaxLength(100);
            pva.Property(pva => pva.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            pva.Property(pva => pva.UpdatedAt).HasColumnType("timestamp without time zone");

            pva.HasOne(v => v.AddedByVendorUser).WithMany(a => a.ProductVariantAttributes).HasForeignKey(v => v.AddedByVendorUserId).HasConstraintName("FK_Vendor_User_Product_Attribute");

        });
        modelBuilder.Entity<ApprovalHistory>(entity =>
        {
            entity.HasKey(ah => ah.ApprovalHistoryId);

            entity.Property(ah => ah.EntityType).IsRequired().HasMaxLength(50);
            entity.Property(ah => ah.EntityId).IsRequired();

            entity.Property(ah => ah.PreviousStatusId).IsRequired();
            entity.HasOne(ah => ah.PreviousStatus).WithMany(ps => ps.PreviousApprovalHistories).HasForeignKey(ah => ah.PreviousStatusId).OnDelete(DeleteBehavior.Restrict);

            entity.Property(ah => ah.NewStatusId).IsRequired();
            entity.HasOne(ah => ah.NewStatus).WithMany(ps => ps.NewApprovalHistories).HasForeignKey(ah => ah.NewStatusId).OnDelete(DeleteBehavior.Restrict);

            entity.Property(ah => ah.ReviewerId).IsRequired();
            entity.Property(ah => ah.ReviewerType).IsRequired();

            entity.Property(ah => ah.Remarks).HasMaxLength(500);
            entity.Property(ah => ah.ReviewedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
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
            o.Property(o => o.OrderStatusId).HasDefaultValue(1);
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
            oi.HasOne(oi => oi.Inventory).WithMany(i => i.OrderItems).HasForeignKey(oi => oi.InventoryId).OnDelete(DeleteBehavior.Restrict);
            oi.HasOne(oi => oi.Order).WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId).HasConstraintName("FK_Order_Items_Order").OnDelete(DeleteBehavior.Restrict);
            oi.HasOne(oi => oi.ProductVariant).WithMany(pv => pv.OrderItems).HasForeignKey(oi => oi.ProductVariantId).HasConstraintName("FK_Order_Items_Product_Variant").OnDelete(DeleteBehavior.Restrict);
            oi.HasOne(oi => oi.OrderItemStatus).WithMany(os => os.OrderItems).HasForeignKey(oi => oi.OrderItemStatusId).HasConstraintName("FK_Order_Items_Status").OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IdempotencyKey>()
    .HasIndex(i => i.Key)
    .IsUnique();


        modelBuilder.Entity<PasswordSetToken>()
        .HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PasswordSetToken>().HasIndex(p => p.Token).IsUnique();
        
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
            s.Property(s => s.TrackingNumber).IsRequired(false).HasMaxLength(200);
            s.HasIndex(s => s.TrackingNumber).IsUnique();
            s.Property(s => s.CourierName).IsRequired().HasDefaultValue("unassigned");
            s.Property(s => s.ShippingCharge).HasDefaultValue(0);
            s.Property(s => s.ExpectedDeliveryDate).HasColumnType("timestamp without time zone");
            s.Property(s => s.ShippedDate).HasColumnType("timestamp without time zone");
            s.Property(s => s.DeliveryDate).HasColumnType("timestamp without time zone");
            s.Property(s => s.CreatedAt).HasColumnType("timestamp without time zone");
            s.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            s.HasOne(s => s.Order).WithMany(o => o.Shipments).HasForeignKey(s => s.OrderId).HasConstraintName("FK_Shipment_Order").OnDelete(DeleteBehavior.Restrict);
            s.HasOne(s => s.PickupAddress).WithMany(a => a.Shipments).HasForeignKey(s => s.PickupAddressId).HasConstraintName("FK_Shipment_Pickup_Address").OnDelete(DeleteBehavior.Restrict);
            s.HasOne(s => s.ShipmentStatus).WithMany(ss => ss.Shipments).HasForeignKey(s => s.ShipmentStatusId).HasConstraintName("FK_Shipment_Status").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<ShipmentItems>(si =>
        {
            si.HasKey(si => si.ShipmentItemsId).HasName("PK_Shipment_Items");
            si.HasOne(si => si.Shipment).WithMany(s => s.ShipmentItems).HasForeignKey(si => si.ShipmentId).HasConstraintName("FK_Shipment_Items_Shipment").OnDelete(DeleteBehavior.Cascade);
            si.HasOne(si => si.OrderItems).WithMany(oi => oi.ShipmentItems).HasForeignKey(si => si.OrderItemsId).HasConstraintName("FK_Shipment_Items_Order_Items").OnDelete(DeleteBehavior.Restrict);
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
            i.Property(i => i.IsActive).HasDefaultValue(true);
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
            r.Property(r => r.ReturnQuantity).IsRequired();
            r.Property(r => r.RequestedDate).HasColumnType("timestamp without time zone");
            r.Property(r => r.RequestedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            r.Property(r => r.ReviewedDate).HasColumnType("timestamp without time zone");
            r.HasOne(r => r.ReturnReason).WithMany(rr => rr.Returns).HasForeignKey(r => r.ReturnReasonId).HasConstraintName("FK_Return_Reason").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.OrderItems).WithMany(o => o.Returns).HasForeignKey(r => r.OrderItemId).HasConstraintName("FK_Return_Order_Item").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.ReturnStatus).WithMany(rs => rs.Returns).HasForeignKey(r => r.ReturnStatusId).HasConstraintName("FK_Return_Status").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.ReviewedByVendor).WithMany(a => a.Returns).HasForeignKey(r => r.ReviewedByVendorId).HasConstraintName("FK_Return_Vendor_Review").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Refund>(r =>
        {
            r.HasKey(r => r.RefundId).HasName("PK_Refund");
            r.Property(r => r.RefundStatusId).HasDefaultValue(1);
            r.Property(r => r.RequestedDate).HasColumnType("timestamp without time zone");
            r.Property(r => r.RequestedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            r.Property(r => r.ProcessedDate).HasColumnType("timestamp without time zone");
            r.HasOne(r => r.RefundType).WithMany(o => o.Refunds).HasForeignKey(r => r.RefundTypeId).HasConstraintName("FK_Refund_Type").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.OrderItems).WithMany(o => o.Refunds).HasForeignKey(r => r.OrderItemsId).HasConstraintName("FK_Refund_Order_Item").OnDelete(DeleteBehavior.Restrict);
            r.HasOne(r => r.RefundStatus).WithMany(rs => rs.Refunds).HasForeignKey(r => r.RefundStatusId).HasConstraintName("FK_Refund_Status").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<ReturnRefund>(r =>
        {
            r.HasKey(r => r.ReturnRefundId).HasName("PK_Return_Refund");
            r.Property(r => r.DeductionReason).HasMaxLength(250);
            r.HasOne(r => r.Refund).WithOne(r => r.ReturnRefund).HasForeignKey<ReturnRefund>(r => r.RefundId).HasConstraintName("FK_Return_Refund");
            r.HasOne(r => r.Return).WithOne(r => r.ReturnRefund).HasForeignKey<ReturnRefund>(r => r.ReturnId).HasConstraintName("FK_Return_Refund_Id");
        });
        modelBuilder.Entity<RefundType>(r =>
        {
            r.HasKey(r => r.RefundTypeId).HasName("PK_RefundType");
            r.Property(r => r.RefundTypeName).HasMaxLength(100);
            r.HasIndex(r => r.RefundTypeName).IsUnique();
            r.HasData(new RefundType { RefundTypeId = 1, RefundTypeName = "Cancellation" }, new RefundType { RefundTypeId = 2, RefundTypeName = "Return" });
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
            c.Property(c => c.CouponDescription).IsRequired().HasMaxLength(150);
            c.Property(c => c.StartDate).HasColumnType("timestamp without time zone");
            c.Property(c => c.EndDate).HasColumnType("timestamp without time zone");
            c.Property(c => c.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            c.Property(c => c.UpdatedAt).HasColumnType("timestamp without time zone");
            c.HasOne(c => c.CreatedByUser).WithMany(u => u.Coupons).HasForeignKey(c => c.CreatedByUserId).HasConstraintName("FK_Coupons_Created_By_User").OnDelete(DeleteBehavior.Restrict);
            c.HasOne(c => c.CouponType).WithMany(u => u.Coupons).HasForeignKey(c => c.CouponTypeId).HasConstraintName("FK_Coupons_Type").OnDelete(DeleteBehavior.Restrict);
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
            cp.Property(cp => cp.IsActive).HasDefaultValue(true);
            cp.Property(cp => cp.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            cp.Property(cp => cp.UpdatedAt).HasColumnType("timestamp without time zone");
            cp.HasOne(cp => cp.Coupons).WithMany(c => c.CouponsProducts).HasForeignKey(cp => cp.CouponId).HasConstraintName("FK_Coupons_Product_Coupon").OnDelete(DeleteBehavior.Cascade);
            cp.HasOne(cp => cp.Product).WithMany(p => p.CouponsProducts).HasForeignKey(cp => cp.ProductId).HasConstraintName("FK_Coupons_Product_Product").OnDelete(DeleteBehavior.Restrict);
            cp.HasOne(cp => cp.AddedByVendorUser).WithMany().HasForeignKey(cp => cp.AddedByVendorUserId).HasConstraintName("FK_Coupons_Product_Added_By_Vendor_User").OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<CouponType>(c =>
        {
            c.HasKey(c => c.CouponTypeId).HasName("PK_Coupon_Type");
            c.HasIndex(c => c.CouponTypeName).IsUnique();
            c.Property(c => c.CouponTypeName).IsRequired();
            c.HasData(new CouponType() { CouponTypeId = 1, CouponTypeName = "Admin" });
            c.HasData(new CouponType() { CouponTypeId = 2, CouponTypeName = "Vendor" });
        });
        modelBuilder.Entity<Cancel>(entity =>
        {
            entity.HasKey(c => c.CancelId);
            entity.Property(c => c.AdditionalReason).HasMaxLength(500);
            entity.Property(c => c.ConvenienceFee).HasColumnType("decimal(18,2)");
            entity.Property(c => c.CancelledDate).HasColumnType("timestamp without time zone");
            entity.HasOne(c => c.CancelReason).WithMany(cr => cr.Cancels).HasForeignKey(c => c.CancelReasonId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(c => c.OrderItems).WithMany(o => o.Cancels).HasForeignKey(c => c.OrderItemId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(c => c.CancelStatus).WithMany(cs => cs.Cancels).HasForeignKey(c => c.CancelStatusId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(c => c.CancelRefund).WithOne(cr => cr.Cancel).HasForeignKey<CancelRefund>(cr => cr.CancelId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<CancelStatus>(entity =>
        {
            entity.HasKey(cs => cs.CancelStatusId);
            entity.Property(cs => cs.CancelStatusName).IsRequired().HasMaxLength(50);
            entity.HasIndex(cs => cs.CancelStatusName).IsUnique();
            entity.HasMany(cs => cs.Cancels).WithOne(c => c.CancelStatus).HasForeignKey(c => c.CancelStatusId).OnDelete(DeleteBehavior.Restrict);
            entity.HasData(
            new CancelStatus { CancelStatusId = 1, CancelStatusName = "Pending" },
            new CancelStatus { CancelStatusId = 2, CancelStatusName = "Approved" },
            new CancelStatus { CancelStatusId = 3, CancelStatusName = "Rejected" },
            new CancelStatus { CancelStatusId = 4, CancelStatusName = "Refunded" }
            );
        });
        modelBuilder.Entity<CancelReason>(entity =>
        {
            entity.HasKey(cr => cr.CancelReasonId);
            entity.Property(cr => cr.CancelReasonDescription).IsRequired().HasMaxLength(200);
            entity.HasIndex(cr => cr.CancelReasonDescription).IsUnique();
            entity.HasMany(cr => cr.Cancels).WithOne(c => c.CancelReason).HasForeignKey(c => c.CancelReasonId).OnDelete(DeleteBehavior.Restrict);
            entity.HasData(
            new CancelReason { CancelReasonId = 1, CancelReasonDescription = "Ordered by mistake" },
            new CancelReason { CancelReasonId = 2, CancelReasonDescription = "Found a better price elsewhere" },
            new CancelReason { CancelReasonId = 3, CancelReasonDescription = "Delivery time is too long" },
            new CancelReason { CancelReasonId = 4, CancelReasonDescription = "Changed my mind" },
            new CancelReason { CancelReasonId = 5, CancelReasonDescription = "Incorrect shipping address" },
            new CancelReason { CancelReasonId = 6, CancelReasonDescription = "Duplicate order placed" },
            new CancelReason { CancelReasonId = 7, CancelReasonDescription = "Payment issue" },
            new CancelReason { CancelReasonId = 8, CancelReasonDescription = "Product no longer needed" },
            new CancelReason { CancelReasonId = 9, CancelReasonDescription = "Want to change product variant" },
            new CancelReason { CancelReasonId = 10, CancelReasonDescription = "Other" }
            );
        });
        modelBuilder.Entity<CancelRefund>(entity =>
        {
            entity.HasKey(cr => cr.CancelRefundId);
            entity.HasOne(cr => cr.Cancel).WithOne(c => c.CancelRefund).HasForeignKey<CancelRefund>(cr => cr.CancelId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(cr => cr.Refund).WithOne(c => c.CancelRefund).HasForeignKey<CancelRefund>(cr => cr.RefundId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(cr => cr.CancelId).IsUnique(); entity.HasIndex(cr => cr.RefundId).IsUnique();
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
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.NotificationId).HasName("PK_Notifications");
            entity.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).HasConstraintName("FK_Notification").OnDelete(DeleteBehavior.Restrict);
            entity.Property(n => n.Title).HasMaxLength(100).IsRequired();
            entity.Property(n => n.Message).HasMaxLength(500).IsRequired();
            entity.HasOne(n => n.NotificationType).WithMany(n => n.Notifications).HasForeignKey(n => n.NotificationTypeId).HasConstraintName("FK_Notification_Type").OnDelete(DeleteBehavior.Restrict);
            entity.Property(n => n.IsRead).HasDefaultValue(false);
            entity.Property(n => n.CreatedAt).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(n => n.ReadAt).HasColumnType("timestamp without time zone");
        });
        modelBuilder.Entity<NotificationType>(nt =>
        {
            nt.HasKey(nt => nt.NotificationTypeId).HasName("PK_Notification_Type");
            nt.HasIndex(nt => nt.TypeName).IsUnique();
            nt.HasData(
                new NotificationType { NotificationTypeId = 1, TypeName = "OrderPlaced" },
                new NotificationType { NotificationTypeId = 2, TypeName = "OrderCancelled" },
                new NotificationType { NotificationTypeId = 3, TypeName = "OrderPacked" },
                new NotificationType { NotificationTypeId = 4, TypeName = "OrderShipped" },
                new NotificationType { NotificationTypeId = 5, TypeName = "OutForDelivery" },
                new NotificationType { NotificationTypeId = 6, TypeName = "OrderDelivered" },

                new NotificationType { NotificationTypeId = 7, TypeName = "ReturnRequested" },
                new NotificationType { NotificationTypeId = 8, TypeName = "ReturnApproved" },
                new NotificationType { NotificationTypeId = 9, TypeName = "ReturnRejected" },

                new NotificationType { NotificationTypeId = 10, TypeName = "RefundRequested" },
                new NotificationType { NotificationTypeId = 11, TypeName = "RefundApproved" },
                new NotificationType { NotificationTypeId = 12, TypeName = "RefundRejected" },
                new NotificationType { NotificationTypeId = 13, TypeName = "RefundCompleted" },

                new NotificationType { NotificationTypeId = 14, TypeName = "VendorRegistered" },
                new NotificationType { NotificationTypeId = 15, TypeName = "VendorApproved" },
                new NotificationType { NotificationTypeId = 16, TypeName = "VendorRejected" },

                new NotificationType { NotificationTypeId = 17, TypeName = "ProductSubmitted" },
                new NotificationType { NotificationTypeId = 18, TypeName = "ProductApproved" },
                new NotificationType { NotificationTypeId = 19, TypeName = "ProductRejected" },

                new NotificationType { NotificationTypeId = 20, TypeName = "ReviewReceived" },
                new NotificationType { NotificationTypeId = 21, TypeName = "LowStockAlert" },
                new NotificationType { NotificationTypeId = 22, TypeName = "CouponAvailable" });
        });
    }
}