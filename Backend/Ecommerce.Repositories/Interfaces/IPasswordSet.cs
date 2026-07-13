using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces
{
    public interface IPasswordSetTokenRepsository
    {
        Task<PasswordSetToken> Create(PasswordSetToken token);
        Task<PasswordSetToken?> GetByToken(string token);
        Task<PasswordSetToken?> MarkAsUsed(int passwordSetTokenId);
    }
}