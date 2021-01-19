using KW_App_Angular.Dall.Context;
using KW_App_Angular.Dall.Entities;
using KW_App_Angular.Models.User;
using KW_App_Angular.Services.Activity;
using KW_App_Angular.Services.Cookie;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace KW_App_Angular.Services.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUserEntities> _userManager;
        [Obsolete]
        private readonly IHostingEnvironment _env;
        private readonly ApplicationDbContext _db;
        private readonly ICookieService _cookieService;
        private readonly IActivityService _activityService;
        private readonly IServiceProvider _provider;
        private readonly DataProtectionKeys _dataProtectionKeys;


        [Obsolete]
        public UserService(UserManager<ApplicationUserEntities> userManager,
                    IHostingEnvironment env,
                    ApplicationDbContext db,
                    ICookieService cookieService,
                    IActivityService activityService,
                    IServiceProvider provider,
                    IOptions<DataProtectionKeys> dataProtectionKeys)
        {
            _userManager = userManager;
            _env = env;
            _db = db;
            _cookieService = cookieService;
            _activityService= activityService;
            _provider=provider;
            _dataProtectionKeys= dataProtectionKeys.Value;
        }


        public async Task<ProfileModel> GetUserProfileByEmailAsync(string email)
        {
            var userProfile = new ProfileModel();

            try
            {
                var loggedInUserId = GetLoggedInUserId();
                var user = await _userManager.FindByIdAsync(loggedInUserId);

                if (user == null || user.Email != email) return null;

                userProfile = new ProfileModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Phone = user.PhoneNumber,
                    Birthday = user.Birthday,
                    Gender = user.Gender,
                    Displayname = user.DisplayName,
                    Firstname = user.Firstname,
                    Middlename = user.Middlename,
                    Lastname = user.Lastname,
                    IsEmailVerified = user.EmailConfirmed,
                    IsPhoneVerified = user.PhoneNumberConfirmed,
                    IsTermsAccepted = user.Terms,
                    IsTwoFactorOn = user.TwoFactorEnabled,
                    ProfilePic = user.ProfilePic,
                    UserRole = user.UserRole,
                    IsAccountLocked = user.LockoutEnabled,
                    IsEmployee = user.IsEmployee,
                   
                    Activities = new List<ActivityEntities>(_db.Activities.Where(x => x.UserId == user.Id)).OrderByDescending(o => o.Date).Take(20).ToList()
                };

            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return userProfile;
        }


        public async Task<ProfileModel> GetUserProfileByIdAsync(string userId)
        {
            ProfileModel userProfile = new ProfileModel();

            var loggedInUserId = GetLoggedInUserId();   
            var user = await _userManager.FindByIdAsync(loggedInUserId);

            if (user == null || user.Id != userId) return null;

            try
            {
                userProfile = new ProfileModel()
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Phone = user.PhoneNumber,
                    Birthday = user.Birthday,
                    Gender = user.Gender,
                    Displayname = user.DisplayName,
                    Firstname = user.Firstname,
                    Middlename = user.Middlename,
                    Lastname = user.Lastname,
                    IsEmailVerified = user.EmailConfirmed,
                    IsPhoneVerified = user.PhoneNumberConfirmed,
                    IsTermsAccepted = user.Terms,
                    IsTwoFactorOn = user.TwoFactorEnabled,
                    ProfilePic = user.ProfilePic,
                    UserRole = user.UserRole,
                    IsAccountLocked = user.LockoutEnabled,
                    IsEmployee = user.IsEmployee,                  
                    Activities = new List<ActivityEntities>(_db.Activities.Where(x => x.UserId == user.Id)).OrderByDescending(o => o.Date).Take(20).ToList()
                };
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return userProfile;


        }


        public async Task<ProfileModel> GetUserProfileByUsernameAsync(string username)
        {
            var userProfile = new ProfileModel();

            try
            {
                var loggedInUserId = GetLoggedInUserId();
                var user = await _userManager.FindByIdAsync(loggedInUserId);
                if (user == null || user.UserName != username) return null;

                userProfile = new ProfileModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.UserName,
                    Phone = user.PhoneNumber,
                    Birthday = user.Birthday,
                    Gender = user.Gender,
                    Displayname = user.DisplayName,
                    Firstname = user.Firstname,
                    Middlename = user.Middlename,
                    Lastname = user.Lastname,
                    IsEmailVerified = user.EmailConfirmed,
                    IsPhoneVerified = user.PhoneNumberConfirmed,
                    IsTermsAccepted = user.Terms,
                    IsTwoFactorOn = user.TwoFactorEnabled,
                    ProfilePic = user.ProfilePic,
                    UserRole = user.UserRole,
                    IsAccountLocked = user.LockoutEnabled,
                    IsEmployee = user.IsEmployee,
                   
                    Activities = new List<ActivityEntities>(_db.Activities.Where(x => x.UserId == user.Id)).OrderByDescending(o => o.Date).Take(20).ToList()
                };

            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return userProfile;
        }


        public async Task<bool> CheckPasswordAsync(ProfileModel model, string password)
        {
            try
            {
                var loggedInUserId = GetLoggedInUserId();
                var user = await _userManager.FindByIdAsync(loggedInUserId);

                if (user.UserName != _cookieService.Get("username") ||
                    user.UserName != model.Username)
                    return false;

                if (!await _userManager.CheckPasswordAsync(user, password))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return false;
            }

            return true;
        }

        #region Profile update
        
        
        public async Task<bool> UpdateProfileAsync(IFormCollection formData)
        {
            var loggedInUserId = GetLoggedInUserId();
            var user = await _userManager.FindByIdAsync(loggedInUserId);

            if (user == null) return false;

            if (user.UserName != _cookieService.Get("username") ||
                user.UserName != formData["username"].ToString() ||
                user.Email != formData["email"].ToString())
                return false;

            try
            {
                ActivityEntities activityEntities = new ActivityEntities { UserId = user.Id };
                await UpdateProfilePicAsync(formData, user);

                user.Firstname = formData["firstname"];
                user.Birthday = formData["birthdate"];
                user.Lastname = formData["lastname"];
                user.Middlename = formData["middlename"];
                user.DisplayName = formData["displayname"];
                user.PhoneNumber = formData["phone"];
                user.Gender = formData["gender"];
                user.TwoFactorEnabled = Convert.ToBoolean(formData["IsTwoFactorOn"]);

               
                await _userManager.UpdateAsync(user);

                activityEntities.Date = DateTime.UtcNow;
                activityEntities.IpAddress = _cookieService.GetUserIP();
                activityEntities.Location = _cookieService.GetUserCountry();
                activityEntities.OperatingSystem = _cookieService.GetUserOS();
                activityEntities.Type = "Profile update successful";
                activityEntities.Icon = "fas fa-thumbs-up";
                activityEntities.Color = "success";
                await _activityService.AddUserActivity(activityEntities);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while updating profile {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return false;
        }
       
        
        private async Task<ApplicationUserEntities> UpdateProfilePicAsync(IFormCollection formData, ApplicationUserEntities user)
        {
            // First we create an empty array to store old file info
            var oldProfilePic = new string[1];
            // we will store path of old file to delete in an empty array.
            oldProfilePic[0] = Path.Combine(_env.WebRootPath + user.ProfilePic);

            // Create the Profile Image Path
            var profPicPath = _env.WebRootPath + $"{Path.DirectorySeparatorChar}uploads{Path.DirectorySeparatorChar}user{Path.DirectorySeparatorChar}profile{Path.DirectorySeparatorChar}";

            // If we have received any files for update, then we update the file path after saving to server
            // else we return the user without any changes
            if (formData.Files.Count <= 0) return user;

            var extension = Path.GetExtension(formData.Files[0].FileName);
            var filename = DateTime.Now.ToString("yymmssfff");
            var path = Path.Combine(profPicPath, filename) + extension;
            var dbImagePath = Path.Combine($"{Path.DirectorySeparatorChar}uploads{Path.DirectorySeparatorChar}user{Path.DirectorySeparatorChar}profile{Path.DirectorySeparatorChar}", filename) + extension;

            user.ProfilePic = dbImagePath;

            // Copying New Files to the Server - profile Folder
            await using (var stream = new FileStream(path, FileMode.Create))
            {
                await formData.Files[0].CopyToAsync(stream);
            }

            // Delete old file after successful update
            if (!System.IO.File.Exists(oldProfilePic[0])) return user;

            System.IO.File.SetAttributes(oldProfilePic[0], FileAttributes.Normal);
            System.IO.File.Delete(oldProfilePic[0]);

            return user;
        }


        #endregion
        public async Task<bool> ChangePasswordAsync(ProfileModel model, string newPassword)
        {
            bool result;
            try
            {
                ActivityEntities activityEntities = new ActivityEntities();
                activityEntities.Date = DateTime.UtcNow;
                activityEntities.IpAddress = _cookieService.GetUserIP();
                activityEntities.Location = _cookieService.GetUserCountry();
                activityEntities.OperatingSystem = _cookieService.GetUserOS();

                var loggedInUserId = GetLoggedInUserId();
                var user = await _userManager.FindByIdAsync(loggedInUserId);

                if (user != null)
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
                    var updateResult = await _userManager.UpdateAsync(user);
                    result = updateResult.Succeeded;
                    activityEntities.UserId = user.Id;
                    activityEntities.Type = "Password Changed successful";
                    activityEntities.Icon = "fas fa-thumbs-up";
                    activityEntities.Color = "success";
                    await _activityService.AddUserActivity(activityEntities);
                }
                else
                {
                    result = false;
                }

            }
            catch (Exception ex)
            {
                result = false;
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return result;
        }


        public async Task<bool> AddUserActivity(ActivityEntities model)
        {
            try
            {
                await _activityService.AddUserActivity(model);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return false;
        }


        public async Task<List<ActivityEntities>> GetUserActivity(string username)
        {
            List<ActivityEntities> userActivities = new List<ActivityEntities>();

            try
            {
                var loggedInUserId = GetLoggedInUserId();
                var user = await _userManager.FindByIdAsync(loggedInUserId);

                if (user == null || user.UserName != username) return null;

                userActivities = await _db.Activities.Where(x => x.UserId == user.Id).OrderByDescending(o => o.Date).Take(20).ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return userActivities;
        }



        private string GetLoggedInUserId()
        {
            try
            {
                var protectorProvider = _provider.GetService<IDataProtectionProvider>(); 
                var protector = protectorProvider.CreateProtector(_dataProtectionKeys.ApplicationUserKey);
                var unprotectUserId = protector.Unprotect(_cookieService.Get("user_id"));
                return unprotectUserId;
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while decrypting user Id  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return null;

        }
    }
}
