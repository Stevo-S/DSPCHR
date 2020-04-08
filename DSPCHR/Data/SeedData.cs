using DSPCHR.Authorisation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string defaultAdminPassword)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureAdminUser(serviceProvider, defaultAdminPassword, "admin");
                await EnsureAdminRole(serviceProvider, adminID, RoleNames.AdministratorsRoleName);
                await EnsureRoleExists(serviceProvider, "Support");
            }
        }

        private static async Task<IdentityResult> EnsureRoleExists(IServiceProvider serviceProvider, string roleName)
        {
            IdentityResult IR = null;

            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("rolemanager null");
            }

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            return IR;
        }

        private static async Task<IdentityResult> EnsureAdminRole(IServiceProvider serviceProvider, string userId, string roleName)
        {
            IdentityResult IR = await EnsureRoleExists(serviceProvider, roleName);

            var userManager = serviceProvider.GetService<UserManager<IdentityResult>>();

            var user = await userManager.FindByNameAsync(userId);

            if (user == null)
            {
                throw new Exception("The default password was probably not strong enough");
            }

            IR = await userManager.AddToRoleAsync(user, roleName);

            return IR;
        }

        private static async Task<string> EnsureAdminUser(IServiceProvider serviceProvider, string defaultAdminPassword, string userName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = userName,
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }
    }
}
