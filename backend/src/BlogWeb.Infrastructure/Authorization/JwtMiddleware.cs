using BlogWeb.Common.Helpers;
using BlogWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BlogWeb.Infrastructure.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtUtils.ValidateJwtToken(token);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                // context.Items["User"] = userService.GetById(userId.Value);
            }

            await _next(context);
        }
    }
}