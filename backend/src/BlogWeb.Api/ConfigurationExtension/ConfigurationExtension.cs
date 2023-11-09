using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using BlogWeb.Application.Common.Authorization;
using BlogWeb.Domain.Emails;
using BlogWeb.Domain.Entities.Authentication;
using BlogWeb.Infrastructure.Persistence;
using BlogWeb.Infrastructure.Services.Emails;
using BlogWeb.Infrastructure.Services.Users;

using FluentValidation;

//using MediatR;
using BlogWeb.Application.Authentication.Commands.Authentication;
using BlogWeb.Application.Authentication.Handlers;
using BlogWeb.Application.Behaviors;
using BlogWeb.Application.Interfaces.Repositories;
using BlogWeb.Domain.Models.Authentication;
using BlogWeb.Infrastructure.UnitOfWork;

using MediatR;

namespace BlogWeb.Api.ConfigurationExtension
{
    public static class ConfigurationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {

            // configure DI for application services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            // Http accessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // DI Identity
            services.AddIdentity<UserApplication, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Add Config for Required Email
            services.Configure<IdentityOptions>(
                opts => opts.SignIn.RequireConfirmedEmail = true
                );
            //Add Email Configs
            var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);

            // Mediator
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            services.AddScoped<IRequestHandler<AuthenticationCommand, AuthenticateResponse>, AuthenticationHandler>();

            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
                });
            });

            // Adding Authentication
            var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                    ValidateIssuer = true,
                    ValidateAudience = true
                };
            });
            // Auto mapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(typeof(Program));
            return services;
        }
    }
}