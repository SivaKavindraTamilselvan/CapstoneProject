using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class AdminService : IAdminService
{
    public async Task<ResponseReviewOfProductDTO> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO,int adminUserId)
    {
        var product = (await _productRepsository.GetAll()).FirstOrDefault(p=>p.ProductId == requestReviewOfProductDTO.ProductId);
        if(product == null)
        {
            throw new DataNotFoundException("Product Is Missing");
        }
        var adminUser = (await _adminUserRepsository.GetAll()).FirstOrDefault(u=>u.UserId == adminUserId);
        if(adminUser == null)
        {
            throw new DataNotFoundException("Admin Not Found");
        }
        product.ApprovalStatusId = requestReviewOfProductDTO.ApprovalStatusId;
        product.ReviewedByAdminId = adminUser.AdminUserId;
        product.ApprovedAt = DateTime.Now;
        await _productRepsository.Update(product.ProductId,product);
        return _mapper.Map<ResponseReviewOfProductDTO>(product);
    }
}
