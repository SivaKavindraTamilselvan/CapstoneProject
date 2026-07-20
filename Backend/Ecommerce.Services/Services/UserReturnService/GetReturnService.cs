using System.Data.Common;
using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserReturnService : IUserReturnService
{
    public async Task<PagedResponse<ReturnSummaryDto>> GetAllReturnsForUser(RequestUserReturnFilter request, int userId)
    {
        _logger.LogInformation("Getting returns for UserId: {UserId}, PageNumber: {PageNumber}, PageSize: {PageSize}", userId, request.PageNumber, request.PageSize);

        var result = await _returnRepsository.GetAllReturnsForUser(request, userId);

        _logger.LogInformation("Retrieved {ReturnCount} returns out of {TotalCount} total returns for UserId: {UserId}", result.data.Count, result.totalCount, userId);

        return new PagedResponse<ReturnSummaryDto>
        {
            Items = _mapper.Map<List<ReturnSummaryDto>>(result.data),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = result.totalCount
        };
    }
}