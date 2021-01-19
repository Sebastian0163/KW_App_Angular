using KW_App_Angular.Dall.Entities;
using KW_App_Angular.Models;
using KW_App_Angular.Models.User;
using KW_App_Angular.Services.Cookie;
using KW_App_Angular.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace KW_App_Angular.Controllers.API.v1
{   [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = "User")]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICookieService _cookieService;

        public ProfileController(IUserService userService, ICookieService cookieService)
        {
            _userService = userService;
            _cookieService = cookieService;
        }

        [HttpGet("[action]/{username}")]
        public async Task<IActionResult> GetUserProfile([FromRoute] string username)
        {

            if (username == null)
            {
                return BadRequest();
            }
            var result = await _userService.GetUserProfileByUsernameAsync(username);

            if (result == null) return NotFound();

            return Ok(result);

        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateProfile(IFormCollection formData)
        {
            ProfileModel model = new ProfileModel { Username = formData["Username"] };

            var password = formData["Password"].ToString();

            if (await _userService.CheckPasswordAsync(model, password))
            {
                var result = await _userService.UpdateProfileAsync(formData);

                if (result)
                {
                    return Ok(new { Message = "Profile updated Successfully!" });
                }
            }

            return BadRequest(new { Message = "Could not Update Profile." });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePassword([FromBody] ResetPasswordViewModel model)
        {
            if (string.IsNullOrEmpty(model.OldPassword))
            {
                return BadRequest("Old Password must be supplied for password change.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(model);
            }

            var user = await _userService.GetUserProfileByEmailAsync(model.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok(new { message = "Password changed Successfully" });
            }

            if (!await _userService.CheckPasswordAsync(user, model.OldPassword))
            {
                // Notify attempt was made - to change password failed
                ActivityEntities activityEntities = new ActivityEntities
                {
                    UserId = user.UserId,
                    Date = DateTime.UtcNow,
                    IpAddress = _cookieService.GetUserIP(),
                    Location = _cookieService.GetUserCountry(),
                    OperatingSystem = _cookieService.GetUserOS(),
                    Type = "Profile update failed - Invalid Old Password",
                    Icon = "fas fa-exclamation-triangle",
                    Color = "warning"
                };

                var activityAdd = await _userService.AddUserActivity(activityEntities);

                return BadRequest(new { message = "Invalid Old Password" });
            }

            var result = await _userService.ChangePasswordAsync(user, model.Password);

            if (result)
            {
                return Ok(new { message = "Password changed Successfully" });
            }
            return BadRequest(new { message = "Password could not be Changed. Try again later" });
        }

        [HttpGet("[action]/{username}")]
        public async Task<IActionResult> GetUserActivity([FromRoute] string username)
        {
            var result = await _userService.GetUserActivity(username);

            if (result != null)
            {
                return Ok(new { Message = "Fetched user activities successfully!", data = result });
            }

            return BadRequest(new { Message = "Could not fetch user activities." });
        }

    }
}
