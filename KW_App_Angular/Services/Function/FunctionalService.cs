using KW_App_Angular.Dall.Entities;
using KW_App_Angular.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Services.Function
{
    public class FunctionalService : IFunctionalService
    {
        private readonly AdminUserModel _adminUserModel;
        private readonly AppUserModel _appUserModel;
        private readonly UserManager<ApplicationUserEntities> _userManager;
        

       
        public FunctionalService(IOptions<AppUserModel> appUserModel,
            IOptions<AdminUserModel> adminUserModel,
            UserManager<ApplicationUserEntities> userManager)
        {
            _adminUserModel = adminUserModel.Value;
            _appUserModel = appUserModel.Value;
            _userManager = userManager;
          

        }

        public async Task CreateDefaultAdminUser()
        {
            try
            {
                var adminUser = new ApplicationUserEntities
                {
                    Email = _adminUserModel.Email,
                    UserName = _adminUserModel.Username,
                    EmailConfirmed = true,
                    ProfilePic = GetDefaultProfilePic(),      ///             
                    Firstname = _adminUserModel.Firstname,
                    Lastname = _adminUserModel.Lastname,
                    UserRole = "Administrator",
                    IsActive = true,

                };


                var result = await _userManager.CreateAsync(adminUser, _adminUserModel.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Administrator");
                    // Log.Information("Admin User Created {UserName}", adminUser.UserName);
                }
                else
                {
                    var errorString = string.Join(",", result.Errors);
                    Log.Error("Error while creating user {Error}", errorString);
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error while creating user {Error} {StackTrace} {InnerException} {Source}",
                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
        }

        public async Task CreateDefaultUser()
        {
            try
            {
                var appUser = new ApplicationUserEntities
                {
                    Email = _appUserModel.Email,
                    UserName = _appUserModel.Username,
                    EmailConfirmed = true,
                    ProfilePic = GetDefaultProfilePic(),

                    Firstname = _appUserModel.Firstname,
                    Lastname = _appUserModel.Lastname,
                    UserRole = "Customer",
                    IsActive = true,

                };

                var result = await _userManager.CreateAsync(appUser, _appUserModel.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "Customer");
                    Log.Information("App User Created {UserName}", appUser.UserName);
                }
                else
                {
                    var errorString = string.Join(",", result.Errors);
                    Log.Error("Error while creating user {Error}", errorString);
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error while creating user {Error} {StackTrace} {InnerException} {Source}",
                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
        }

        private string GetDefaultProfilePic()
        {

            return string.Empty;
        }
    }
}
