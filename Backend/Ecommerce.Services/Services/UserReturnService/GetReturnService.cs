using System.Data.Common;
using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class UserReturnService : IUserReturnService
{
    public async Task<PagedResponse<ReturnSummaryDto>> GetAllReturnsForUser(RequestUserReturnFilter request, int UserId)
    {
        var result = await _returnRepsository.GetAllReturnsForUser(request, UserId);
        return new PagedResponse<ReturnSummaryDto>
        {
            Items = _mapper.Map<List<ReturnSummaryDto>>(result.data),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = result.totalCount
        };
    }
}