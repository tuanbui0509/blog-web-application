using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using BlogWeb.Domain.Entities.Authentication;
using BlogWeb.Domain.Helpers;
using BlogWeb.Infrastructure.Persistence;
using BlogWeb.Infrastructure;
using BlogWeb.Application.Common.Authorization;
using BlogWeb.Api.ConfigurationExtension;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var services = builder.Services;
var env = builder.Environment;

// add all services
services.ConfigureInfraStructure(builder.Configuration);
services.AddApplication(builder.Configuration);

// Controller
services.AddControllers();

// configure strongly typed settings object
services.AddEndpointsApiExplorer();

var app = builder.Build();

// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

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

using var scope = app.Services.CreateScope();
{
    try
    {
        var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dataContext.Database.MigrateAsync();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserApplication>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await ApplicationDbContextSeed.SeedDefaultUserAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while migrating or seeding the database.");

        throw;
    }
}

app.Run("http://localhost:4000");