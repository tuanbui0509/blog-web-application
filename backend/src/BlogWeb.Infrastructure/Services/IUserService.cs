using BlogWeb.Application.Entities.Authentication;
using BlogWeb.Application.Models;

namespace BlogWeb.Infrastructure.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
    }
}