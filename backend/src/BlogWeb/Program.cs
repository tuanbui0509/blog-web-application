using System.Text.Json.Serialization;
using BlogWeb.Common.Helpers;
using BlogWeb.Infrastructure.Authorization;
using BlogWeb.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BlogWeb.Infrastructure.Services.Emails;
using BlogWeb.Application.Models.Emails;
using Microsoft.AspNetCore.Identity;
using BlogWeb.Common.Interfaces;
using BlogWeb.Infrastructure.Services.Users;
using BlogWeb.Application.Entities.Authentication;
using BlogWeb.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var services = builder.Services;
var env = builder.Environment;

//Add dbContext, here you can we are using In-memory database.For Entity Framework
services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("WebBlogDb"), b => b.MigrationsAssembly("BlogWeb"));
});

//Add Config for Required Email
builder.Services.Configure<IdentityOptions>(
    opts => opts.SignIn.RequireConfirmedEmail = true
    );

// Controller
services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// services.AddIdentity<UserApplication, IdentityRole>(options =>
// {
//     // Password settings
//     options.Password.RequireDigit = true;
//     options.Password.RequiredLength = 8;
//     options.Password.RequireNonAlphanumeric = false;
//     options.Password.RequireUppercase = true;
//     options.Password.RequireLowercase = false;
//     options.Password.RequiredUniqueChars = 1;
// })
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddDefaultTokenProviders();
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

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// configure strongly typed settings object
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
//Add Email Configs
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
services.AddSingleton(emailConfig);

// configure DI for application services
services.AddIdentity<UserApplication, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IJwtUtils, JwtUtils>();
services.AddScoped<IEmailService, EmailService>();
services.AddSingleton<ICurrentUserService, CurrentUserService>();
services.AddIdentity<UserApplication, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
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
    await dataContext.SaveChangesAsync();
}
catch (Exception ex)
{
    // var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // logger.LogError(ex, "An error occurred while migrating or seeding the database.");

    throw ex;
}


// {
//     var testUsers = new List<UserApplication>
//     {
//         new UserApplication { UserName = "admin", PasswordHash = BCryptNet.HashPassword("admin") },
//         new UserApplication { UserName = "user", PasswordHash = BCryptNet.HashPassword("user") }
//     };

//     using var scope = app.Services.CreateScope();
//     var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     dataContext.Users.AddRange(testUsers);
// }
app.Run("http://localhost:4000");
