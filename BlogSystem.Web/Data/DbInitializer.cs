using Microsoft.AspNetCore.Identity;
using BlogSystem.Web.Models;

namespace BlogSystem.Web.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Create roles if they don't exist
            string[] roleNames = { "Admin", "Editor", "Reader" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user if it doesn't exist
            var adminEmail = "admin@blog.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed categories if they don't exist
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Technology", Description = "Posts about technology and programming" },
                    new Category { Name = "Lifestyle", Description = "Posts about lifestyle and personal development" },
                    new Category { Name = "News", Description = "Current events and news" }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed tags if they don't exist
            if (!context.Tags.Any())
            {
                var tags = new[]
                {
                    new Tag { Name = "C#" },
                    new Tag { Name = "ASP.NET Core" },
                    new Tag { Name = "Web Development" },
                    new Tag { Name = "Programming" },
                    new Tag { Name = "Technology" }
                };

                context.Tags.AddRange(tags);
                await context.SaveChangesAsync();
            }
        }
    }
} 