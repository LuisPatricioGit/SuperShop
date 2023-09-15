using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SuperShop.Data.Entities;
using SuperShop.Helpers;

namespace SuperShop.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");

            if (!_context.Countries.Any())
            {
                var cities = new List<City>();
                cities.Add(new City { Name = "Lisboa" });
                cities.Add(new City { Name = "Porto" });
                cities.Add(new City { Name = "Faro" });

                _context.Countries.Add(new Country
                {
                    Cities = cities,
                    Name = "Portugal"
                });

                await _context.SaveChangesAsync();
            }

            var user = await _userHelper.GetUserByEmailAsync("luispatricio.info@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Luis",
                    LastName = "Patricio",
                    Email = "luispatricio.info@gmail.com",
                    UserName = "luispatricio.info@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumber = "999666333",
                    Address = "Rua Ua 2",
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()

                };

                var result = await _userHelper.AddUserAsync(user, "121212");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not Create User in Seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }


            if (!_context.Products.Any())
            {
                AddProduct("IPhone X", user);
                AddProduct("Magic Mouse", user);
                AddProduct("IWatch Series 4", user);
                AddProduct("IPad Mini", user);
                await _context.SaveChangesAsync();
            }
        }

        private void AddProduct(string name, User user)
        {
            _context.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000),
                IsAvailable = true,
                Stock = _random.Next(100),
                User = user
            });
        }
    }
}
