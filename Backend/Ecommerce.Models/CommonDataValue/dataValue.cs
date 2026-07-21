public enum RoleEnum
{
    Admin = 1,
    User = 2,
    Vendor = 3,
    VendorOwner = 1
}

public enum AdminRoleEnum
{
    Overall_Admin = 1,
    Vendor_Admin = 2,
    Product_Admin = 3,
    Order_Admin = 4,
    Coupons_Logistic_Admin = 5,
    Return_Admin = 6,
    Refund_Admin = 7,
    Exchange_Admin = 8,
    Payment_Admin = 9
}

public enum VendorRoleEnum
{
    Owner = 1,
    Manager = 2,
    ProductManager = 3,
    OrderManager = 4,
    ReturnManager = 5,
    RefundManager = 6,
    InventoryManager = 7,
    CouponManager = 8
}

public enum ApprovalStatusEnum
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3,
    Deleted_By_Admin = 4
}

public enum ProductApprovalStatusEnum
{
    Pending = 1,
    Vendor_Approved = 2,
    Vendor_Rejected = 3,
    Admin_Approved = 4,
    Admin_Rejected = 5,
    Deleted_By_Admin = 6
}

public enum ProductStatusEnum
{
    Draft = 1,
    Active = 2,
    Temporarily_Not_Available = 3,
    Archived = 4
}

public enum TableName
{
    Address = 1,
    User = 2,
    CartItems = 3,
}
public enum AuditAction
{
    Created = 1,
    Updated = 2,
    Deleted = 3
}

public enum NotificationTypeEnum
{
    // Orders
    OrderPlaced = 1,
    OrderCancelled = 2,
    OrderPacked = 3,
    OrderReadyForPickup = 4,
    OrderShipped = 5,
    OrderDelivered = 6,
    OrderCompleted = 7,

    // Returns & Refunds
    ReturnApproved = 8,
    ReturnRejected = 9,
    ReturnDisputed = 10,
    ReturnRefunded = 11,

    // Products
    ProductAdded = 12,
    ProductSubmitted = 13,
    ProductReviewed = 14,
    ProductImageAdded = 15,
    ProductDeleted = 16,

    // Vendor
    VendorDeleted = 17,

    // Inventory
    InventoryAdded = 18,
    InventoryDeleted = 19,
    LowStockAlert = 20,

    // Warehouse
    WarehouseAdded = 21,
    WarehouseDeleted = 22,

    // Master Data
    Attribute = 23,
    MappedAttribute = 24,
    Category = 25,
    SubCategory = 26
}
public enum ShipmentStatusEnum
{
    Pending = 1,
    Payment_Success = 2,
    Payment_Failed = 3,
    Ready_For_Pick_Up = 4,
    Picked_Up = 5,
    In_Transit = 6,
    Out_Of_Delivery = 7,
    Delivered = 8,
    Failed = 9,
    Returned = 10
}

public enum OrderItemStatusEnum
{
    Pending = 1,
    Packed = 2,
    Processed = 3,
    Delivered = 4,
    Delayed = 5,
    Returned = 6,
    Cancelled = 7,
    Refunded = 8,
    Return_Requested = 9,
    Return_Accepted = 10,
    Return_Rejected = 11
}

public enum ShipmentTypeEnum
{
    Order = 1,
    Return = 2,
    Exchange = 3
}

public enum OrderStatusEnum
{
    Pending = 1,
    Confirmed = 2,
    Completed = 3,
    Cancelled = 4,
    Failed = 5
}

public enum ReturnStatusEnum
{
    Requested = 1,
    Approved = 2,
    Rejected = 3,
    PickupScheduled = 4,
    PickedUp = 5,
    Received = 6,
    InspectionCompleted = 7,
    RefundProcessed = 8,
    Completed = 9,
    Cancelled = 10,
    DisputeReturn = 11
}

public enum CancelStatusEnum
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Refunded = 4
}

public enum RefundStatusEnum
{
    VendorRequested = 1,
    Pending = 2,
    AdminReviewed = 3,
    Processed = 4,
    Failed = 5,
    Cancelled = 6,
    Completed = 7
}

public enum CouponTypeEnum
{
    Admin = 1,
    Vendor = 2
}