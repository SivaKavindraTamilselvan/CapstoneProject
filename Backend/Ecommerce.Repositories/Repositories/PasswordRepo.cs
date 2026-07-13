using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class PasswordSetTokenRepsository : IPasswordSetTokenRepsository
    {
        private readonly EcommerceContext _ecommerceContext;

        public PasswordSetTokenRepsository(EcommerceContext ecommerceContext)
        {
            _ecommerceContext = ecommerceContext;
        }

        public async Task<PasswordSetToken> Create(PasswordSetToken token)
        {
            _ecommerceContext.PasswordSetTokens.Add(token);
            await _ecommerceContext.SaveChangesAsync();
            return token;
        }

        public async Task<PasswordSetToken?> GetByToken(string token)
        {
            return await _ecommerceContext.PasswordSetTokens
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Token == token);
        }

        public async Task<PasswordSetToken?> MarkAsUsed(int passwordSetTokenId)
        {
            var token = await _ecommerceContext.PasswordSetTokens
                .FirstOrDefaultAsync(p => p.PasswordSetTokenId == passwordSetTokenId);
            if (token == null) return null;

            token.IsUsed = true;
            await _ecommerceContext.SaveChangesAsync();
            return token;
        }
    }
}