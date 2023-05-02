using System.Text;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using BlogWeb.Application.Entities.Authentication;
using BlogWeb.Application.Models.Emails;
using BlogWeb.Common.Helpers;
using BlogWeb.Common.Interfaces;
using BlogWeb.Infrastructure.Authorization;
using BlogWeb.Infrastructure.Persistence;
using BlogWeb.Infrastructure.Services;
using BlogWeb.Infrastructure.Services.Emails;
using BlogWeb.Infrastructure.Services.Users;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var services = builder.Services;
var env = builder.Environment;

//Add dbContext, here you can we are using In-memory database.For Entity Framework
services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("WebBlogDb"), b => b.MigrationsAssembly("BlogWeb"));
});
// DI Identity
services.AddIdentity<UserApplication, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Add Config for Required Email
builder.Services.Configure<IdentityOptions>(
    opts => opts.SignIn.RequireConfirmedEmail = true
    );

// services.AddCors(options =>
// {
//     options.AddDefaultPolicy(
//         policy =>
//         {
//             policy.WithOrigins("http://example.com",
//                                 "http://www.contoso.com");
//         });
// });

// Controller
services.AddControllers();

services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(option =>
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

// services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// configure strongly typed settings object
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
//Add Email Configs
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
services.AddSingleton(emailConfig);

// configure DI for application services
services.AddTransient<IUserService, UserService>();
services.AddScoped<IJwtUtils, JwtUtils>();
services.AddScoped<IEmailService, EmailService>();
services.AddSingleton<ICurrentUserService, CurrentUserService>();
//User Manager Service
// Adding Authentication
var secretKeyBytes = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]);
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
// app.UseMiddleware<JwtMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebApiApp v1"));
}
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// create hardcoded test users in db on startup
await using var provider = new ServiceCollection()
    .AddScoped<ApplicationDbContext>()
    .BuildServiceProvider();

try
{
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dataContext.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserApplication>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await ApplicationDbContextSeed.SeedDefaultUserAsync(userManager, roleManager);
}
catch (Exception ex)
{
    throw new Exception(ex.Message);
}

app.Run("http://localhost:4000");