using Microsoft.AspNetCore.Identity;
using SM_API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SM_API.Data
{
    public static class DbInitializer
    {
        //public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager, SupermarketDbContext context)
        //{
        //    // Ensure database is created
        //    context.Database.EnsureCreated();

        //    // Check if there's already a user in the database
        //    if (!userManager.Users.Any())
        //    {
        //        // Create the first user (System Admin)
        //        var admin = new User
        //        {
        //            Username = "admin",
        //            FullName = "System Admin",
        //            PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
        //            CreatedById = Guid.Empty,  // For the first user, CreatedBy will be null
        //        };

        //        var result = await userManager.CreateAsync(admin, "1234"); // Use a secure password

        //        if (result.Succeeded)
        //        {
        //            // Assign admin role to this user
        //            await userManager.AddToRoleAsync(admin, "Admin");
        //        }



        //        // Now create the first user with the system admin's Id as CreatedById
        //        var firstUser = context.Users.FirstOrDefault(u => u.Username == "firstuser");

        //        if (firstUser == null)
        //        {
        //            firstUser = new User
        //            {
        //                Username = "user1",
        //                FullName= "User 1",
        //                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
        //                CreatedById = admin.Id // Set CreatedById to the system admin's Id
        //            };

        //            userManager.CreateAsync(firstUser, "1234").Wait();
        //        }
        //    }
        //}
    }
}
