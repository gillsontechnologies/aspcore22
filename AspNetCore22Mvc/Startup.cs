using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCore22Mvc.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using AspNetCore22Mvc.Services;
using AspNetCore22Mvc.Models;
using System.Collections;
using AspNetCore22Mvc.Extensions;

namespace AspNetCore22Mvc
{
    public class Startup
    {
        public IConfigurationRoot AdminConfiguration { get; set; }
        public IConfiguration Configuration { get; }
        //added today
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            AdminConfiguration = new ConfigurationBuilder()
                            .SetBasePath(env.ContentRootPath)
                            .AddJsonFile(@"App_Data\adminSettings.json")
                            .Build();
            Configuration = configuration;
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

         //added code
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Lockout settings
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //external login
            services.AddAuthentication()
                .AddMicrosoftAccount(msAccountOptions =>
            {
                msAccountOptions.ClientId = AdminConfiguration["MicrosoftClientId"];
                msAccountOptions.ClientSecret = AdminConfiguration["MicrosoftClientSecret"];
            })
            .AddGoogle(googleOptions => {
                googleOptions.ClientId = AdminConfiguration["GoogleClientId"];
                googleOptions.ClientSecret = AdminConfiguration["GoogleClientSecret"];
            });

            // Add application services
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, Data.ApplicationDbContext DBContext, UserManager<ApplicationUser> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseCookiePolicy();
            DbInitializer.Initialize(DBContext, userManager);
            CreateUserRoles(serviceProvider).Wait();
           
        }

        //added code to configureroles
        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            IdentityResult roleResult;
            //Adding Addmin Role 
            ArrayList roleList = new ArrayList { "Super", "Admin", "Sales", "Operations", "FieldForce" };
            for (int i = 0; i < roleList.Count; i++)
            {
                var roleCheck = await RoleManager.RoleExistsAsync(roleList[i].ToString());
                if (!roleCheck)
                {
                    //create the roles and seed them to the database  
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleList[i].ToString()));
                }
            }

            //Assign Admin role to super User here we have given our newly loregistered login id for Admin management
            var settings = GetAdminSettings.GetSettings();
            ApplicationUser user = await UserManager.FindByEmailAsync(settings.AdminEmailId);
            if (user != null)
            {
                var User = new ApplicationUser();
                await UserManager.AddToRoleAsync(user, "Super");

                //Approve super admin
                user.isApproved = true;
                await UserManager.UpdateAsync(user);
            }

        }


    }
}
