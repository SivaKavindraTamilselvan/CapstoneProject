namespace Ecommerce.Models;
public class AdminRole
{
    public int AdminRoleId {get;set;}
    public string AdminRoleName {get;set;} = string.Empty;
    public ICollection<AdminUser> AdminUsers {get;set;} = new List<AdminUser>();
}