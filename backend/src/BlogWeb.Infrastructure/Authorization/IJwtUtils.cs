using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlogWeb.Infrastructure.Authorization
{
    public interface IJwtUtils
    {
        public JwtSecurityToken GenerateJwtToken(List<Claim> authClaims);
        public string GenerateJwtToken(string userId);
        public int? ValidateJwtToken(string token);
    }
}