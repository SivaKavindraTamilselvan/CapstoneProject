
public class ResponseDeleteProduct
{
    public int ProductId {get;set;}
    public int VendorId {get;set;}
    public string VendorName {get;set;} = string.Empty;
    public string ProductName {get;set;} = string.Empty;
    public int ProductSubCategoryId {get;set;}
    public string ProductSubCategoryName {get;set;} = string.Empty;
    public string Description {get;set;} = string.Empty;
    public int ProductApprovalStatusId {get;set;} = 1;
    public int AddedByVendorUserId {get;set;}
    public int ProductStatusId {get;set;} = 1;
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime? UpdatedAt{get;set;}

}