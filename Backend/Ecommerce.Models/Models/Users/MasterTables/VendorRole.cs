namespace Ecommerce.Models;
public class VendorRole
{
    public int VendorRoleId {get;set;}
    public string VendorRoleName {get;set;} = string.Empty;
    public ICollection<VendorUser> VendorUsers {get;set;} = new List<VendorUser>();
}