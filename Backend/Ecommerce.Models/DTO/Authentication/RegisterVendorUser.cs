using System.ComponentModel.DataAnnotations;
using Ecommerce.Models;

namespace Ecommerce.DTOs;

public class RequestRegisterVendorUserDTO
{
    public RequestRegisterUserDTO requestRegisterUserDTO {get;set;} = new();

    [Required(ErrorMessage = "Vendor Role Cannot Be Empty")]
    [Range(1,8,ErrorMessage = "Vendor Role Id Must Be Between 1 and 8")]
    public int VendorRoleId {get;set;}
}

public class ResponseRegisterVendorUserDTO
{
    public int VendorId { get; set; }
    public int UserId { get; set; }
    public int VendorRoleId {get;set;}
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}