using BlogWeb.Domain.Dto;
using BlogWeb.Domain.Helpers;
using BlogWeb.Domain.Models;
using BlogWeb.Domain.Models.Authentication;
using BlogWeb.Domain.SignUp;

namespace BlogWeb.Application.Interfaces
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