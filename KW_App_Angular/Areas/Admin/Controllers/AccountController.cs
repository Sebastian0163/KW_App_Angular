using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.AspNetCore.Http;
using KW_App_Angular.Models;
using KW_App_Angular.Models.Helper;
using KW_App_Angular.Dall.Entities;
using KW_App_Angular.Dall.Context;
using KW_App_Angular.Services.Cookie;
using KW_App_Angular.Services.Account;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KW_App_Angular.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        public IActionResult Login()
        { return View(); }

        private readonly AppSettings _appSettings;
        private DataProtectionKeys _dataProtectionKeys;
        private readonly IServiceProvider _provider;
        private readonly ApplicationDbContext _db;
        private readonly IAccountService _accountService;
        private readonly ICookieService _cookieService;
        private const string AccessToken = "access_token";
        private const string User_Id = "user_id";
        string[] cookiesToDelete = { "twoFactorToken", "memberId", "rememberDevice", "user_id", "access_token" };
        
        public AccountController(IOptions<AppSettings> appSettings,
            IServiceProvider provider,
            ApplicationDbContext db,
            IAccountService accountService,
            ICookieService cookieService, IOptions<DataProtectionKeys> dataProtectionKeys)
        {
            _appSettings = appSettings.Value;
            _provider = provider;
            _db = db;
            _accountService = accountService;
            _cookieService = cookieService;
            _dataProtectionKeys = dataProtectionKeys.Value;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await Task.Delay(0);
            ViewData["ReturnUrl"] = returnUrl;
            try
            {
                // Check if user is already logged in 
                if (!Request.Cookies.ContainsKey(AccessToken) || !Request.Cookies.ContainsKey(User_Id))
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, string returnUrl = null)
        {
            // First get the return url - the url which the user was trying to access initially
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    var jwtToken = await _accountService.Auth(model);
                    const int expireTime = 60; // set the value to 60 - as dont want the admin cookie to stay in browser for longer

                    _cookieService.SetCookie("access_token", jwtToken.Token, expireTime);
                    _cookieService.SetCookie("user_id", jwtToken.UserId, expireTime);
                    _cookieService.SetCookie("username", jwtToken.Username, expireTime);
                    Log.Information($"User {model.Email} logged in.");

                    return Ok("Success");

                }
                catch (Exception ex)
                {
                    Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                       ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }

            }

            ModelState.AddModelError("", "Invalid Username/Password was entered");
            Log.Error("Invalid Username/Password was entered");

            return Unauthorized("Please Check the Login Credentials - Invalid Username/Password was entered");

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = _cookieService.Get("user_id");

                if (userId != null)
                {
                    var protectorProvider = _provider.GetService<IDataProtectionProvider>();
                    var protector = protectorProvider.CreateProtector(_dataProtectionKeys.ApplicationUserKey);
                    var unprotectedToken = protector.Unprotect(userId);

                    var rt = _db.Tokens.FirstOrDefault(t => t.UserId == unprotectedToken);

                    // First remove the Token
                    if (rt != null) _db.Tokens.Remove(rt);
                    await _db.SaveChangesAsync();

                    // Second remove all Cookies              
                    _cookieService.DeleteAllCookies(cookiesToDelete);
                }

            }
            catch (Exception ex)
            {
                _cookieService.DeleteAllCookies(cookiesToDelete);
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            Log.Information("User logged out.");
            return RedirectToLocal(null);
        }



        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            // Preventing open redirect attack
            return Url.IsLocalUrl(returnUrl)
                ? (IActionResult)Redirect(returnUrl)
                : RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}
