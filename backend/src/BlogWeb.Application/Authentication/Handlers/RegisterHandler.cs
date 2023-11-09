using BlogWeb.Application.Authentication.Commands.Register;
using BlogWeb.Application.Common.Authorization;
using BlogWeb.Application.Interfaces.Repositories;
using BlogWeb.Domain.Interfaces.Repositories;
using BlogWeb.Domain.Models;
using BlogWeb.Domain.Models.Authentication;

namespace BlogWeb.Application.Authentication.Handlers
{
    public class RegisterHandler : IRequestHandlerWrapper<RegisterCommand, AuthenticateResponse>
    {
        private readonly IUserService _userService;
        private readonly IJwtUtils _jwtUtils;
        public RegisterHandler(IUserService userService, IJwtUtils jwtUtils)
        {
            _userService = userService;
            _jwtUtils = jwtUtils;
        }

        public async Task<ServiceResult<AuthenticateResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var Response = await _userService.RegisterUser(request);
            return ServiceResult.Success(Response);
        }
    }
}