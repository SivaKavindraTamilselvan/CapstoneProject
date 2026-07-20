namespace Ecommerce.DTOs;

public class RequestAdiminUserFilter : PaginationFilter
{
    public int? AdminRoleId {get;set;}
    public bool? Status {get;set;}
    public string? Email {get;set;}
    public string? PhoneNumber{get;set;}
}

public class RequestVendorUserFilter : PaginationFilter
{
    public int? VendorRoleId {get;set;}
    public bool? Status {get;set;}
    public string? Email {get;set;}
    public string? PhoneNumber{get;set;}
}


public class ResponseGetAdminUserDTO
{
    public int AdminUserId {get;set;}
    public int UserId {get;set;}
    public int AdminRoleId {get;set;}
    public string? AdminRoleName {get;set;}
    public string? FirstName { get; set; } 
    public string? LastName { get; set; } 
    public string? Email { get; set; } 
    public string? PhoneNumber { get; set; }
    public bool IsActive {get;set;}
    public DateTime CreatedAt {get;set;} 
    
}

public class ResponseGetVendorUserDTO
{
    public int VendorUserId {get;set;}
    public int UserId {get;set;}
    public int VendorRoleId {get;set;}
    public string? VendorRoleName {get;set;}
    public string? FirstName { get; set; } 
    public string? LastName { get; set; } 
    public string? Email { get; set; } 
    public string? PhoneNumber { get; set; }
    public bool IsActive {get;set;}
    public DateTime CreatedAt {get;set;} 
    
}


public class ResponseGetVendorUserListDTO
{
    public int VendorUserId {get;set;}
    public string? VendorRoleName {get;set;}
    public string? FirstName { get; set; } 
    public string? LastName { get; set; } 
    public string? Email { get; set; } 
    public string? PhoneNumber { get; set; }
    public bool IsActive {get;set;}
    
}