using KW_App_Angular.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Services.User
{
    public class UserService : IUserService
    {
        public Task<ProfileModel> GetUserProfileByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<ProfileModel> GetUserProfileByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ProfileModel> GetUserProfileByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }
    }
}
