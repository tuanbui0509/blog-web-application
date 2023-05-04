using BlogWeb.Application.Common.Helpers;
using BlogWeb.Application.Common.Models;
using BlogWeb.Application.Dto;
using BlogWeb.Application.Models;
using BlogWeb.Application.Models.SignUp;

namespace BlogWeb.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<Response> Authenticate(AuthenticateRequest loginModel);
        Task<Response> RegisterUser(RegisterUser registerUser);
        Task<string> GetUserNameAsync(string userId);
        Task<Response> LoginWithOTP(string code, string username);
        Task<ApplicationUserDto> CheckUserPassword(string userName, string password);

        Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

        Task<bool> UserIsInRole(string userId, string role);

        Task<Result> DeleteUserAsync(string userId);
    }
}