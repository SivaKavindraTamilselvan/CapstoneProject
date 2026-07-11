using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;
public interface IProfileService
{
       public  Task<ResponseGetProfileDTO> GetProfile(int userId);
}