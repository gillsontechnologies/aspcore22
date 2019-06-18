using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AspNetCore22Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore22Mvc.Extensions
{
    public static class DbInitializer
    {


        public static void Initialize(Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)//SchoolContext is EF context
        {
            //context.Database.EnsureCreated();//if db is not exist ,it will create database .but ,do nothing . 
            context.Database.Migrate();

            if (!context.Users.Any())
            {
                var settings = GetAdminSettings.GetSettings();
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = settings.AdminEmailId,
                    Name = "Tripti",
                    Surname = "Pandey",
                    Organisation = "Gillson Technologies",
                    Position = "Super Admin",
                    PhoneNumber = "9999999999"
                };
                userManager.CreateAsync(user, "Super@123").Wait();
            }
        }
    }
}
