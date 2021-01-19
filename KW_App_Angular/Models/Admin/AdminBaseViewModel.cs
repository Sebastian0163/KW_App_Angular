using KW_App_Angular.Models.Helper;
using KW_App_Angular.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Models.Admin
{
    public class AdminBaseViewModel
    {
        public ProfileModel Profile { get; set; }
        public AddUserModel AddUser { get; set; }
        public AppSettings AppSetting { get; set; }
        public ResetPasswordViewModel ResetPassword { get; set; }

    }
}
