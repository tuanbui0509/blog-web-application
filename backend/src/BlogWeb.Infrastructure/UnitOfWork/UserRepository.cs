using BlogWeb.Application.Interfaces.Repositories;
using BlogWeb.Application.Interfaces.Repository;
using BlogWeb.Domain.Entities.Authentication;
using BlogWeb.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace BlogWeb.Infrastructure.UnitOfWork
{
    public class UserRepository : Repository<UserApplication>, IUserRepository
    {
        private ApplicationDbContext dbContext;

        public UserRepository(ApplicationDbContext dbContext, ILogger logger) : base(logger, dbContext)
        {
        }
    }
}