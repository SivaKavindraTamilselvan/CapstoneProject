using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Services
{
    public class TokenService : ITokenService
    {
        readonly string _key;
        readonly string _issuer;
        readonly string _duration;
        public TokenService(IConfiguration configuration)
        {
            _key = configuration["JWT:Key"] ?? "This is the alternate key";
            _issuer = configuration["JWT:Issuer"] ?? "Any Server";
            _duration = configuration["JWT:DurationInMinutes"] ?? "60";
        }
        public string CreateNewToken(TokenRequest request)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, request.UserId.ToString()),
                new Claim(ClaimTypes.Name,$"{request.FirstName} {request.LastName}"),
                new Claim(ClaimTypes.Role,request.RoleId.ToString()),
                new Claim(ClaimTypes.Email,request.Email)
            };

            if (request.RoleId == 1 && request.AdminRoleId != null)
            {
                claims.Add(new Claim("AdminRoleId",request.AdminRoleId.Value.ToString()));
            }
            if (request.RoleId == 2 && request.VendorRoleId != null)
            {
                claims.Add(new Claim("VendorRoleId",request.VendorRoleId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_duration)),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
