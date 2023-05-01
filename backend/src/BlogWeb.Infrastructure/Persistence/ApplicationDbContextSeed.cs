using BlogWeb.Application.Entities.Authentication;
using BlogWeb.Common.Constants;
using Microsoft.AspNetCore.Identity;

namespace BlogWeb.Infrastructure.Persistence
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<UserApplication> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            // await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            // await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            // await roleManager.CreateAsync(new IdentityRole(Roles.SuperUser.ToString()));
            // await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            var defaultUser = new UserApplication
            {
                UserName = "tuanbui",
                Email = "tuanbui0509@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                await userManager.CreateAsync(defaultUser, "admin-tb");
                await userManager.AddToRolesAsync(defaultUser, new[] { Roles.Admin, Roles.SuperAdmin });
            }
        }
    }
}