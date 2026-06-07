using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorReturnService
{
    public Task<ResponseReviewReturnDTO> ReviewReturnOrder(RequestReviewReturnDTO requestReviewReturnDTO,int vendorUserId);
}