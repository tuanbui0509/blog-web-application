using BlogWeb.Application.Models;
using BlogWeb.Common.Helpers;

namespace BlogWeb.Infrastructure.Services
{
    public interface IUserService
    {
        Task<Response> Authenticate(AuthenticateRequest loginModel);
    }
}