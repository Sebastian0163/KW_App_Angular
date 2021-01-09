using KW_App_Angular.Dall.Context;
using KW_App_Angular.Dall.Entities;
using KW_App_Angular.Models;
using KW_App_Angular.Services.Function;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace KW_App_Angular
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region DB CONNECTION OPTIONS           
            services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer(Configuration.GetConnectionString("KW_App_DEV"), x => x.MigrationsAssembly("KW_App_Angular")));

            services.AddDbContext<DataProtKeyContext>(opt =>
            opt.UseSqlServer(Configuration.GetConnectionString("DataProtKey"), x => x.MigrationsAssembly("KW_App_Angular")));
            #endregion

            #region DEFAULT IDENTITY OPTIONS 
            var identityDefaultModelConfiguration = Configuration.GetSection("IdentityDefaultModel");
            services.Configure<IdentityDefaultModel>(identityDefaultModelConfiguration);
            var identityDefaultModel = identityDefaultModelConfiguration.Get<IdentityDefaultModel>();

            services.AddIdentity<ApplicationUserEntities, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = identityDefaultModel.PasswordRequireDigit;
                options.Password.RequiredLength = identityDefaultModel.PasswordRequiredLength;
                options.Password.RequireNonAlphanumeric = identityDefaultModel.PasswordRequireNonAlphanumeric;
                options.Password.RequireUppercase = identityDefaultModel.PasswordRequireUppercase;
                options.Password.RequireLowercase = identityDefaultModel.PasswordRequireLowercase;
                options.Password.RequiredUniqueChars = identityDefaultModel.PasswordRequiredUniqueChars;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identityDefaultModel.LockoutDefaultLockoutTimeSpanInMinutes);
                options.Lockout.MaxFailedAccessAttempts = identityDefaultModel.LockoutMaxFailedAccessAttempts;
                options.Lockout.AllowedForNewUsers = identityDefaultModel.LockoutAllowedForNewUsers;

                // User settings
                options.User.RequireUniqueEmail = identityDefaultModel.UserRequireUniqueEmail;

                // email confirmation require
                options.SignIn.RequireConfirmedEmail = identityDefaultModel.SignInRequireConfirmedEmail;

            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            #endregion

            # region SERVICE 
            services.AddTransient<IFunctionalService, FunctionalService>();
            # endregion

            #region Create App Users Models
            services.Configure<AdminUserModel>(Configuration.GetSection("AdminUserOptions"));
            services.Configure<AppUserModel>(Configuration.GetSection("AppUserOptions"));
            #endregion



            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
