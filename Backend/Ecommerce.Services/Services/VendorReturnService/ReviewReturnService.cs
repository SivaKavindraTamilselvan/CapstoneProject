using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class VendorReturnService : IVendorReturnService
{
    private readonly ILogger<VendorReturnService> _logger;
    private readonly IReturnRepsository _returnRepsository;
    private readonly IVendorUserValidation _vendorUserValidation;
    private readonly IMapper _mapper;
    public VendorReturnService(ILogger<VendorReturnService> logger, IReturnRepsository returnRepsository, IMapper mapper, IVendorUserValidation vendorUserValidation)
    {
        _returnRepsository = returnRepsository;
        _vendorUserValidation = vendorUserValidation;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<ResponseReviewReturnDTO> ReviewReturnOrder(RequestReviewReturnDTO requestReviewReturnDTO, int vendorUserId)
    {
        _logger.LogInformation("Vendor UserId {VendorUserId} reviewing ReturnId {ReturnId}. Review Decision: {Review}", vendorUserId, requestReviewReturnDTO.ReturnId, requestReviewReturnDTO.Review);

        var vendorUser = await _vendorUserValidation.ValidateVendorUserByUserId(vendorUserId);
        var returnItem = await _returnRepsository.Get(requestReviewReturnDTO.ReturnId);
        if (returnItem == null)
        {
            _logger.LogWarning("ReturnId {ReturnId} not found for Vendor UserId {VendorUserId}", requestReviewReturnDTO.ReturnId, vendorUserId);
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
        var previousStatus = returnItem.ReturnStatusId;
        returnItem.ReviewedByVendorId = vendorUser.VendorUserId;
        await _returnRepsository.Update(returnItem.ReturnId, returnItem);
        _logger.LogInformation("ReturnId {ReturnId} reviewed successfully by Vendor UserId {VendorUserId}. Status changed from {PreviousStatus} to {NewStatus}", returnItem.ReturnId, vendorUserId, previousStatus, returnItem.ReturnStatusId);
        return _mapper.Map<ResponseReviewReturnDTO>(returnItem);
    }
}