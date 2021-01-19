using KW_App_Angular.Dall.Entities;
using KW_App_Angular.Models.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Services.User
{
    public interface IUserService
    {
        Task<ProfileModel> GetUserProfileByIdAsync(string userId);
        Task<ProfileModel> GetUserProfileByUsernameAsync(string username);
        Task<ProfileModel> GetUserProfileByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(ProfileModel model, string password);
        Task<bool> UpdateProfileAsync(IFormCollection formData);
        Task<bool> ChangePasswordAsync(ProfileModel model, string newPassword);
        Task<bool> AddUserActivity(ActivityEntities model);
        Task<List<ActivityEntities>> GetUserActivity(string username);
    }
}
