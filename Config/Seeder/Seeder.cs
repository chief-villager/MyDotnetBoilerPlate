using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Villager.Api.Data;
using Villager.Api.Domain;

namespace Villager.Api.Config.Seeder
{
    public static class Seeder
    {
        public static void Seed(WebApplication webApplication)
        {
            var scope = webApplication.Services.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();

            // Seed data if necessary
            if (!db.Users.Any())
            {
                var newUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "shola@dev.com",
                    DateOfBirth = "1997-29-10",


                };
                newUser.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(newUser, "Admin1234");
                db.Users.Add(newUser);
                db.SaveChanges();
            }



        }
    }
}

