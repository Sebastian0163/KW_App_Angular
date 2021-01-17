using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Dall.Entities
{
    public class ApplicationUserEntities : IdentityUser
    {
        public string Notes { get; set; }
        public string DisplayName { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }
        public string ProfilePic { get; set; }
        public string Birthday { get; set; }
        public bool IsProfileComplete { get; set; }
        public bool Terms { get; set; }
        public bool IsEmployee { get; set; }
        public string UserRole { get; set; }
        public DateTime AccountCreatedOn { get; set; } = DateTime.Now;
        public bool RememberMe { get; set; }
        public bool IsActive { get; set; }
       

    }
}
