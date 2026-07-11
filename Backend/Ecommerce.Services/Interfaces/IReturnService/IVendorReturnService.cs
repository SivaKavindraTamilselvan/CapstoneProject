using Ecommerce.DTOs;
using Ecommerce.DTOs.Returns;

namespace Ecommerce.Services.Interfaces;

public interface IVendorReturnService
{
        public  Task<ResponseReviewReturnDTO> AcceptReturnProduct(int returnId, int vendorUserId);
    public Task<ReturnDetailsDto?> GetReturnDetails(int returnId);
    public Task<PagedResponse<ReturnSummaryDto>> GetAllReturnsForVendor(RequestVendorReturnFilter request, int vendorUserId);
    public Task<ResponseReviewReturnDTO> ReviewReturnOrderProduct(RequestReviewReturnProductDTO requestReviewReturnProductDTO, int vendorUserId);
    public Task<ResponseReviewReturnDTO> ReviewReturnOrder(RequestReviewReturnDTO requestReviewReturnDTO, int vendorUserId);
}