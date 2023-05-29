using Microsoft.AspNetCore.Identity;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            var user = await _userHelper.GetUserByEmailAsync("luispatricio.info@gmail.com");
            if (user == null)
            {
                user = new User 
                {
                    FirstName = "Luis",
                    LastName = "Patricio",
                    Email = "luispatricio.info@gmail.com",
                    UserName = "MrSiul",
                    PhoneNumber = "999666333"
                };

                var result = await _userHelper.AddUserAsync(user, "121212");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not Create User in Seeder");
                }
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
