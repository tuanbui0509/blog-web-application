using System.Reflection;

using Microsoft.AspNetCore.Identity;
using BlogWeb.Infrastructure.Authorization;
using BlogWeb.Infrastructure.Persistence;
using BlogWeb.Infrastructure.Services.Emails;
using BlogWeb.Infrastructure.Services.Users;

using FluentValidation;

using MediatR;
using BlogWeb.Domain.Entities.Authentication;
using BlogWeb.Application.Behaviors;
using BlogWeb.Application.Interfaces;

namespace BlogWeb.Api.DependencyInjection
{
    public static class ConfigurationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            // DI Identity
            services.AddIdentity<UserApplication, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Add Config for Required Email
            services.Configure<IdentityOptions>(
                opts => opts.SignIn.RequireConfirmedEmail = true
                );

            // Mediator
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            // Auto mapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Http accessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}