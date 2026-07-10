using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorReturnService : IVendorReturnService
{
    public async Task<ResponseReviewReturnDTO> ReviewReturnOrderProduct(RequestReviewReturnProductDTO requestReviewReturnProductDTO, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} reviewing ReturnId {ReturnId}", vendorUserId, requestReviewReturnProductDTO.ReturnId);
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var returnItem = await _returnRepsository.Get(requestReviewReturnProductDTO.ReturnId);
        if (returnItem == null)
        {
            _logger.LogWarning("ReturnId {ReturnId} not found for Vendor UserId {VendorUserId}", requestReviewReturnProductDTO.ReturnId, vendorUserId);
            throw new DataNotFoundException("Return Not Found");
        }
        returnItem.DamageCost = requestReviewReturnProductDTO.DamageCost;
        returnItem.VendorReview = requestReviewReturnProductDTO.Remarks;
        returnItem.ReviewedByVendorId = vendorUser.VendorUserId;
        returnItem.ReviewedDate = DateTime.Now;
        returnItem.ReturnStatusId = 7;
        await _returnRepsository.Update(returnItem.ReturnId, returnItem);
        _logger.LogInformation("ReturnId {ReturnId} reviewed successfully by Vendor UserId {VendorUserId}. DamageCost: {DamageCost}", returnItem.ReturnId, vendorUserId, returnItem.DamageCost);
        return _mapper.Map<ResponseReviewReturnDTO>(returnItem);
    }
    public async Task<PagedResponse<ReturnSummaryDto>> GetAllReturnsForVendor(RequestVendorReturnFilter request, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} requested returns with filter {@Filter}",vendorUserId,request);
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var result = await _returnRepsository.GetAllReturnsForVendor(request, vendor.VendorId);
        _logger.LogInformation("Returning {ReturnCount} returns for VendorId {VendorId}",result.data.Count,vendor.VendorId);
        return new PagedResponse<ReturnSummaryDto>
        {
            Items = _mapper.Map<List<ReturnSummaryDto>>(result.data),
            TotalCount = result.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

    }
}
