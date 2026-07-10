using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorReturnService
{
    public Task<PagedResponse<ReturnSummaryDto>> GetAllReturnsForVendor(RequestVendorReturnFilter request,int vendorUserId);
    public Task<ResponseReviewReturnDTO> ReviewReturnOrderProduct(RequestReviewReturnProductDTO requestReviewReturnProductDTO,int vendorUserId);
    public Task<ResponseReviewReturnDTO> ReviewReturnOrder(RequestReviewReturnDTO requestReviewReturnDTO,int vendorUserId);
}