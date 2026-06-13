using AutoMapper;
using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class AdminService : IAdminService
{
    public async Task<ResponseRegisterAdminDTO> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO, int adminUserId)
    {
        _logger.LogInformation("Admin registration initiated by AdminUserId {AdminUserId} for Email {Email}", adminUserId, requestRegisterAdminDTO.requestRegisterUserDTO.Email);
        var result = await _authentication.RegisterAdmin(requestRegisterAdminDTO, adminUserId);
        _logger.LogInformation("Admin registration completed successfully. New UserId {UserId}", result.UserId);
        return result;
    }
}