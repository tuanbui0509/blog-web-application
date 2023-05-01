using BlogWeb.Application.Dto;
using BlogWeb.Application.Entities.Authentication;
using BlogWeb.Common.Helpers;
using Microsoft.AspNetCore.Identity;

namespace BlogWeb.Application.Models
{
    public class AuthenticateResponse: Response
    {
        public ApplicationUserDto User { get; set; }

        public string Token { get; set; }

        public DateTime Expiration { get; set; }

        public AuthenticateResponse(IdentityUser user, string token,DateTime expiration)
        {
            User = new ApplicationUserDto{
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
            Expiration = expiration;
            Token = token;
            Message="Login Success";
            Status = "Success";
        }
    }
}