using KW_App_Angular.Dall.Context;
using KW_App_Angular.Dall.Entities;
using KW_App_Angular.Models;
using KW_App_Angular.Models.Helper;
using KW_App_Angular.Models.Response;
using KW_App_Angular.Services.Activity;
using KW_App_Angular.Services.Cookie;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using KW_App_Angular.Models.Request;

namespace KW_App_Angular.Services.Account
{
    public class AccountService: IAccountService
    {
        private readonly UserManager<ApplicationUserEntities> _userManager;
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _db;
        private readonly ICookieService _cookieService;
        private readonly IServiceProvider _provider;
        private readonly DataProtectionKeys _dataProtectionKeys;
        private readonly IActivityService _activityService;
        //private IDataProtector _protector;
        //private string[] UserRoles = new[] { "Administrator", "Customer" };
        //private TokenValidationParameters validationParameters;
        //private JwtSecurityTokenHandler handler;
        //private string unProtectedToken;
        //private ClaimsPrincipal validateToken;

        public AccountService(UserManager<ApplicationUserEntities> userManager,
            IOptions<AppSettings> appSettings, IOptions<DataProtectionKeys> dataProtectionKeys,
            ApplicationDbContext db,
            ICookieService cookieService, IServiceProvider provider, IActivityService activityService)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _dataProtectionKeys = dataProtectionKeys.Value;
            _db = db;
            _cookieService = cookieService;
            _provider = provider;
            _activityService = activityService; 
        }
        public async Task<TokenResponseModel> Auth(LoginViewModel model)
        {
            ActivityEntities activityModel = new ActivityEntities();
            activityModel.Date = DateTime.UtcNow;
            activityModel.IpAddress = _cookieService.GetUserIP();
            activityModel.Location = _cookieService.GetUserCountry();
            activityModel.OperatingSystem = _cookieService.GetUserOS();

            try
            {
                // Get the User from Database
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null) return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);

                // Get the role of the user - validate if he is admin - dont bother to go ahead if returned false
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.FirstOrDefault() != "Administrator")
                {   activityModel.UserId = user.Id;
                    activityModel.Type = "UnAuthorized  ";
                    activityModel.Icon = "fas fa-user-secret";
                    activityModel.Color = "danger";                   
                    await _activityService.AddUserActivity(activityModel); 
                    Log.Error("Error: Role not admin");
                    return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);
                }

                // If user is admin continue to execute the code
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    activityModel.UserId = user.Id;
                    activityModel.Type = "Password Login Error";
                    activityModel.Icon = "far fa-times-circle";
                    activityModel.Color = "warning";
                    await _activityService.AddUserActivity(activityModel);
                    Log.Error("Error : Invalid Password for Admin");
                    return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);
                }

                // Then Check If Email Is confirmed
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    activityModel.UserId = user.Id;
                    activityModel.Type = "Email not Verified";
                    activityModel.Icon = "far fa-envelope";
                    activityModel.Color = "warning";
                    await _activityService.AddUserActivity(activityModel);
                    Log.Error("Error : Email Not Confirmed for {user}", user.UserName);
                    return CreateErrorResponseToken("Email Not Confirmed", HttpStatusCode.Unauthorized);
                }

                activityModel.UserId = user.Id;
                activityModel.Type = "Login successful";
                activityModel.Icon = "fas fa-thumbs-up";
                activityModel.Color = "success";
                await _activityService.AddUserActivity(activityModel);
                var authToken = await GenerateNewToken(user, model);
                return authToken;
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                   ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);

        }


        //
        private static TokenResponseModel CreateErrorResponseToken(string errorMessage, HttpStatusCode statusCode)
        {
            var errorToken = new TokenResponseModel
            {
                Token = null,
                Username = null,
                Role = null,
                RefreshTokenExpiration = DateTime.Now,
                RefreshToken = null,
                Expiration = DateTime.Now,
                ResponseInfo = CreateResponse(errorMessage, statusCode)
            };

            return errorToken;
        }
        private static ResponseStatusInfoModel CreateResponse(string errorMessage, HttpStatusCode statusCode)
        {
            var responseStatusInfo = new ResponseStatusInfoModel
            {
                Message = errorMessage,
                StatusCode = statusCode
            };

            return responseStatusInfo;
        }

        private async Task<TokenResponseModel> GenerateNewToken(ApplicationUserEntities user, LoginViewModel model)
        {
            // Create a key to encrypt the JWT 
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

            // Get user role => check if user is admin
            var roles = await _userManager.GetRolesAsync(user);

            // Creating JWT token
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName), // Sub - Identifies principal that issued the JWT
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Jti - Unique identifier of the token
                        new Claim(ClaimTypes.NameIdentifier, user.Id), // Unique Identifier of the user
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()), // Role of the user
                        new Claim("LoggedOn", DateTime.Now.ToString(CultureInfo.InvariantCulture)), // Time When Created
                 }),

                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _appSettings.Site, // Issuer - Identifies principal that issued the JWT.
                Audience = _appSettings.Audience, // Audience - Identifies the recipients that the JWT is intended for.
                Expires = (string.Equals(roles.FirstOrDefault(), "Administrator", StringComparison.CurrentCultureIgnoreCase)) ? DateTime.UtcNow.AddMinutes(60) : DateTime.UtcNow.AddMinutes(Convert.ToDouble(_appSettings.ExpireTime))
            };

            /* Create the unique encryption key for token - 2nd layer protection */
            var encryptionKeyRt = Guid.NewGuid().ToString();
            var encryptionKeyJwt = Guid.NewGuid().ToString();

            /* Get the Data protection service instance */
            var protectorProvider = _provider.GetService<IDataProtectionProvider>();

            /* Create a protector instance */
            var protectorJwt = protectorProvider.CreateProtector(encryptionKeyJwt);

            /* Generate Token and Protect the user token */
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encryptedToken = protectorJwt.Protect(tokenHandler.WriteToken(token));

            /* Create and update the token table */
            TokenEntities newRtoken = new TokenEntities();

            /* Create refresh token instance */
            newRtoken = CreateRefreshToken(_appSettings.ClientId, user.Id, Convert.ToInt32(_appSettings.RtExpireTime));

            /* assign the tne JWT encryption key */
            newRtoken.EncryptionKeyJwt = encryptionKeyJwt;

            newRtoken.EncryptionKeyRt = encryptionKeyRt;

            /* Add Refresh Token with Encryption Key for JWT to DB */
            try
            {
                // First we need to check if the user has already logged in and has tokens in DB
                var rt = _db.Tokens
                    .FirstOrDefault(t => t.UserId == user.Id);

                if (rt != null)
                {
                    // invalidate the old refresh token (by deleting it)
                    _db.Tokens.Remove(rt);

                    // add the new refresh token
                    _db.Tokens.Add(newRtoken);

                }
                else
                {
                    await _db.Tokens.AddAsync(newRtoken);
                }

                // persist changes in the DB
                await _db.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                    ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            // Return Response containing encrypted token
            var protectorRt = protectorProvider.CreateProtector(encryptionKeyRt);
            var layerOneProtector = protectorProvider.CreateProtector(_dataProtectionKeys.ApplicationUserKey);

            var encAuthToken = new TokenResponseModel
            {
                Token = layerOneProtector.Protect(encryptedToken),
                Expiration = token.ValidTo,
                RefreshToken = protectorRt.Protect(newRtoken.Value),
                Role = roles.FirstOrDefault(),
                Username = user.UserName,
                UserId = layerOneProtector.Protect(user.Id),
                ResponseInfo = CreateResponse("Auth Token Created", HttpStatusCode.OK)
            };

            return encAuthToken;
        }

        private static TokenEntities CreateRefreshToken(string clientId, string userId, int expireTime)
        {

            return new TokenEntities()
            {
                ClientId = clientId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(expireTime),
                EncryptionKeyRt = "",
                EncryptionKeyJwt = ""
            };
        }

        
    }
}
