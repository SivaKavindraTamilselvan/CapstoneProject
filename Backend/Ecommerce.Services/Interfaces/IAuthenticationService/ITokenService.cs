using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces
{
    public interface ITokenService
    {
        public string CreateNewToken(TokenRequest request);
    }
}
