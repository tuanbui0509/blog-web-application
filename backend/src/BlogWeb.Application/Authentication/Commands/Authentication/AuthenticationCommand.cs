using BlogWeb.Domain.Models.Authentication;
using MediatR;

namespace BlogWeb.Application.Authentication.Commands.Authentication
{
    public class AuthenticationCommand:IRequest<AuthenticateResponse>
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}