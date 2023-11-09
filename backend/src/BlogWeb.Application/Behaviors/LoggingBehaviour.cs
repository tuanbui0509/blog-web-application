using BlogWeb.Application.Interfaces;
using BlogWeb.Application.Interfaces.Repositories;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace BlogWeb.Application.Behaviors
{
    public class LoggingBehavior<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly ILogger _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;

        public LoggingBehavior(ILogger<TRequest> logger, ICurrentUserService currentUserService, IUserService userService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId ?? string.Empty;
            string userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _userService.GetUserNameAsync(userId);
            }

            _logger.LogInformation("Matech.CleanArchitecture Request: {Name} {@UserId} {@UserName} {@Request}",
                requestName, userId, userName, request);
        }
    }
}