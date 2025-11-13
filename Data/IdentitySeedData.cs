using Microsoft.AspNetCore.Identity;
using IosAssignment2Backend.Models;

namespace IosAssignment2Backend.Data
{
    public class IdentitySeedData
    {
        public static async Task Initialize(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            // Ensure DB exists
            context.Database.EnsureCreated();

            string password = "P@$$w0rd";

            // --- Seed User 1 ---
            if (await userManager.FindByEmailAsync("aa@aa.aa") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "aa@aa.aa",
                    Email = "aa@aa.aa",
                    FirstName = "Alice",
                    LastName = "Anderson",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                await userManager.CreateAsync(user, password);
            }

            // --- Seed User 2 ---
            if (await userManager.FindByEmailAsync("uu@uu.uu") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "uu@uu.uu",
                    Email = "uu@uu.uu",
                    FirstName = "User",
                    LastName = "Userman",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                await userManager.CreateAsync(user, password);
            }
        }
    }
}
