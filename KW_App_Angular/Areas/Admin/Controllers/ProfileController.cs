using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using KW_App_Angular.Services.Cookie;
using KW_App_Angular.Dall.Entities;
using KW_App_Angular.Models.Helper;
using KW_App_Angular.Services.User;
using KW_App_Angular.Models;
using KW_App_Angular.Models.Admin;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KW_App_Angular.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "Admin")]
   // [AutoValidateAntiforgeryToken]
    public class ProfileController : Controller
    {
        private readonly ICookieService _cookieService;
        private readonly IServiceProvider _provider;
        private readonly DataProtectionKeys _dataProtectionKeys;
        //private readonly AppSettings _appSettings;
        private readonly IUserService _userService;
        private static AdminBaseViewModel _adminBaseViewModel;


        public ProfileController(
            ICookieService cookieService,
            IUserService userService,
            IServiceProvider provider,
            IOptions<DataProtectionKeys> dataProtectionKeys)
            
        {
            _userService = userService;
            _cookieService = cookieService;           
            _provider = provider;
            _dataProtectionKeys = dataProtectionKeys.Value;
         
           
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await SetAdminBaseViewModel();
            return View("Index", _adminBaseViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Security()
        {
            await SetAdminBaseViewModel();
            return View("Security", _adminBaseViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Activity()
        {
            await SetAdminBaseViewModel();
            return View("Activity", _adminBaseViewModel);
        }


        private async Task SetAdminBaseViewModel()
        {
            var protectorProvider = _provider.GetService<IDataProtectionProvider>();
            var protector = protectorProvider.CreateProtector(_dataProtectionKeys.ApplicationUserKey);
            var userProfile = await _userService.GetUserProfileByIdAsync(protector.Unprotect(_cookieService.Get("user_id")));
            var resetPassword = new ResetPasswordViewModel();

            _adminBaseViewModel = new AdminBaseViewModel
            {
                Profile = userProfile,
                ResetPassword = resetPassword,
               
            };

        }

    }
}
