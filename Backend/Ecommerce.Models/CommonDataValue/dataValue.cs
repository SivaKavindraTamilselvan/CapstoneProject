public enum RoleEnum
{
    Admin = 1,
    User = 2,
    Vendor = 3,
    VendorOwner = 1
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