using BlogWeb.Domain.Interfaces;
using BlogWeb.Domain.Interfaces.Repositories;
using BlogWeb.Domain.Models.Authentication;

namespace BlogWeb.Application.Authentication.Commands.GetToken
{
    public class GetTokenQuery : IRequestWrapper<AuthenticateResponse>
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}