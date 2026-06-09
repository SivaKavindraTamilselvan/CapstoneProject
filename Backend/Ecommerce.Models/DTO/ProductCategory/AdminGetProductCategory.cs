public class ResponseAdminGetAllCategory
{
    public int ProductCategoryId { get; set; }
    public string? ProductCategoryName { get; set; }
    public bool IsActive {get;set;} = true;
    public int AddedByAdminId {get;set;}
    public string? AddedUserName {get;set;}
    public DateTime CreatedAt {get;set;}
}
public class ResponseAdminGetAllSubCategory
{
    public int ProductSubCategoryId {get;set;}
    public string ProductSubCategoryName {get;set;} = string.Empty;
    public int ProductCategoryId {get;set;}
    public bool CategoryIsActive {get;set;} = true;
    public bool IsActive {get;set;} = true;
    public decimal CommissionPercentage { get; set; }
    public int AddedByAdminId {get;set;}
    public string? AddedUserName {get;set;}
    public DateTime CreatedAt {get;set;}
}
public class ResponseAdminGetAttribute
{
    public int AttributeMasterId { get; set; }
    public string? AttributeName { get; set; }
    public bool IsActive {get;set;} = true;
    public int AddedByAdminId {get;set;}
    public string? AddedUserName {get;set;}
    public DateTime CreatedAt {get;set;}
}
public class ResponseAdminGetCategoryAttribute
{
    public int ProductSubCategoryAttributeId { get; set; }
    public int ProductSubCategoryId { get; set; }
    public string? ProductSubCategoryName {get;set;}
    public bool IsSubCategoryActive { get; set; }
    public int AttributeMasterId { get; set; }
    public string? AttributeName { get; set; }
    public bool IsAttributeActive { get; set; }
    public bool IsActive {get;set;} = true;
    public int AddedByAdminId {get;set;}
    public DateTime CreatedAt {get;set;}

}