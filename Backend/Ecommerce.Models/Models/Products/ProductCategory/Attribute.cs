namespace Ecommerce.Models;
public class AttributeMaster
{
    public int AttributeMasterId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public bool IsActive {get;set;} = true;
    public int AddedByAdminId {get;set;}
    public AdminUser? AddedByAdminUser {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public ICollection<ProductSubCategoryAttribute> ProductSubCategoryAttributes { get; set; } = new List<ProductSubCategoryAttribute>();
}