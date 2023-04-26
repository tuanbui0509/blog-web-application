using BlogWeb.Application.Entities.Authentication;

namespace BlogWeb.Infrastructure.Authorization
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(User user);
        public int? ValidateJwtToken(string token);
    }
}