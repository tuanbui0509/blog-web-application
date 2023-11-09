using BlogWeb.Application.Authentication.Commands.Authentication;
using BlogWeb.Application.Common.Authorization;
using BlogWeb.Application.Interfaces;
using BlogWeb.Application.Interfaces.Repositories;
using BlogWeb.Domain.Helpers;
using BlogWeb.Domain.Models.Authentication;
using MediatR;

namespace BlogWeb.Application.Authentication.Handlers
{
    public class AuthenticationHandler : IRequestHandler<AuthenticationCommand, AuthenticateResponse>
    {
        private readonly IUserService _userService;
        private readonly IJwtUtils _jwtUtils;

        public AuthenticationHandler(IJwtUtils jwtUtils, IUserService userService)
        {
            _jwtUtils = jwtUtils;
            _userService = userService;
        }
        public async Task<AuthenticateResponse> Handle(AuthenticationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userService.CheckUserPassword(request.Email, request.Password);
            return (AuthenticateResponse)await _userService.Authenticate(user);
        }
    }
}