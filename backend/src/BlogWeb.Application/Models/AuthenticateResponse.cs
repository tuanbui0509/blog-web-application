using BlogWeb.Application.Common.Helpers;
using BlogWeb.Application.Dto;

namespace BlogWeb.Application.Models
{
    public class AuthenticateResponse: Response
    {
        public ApplicationUserDto User { get; set; }

        public string Token { get; set; }
    }
}