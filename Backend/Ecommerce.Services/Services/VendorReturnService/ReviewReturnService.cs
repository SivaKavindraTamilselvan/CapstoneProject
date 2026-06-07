using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class VendorReturnService : IVendorReturnService
{
    private readonly IReturnRepsository _returnRepsository;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IMapper _mapper;
    public VendorReturnService(IReturnRepsository returnRepsository, IMapper mapper, IVendorUserValidation vendorUserValidation)
    {
        _returnRepsository = returnRepsository;
        _vendorUserValidation = vendorUserValidation;
        _mapper = mapper;
    }
    public async Task<ResponseReviewReturnDTO> ReviewReturnOrder(RequestReviewReturnDTO requestReviewReturnDTO, int vendorUserId)
    {
        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var returnItem = await _returnRepsository.Get(requestReviewReturnDTO.ReturnId);
        if (returnItem == null)
        {
            throw new DataNotFoundException("Return Not Found");
        }
        if (requestReviewReturnDTO.Review)
        {
            returnItem.ReturnStatusId = 2;
        }
        else
        {
            returnItem.ReturnStatusId = 3;
        }
        returnItem.ReviewedByAdminId = vendorUser.VendorUserId;
        await _returnRepsository.Update(returnItem.ReturnId, returnItem);
        return _mapper.Map<ResponseReviewReturnDTO>(returnItem);
    }
}