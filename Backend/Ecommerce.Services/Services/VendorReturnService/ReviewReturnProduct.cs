using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;

public partial class VendorReturnService : IVendorReturnService
{
    public async Task<ResponseReviewReturnDTO> ReviewReturnOrderProduct(RequestReviewReturnProductDTO requestReviewReturnProductDTO,int vendorUserId)
    {
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var returnItem = await _returnRepsository.Get(requestReviewReturnProductDTO.ReturnId);
        returnItem.DamageCost = requestReviewReturnProductDTO.DamageCost;
        returnItem.VendorReview = requestReviewReturnProductDTO.Remarks;
        returnItem.ReviewedByAdminId = vendorUser.VendorUserId;
        returnItem.ReviewedDate = DateTime.Now;
        await _returnRepsository.Update(returnItem.ReturnId,returnItem);
        return _mapper.Map<ResponseReviewReturnDTO>(returnItem);
    }
}
