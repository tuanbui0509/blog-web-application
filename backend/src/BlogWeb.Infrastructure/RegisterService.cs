using BlogWeb.Application.Interfaces.Repository;
using BlogWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogWeb.Infrastructure
{
    public static class RegisterService
    {
        public static void ConfigureInfraStructure(this IServiceCollection services,
    IConfiguration configuration)
        {
            //Add dbContext, here you can we are using In-memory database.For Entity Framework
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("WebBlogDb"), b => b.MigrationsAssembly("BlogWeb.Api"));
            });
        }
    }
}