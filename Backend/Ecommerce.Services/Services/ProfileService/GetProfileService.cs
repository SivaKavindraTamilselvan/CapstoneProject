using AutoMapper;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class ProfileService : IProfileService
{
    private readonly IUserRepsository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(IMapper mapper, IUserRepsository userRepsository, ILogger<ProfileService> logger)
    {
        _userRepository = userRepsository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResponseGetProfileDTO> GetProfile(int userId)
    {
        _logger.LogInformation("Getting profile for UserId: {UserId}", userId);

        var user = await _userRepository.Get(userId);

        if (user == null)
        {
            _logger.LogWarning("User profile not found. UserId: {UserId}", userId);
            throw new DataNotFoundException("User not found");
        }

        _logger.LogInformation("Successfully retrieved profile for UserId: {UserId}", userId);

        return _mapper.Map<ResponseGetProfileDTO>(user);
    }
}