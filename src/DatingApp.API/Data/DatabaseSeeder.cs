namespace DatingApp.API.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DatingApp.API.Models;
    using Microsoft.AspNetCore.Identity;
    using Newtonsoft.Json;

    public class DatabaseSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public DatabaseSeeder(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async void SeedUsers()
        {
            if (!_userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/userSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                // Create roles
                var roles = new List<Role>
                {
                    new Role { Name = "Member" },
                    new Role { Name = "Admin" },
                    new Role { Name = "Moderator" },
                    new Role { Name = "VIP" }
                };
                
                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(role);
                }

                foreach (var user in users)
                {
                    await _userManager.CreateAsync(user, "password");
                    await _userManager.AddToRoleAsync(user, "Member");
                }

                // Create Admin user
                var adminUser = new User
                {
                    UserName = "Admin",
                };

                var result = await _userManager.CreateAsync(adminUser, "password");
                if (result.Succeeded) 
                {
                    var admin = await _userManager.FindByNameAsync("Admin");
                    await _userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
                }
            }

        }
    }
}