using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Models.Request
{
    public class TokenRequestModel
    {// password or refresh_token    
        public string GrantType { get; set; }
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
