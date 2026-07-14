using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class VendorApprovalHistories : AbstractRepository<int, VendorApprovalHistory>, IVendorApprovalRepsository
{
    public VendorApprovalHistories(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }
}