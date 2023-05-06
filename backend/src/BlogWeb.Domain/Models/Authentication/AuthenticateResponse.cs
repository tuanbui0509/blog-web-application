using BlogWeb.Domain.Dto;
using BlogWeb.Domain.Helpers;

namespace BlogWeb.Domain.Models.Authentication
{
    public class AuthenticateResponse: Response
    {
        public ApplicationUserDto User { get; set; }

        public string Token { get; set; }
    }
}