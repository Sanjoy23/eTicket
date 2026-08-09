using Identity.API.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach(var role in new[] { "Admin", "Member" })
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if(await userManager.FindByEmailAsync("sanjoy@demo.com") is null)
            {
                var admin = new AppUser
                {
                    UserName = "sanjoy@demo.com",
                    Email = "sanjoy@demo.com",
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true
                };
                
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
