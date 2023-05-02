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
            // await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin));
            // await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            // await roleManager.CreateAsync(new IdentityRole(Roles.SuperUser));
            // await roleManager.CreateAsync(new IdentityRole(Roles.User));

            var defaultUser = new UserApplication
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                
                IdentityResult result = await userManager.CreateAsync(defaultUser, "Admin@123");
                if (!result.Succeeded)
                    foreach (IdentityError error in result.Errors)
                        Console.WriteLine($"Oops! {error.Description} ({error.Code})");
                await userManager.AddToRolesAsync(defaultUser, new[] { Roles.Admin, Roles.SuperAdmin });
            }

            var defaultUser2 = new UserApplication
            {
                UserName = "tuanbui",
                Email = "tuanbui@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.UserName != defaultUser2.UserName))
            {
                
                IdentityResult result = await userManager.CreateAsync(defaultUser2, "Admin@123");
                if (!result.Succeeded)
                    foreach (IdentityError error in result.Errors)
                        Console.WriteLine($"Oops! {error.Description} ({error.Code})");
                await userManager.AddToRolesAsync(defaultUser2, new[] { Roles.User, Roles.SuperUser });
            }
        }
    }
}