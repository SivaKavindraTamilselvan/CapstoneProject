using AutoMapper;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class ProfileService : IProfileService
{
    private readonly IUserRepsository _userRepository;
    private readonly IMapper _mapper;
    public ProfileService(IMapper mapper,IUserRepsository userRepsository)
    {
        _userRepository = userRepsository;
        _mapper = mapper;
    }
    public async Task<ResponseGetProfileDTO> GetProfile(int userId)
    {
        var user = await _userRepository.Get(userId);

        if (user == null)
        {
            throw new DataNotFoundException("User not found");
        }

        return _mapper.Map<ResponseGetProfileDTO>(user);
    }
}