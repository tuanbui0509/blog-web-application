using BlogWeb.Application.Authentication.Commands.GetToken;
using BlogWeb.Application.Common.Authorization;
using BlogWeb.Application.Interfaces.Repositories;
using BlogWeb.Domain.Interfaces.Repositories;
using BlogWeb.Domain.Models;
using BlogWeb.Domain.Models.Authentication;

namespace BlogWeb.Application.Authentication.Handlers
{
    public class GetTokenQueryHandler : IRequestHandlerWrapper<GetTokenQuery, AuthenticateResponse>
    {
        private readonly IUserService _userService;
        private readonly IJwtUtils _jwtUtils;

        public GetTokenQueryHandler(IJwtUtils jwtUtils, IUserService userService)
        {
            _jwtUtils = jwtUtils;
            _userService = userService;
        }

        public async Task<ServiceResult<AuthenticateResponse>> Handle(GetTokenQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.CheckUserPassword(request.Email, request.Password);

            if (user == null)
                return ServiceResult.Failed<AuthenticateResponse>(ServiceError.ForbiddenError);

            return ServiceResult.Success(new AuthenticateResponse
            {
                User = user,
                Token = _jwtUtils.GenerateJwtToken(user.Id)
            });
        }

    }
}