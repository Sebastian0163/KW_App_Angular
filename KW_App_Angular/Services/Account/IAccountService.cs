using KW_App_Angular.Models;
using KW_App_Angular.Models.Request;
using KW_App_Angular.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Services.Account
{
     public interface IAccountService
    {
        Task<TokenResponseModel> Auth(LoginViewModel model);
       // Task<TokenResponseModel> Auth(TokenRequestModel model);
    }
}
