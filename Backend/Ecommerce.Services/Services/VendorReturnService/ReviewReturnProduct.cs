using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class VendorReturnService : IVendorReturnService
{
    public async Task<ResponseReviewReturnDTO> ReviewReturnOrderProduct(RequestReviewReturnProductDTO requestReviewReturnProductDTO,int vendorUserId)
    {
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var returnItem = await _returnRepsository.Get(requestReviewReturnProductDTO.ReturnId);
        if(returnItem == null)
        {
            throw new DataNotFoundException("Return Not Found");
        }
        returnItem.DamageCost = requestReviewReturnProductDTO.DamageCost;
        returnItem.VendorReview = requestReviewReturnProductDTO.Remarks;
        returnItem.ReviewedByVendorId = vendorUser.VendorUserId;
        returnItem.ReviewedDate = DateTime.Now;
        returnItem.ReturnStatusId = 7;
        await _returnRepsository.Update(returnItem.ReturnId,returnItem);
        return _mapper.Map<ResponseReviewReturnDTO>(returnItem);
    }
    public async Task<PagedResponse<ReturnSummaryDto>> GetAllReturnsForVendor(RequestVendorReturnFilter request,int vendorUserId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var result = await _returnRepsository.GetAllReturnsForVendor(request,vendor.VendorId);
        return new PagedResponse<ReturnSummaryDto>
        {
            Items = _mapper.Map<List<ReturnSummaryDto>>(result.data),
            TotalCount = result.totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

    }
}
