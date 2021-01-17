﻿using KW_App_Angular.Models.Helper;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace KW_App_Angular.Services.Cookie
{
    public class CookieService: ICookieService
    {
        private readonly CookieOptions _cookieOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(CookieOptions cookieOptions, IHttpContextAccessor httpContextAccessor)
        {
            _cookieOptions = cookieOptions;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Get(string key)
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[key];
        }

        public void SetCookie(string key, string value, int? expireTime, bool isSecure, bool isHttpOnly)
        {
            _cookieOptions.Expires = expireTime.HasValue ? DateTime.Now.AddMinutes(expireTime.Value) : DateTime.Now.AddMilliseconds(10);
            _cookieOptions.Secure = isSecure;
            _cookieOptions.HttpOnly = isHttpOnly;
            //_cookieOptions.IsEssential = true;
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, _cookieOptions);
        }

        public void SetCookie(string key, string value, int? expireTime)
        {
            if (expireTime.HasValue)
            {
                _cookieOptions.Secure = true;
                _cookieOptions.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            }
            else
            {
                _cookieOptions.Secure = true;
                _cookieOptions.Expires = DateTime.Now.AddMilliseconds(10);
            }
            _cookieOptions.HttpOnly = true;
            _cookieOptions.SameSite = SameSiteMode.Strict;
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, _cookieOptions);
        }

        public void DeleteCookie(string key)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
        }

        public void DeleteAllCookies(IEnumerable<string> cookiesToDelete)
        {
            foreach (var key in cookiesToDelete)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
            }
        }
        //TODO ip,os,country
        public string GetUserIP()
        {
            return string.Empty;
          
        }

        public string GetUserCountry()
        {
            return string.Empty;
          
        }

        public string GetUserOS()
        {
            return string.Empty;
           
        }
    }
}
