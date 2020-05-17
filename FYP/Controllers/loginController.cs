using FYP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Security.Claims;

namespace FYP.Controllers
{
    public class loginController : Controller
    {
       

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            TempData[""] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(UserLogin user)
        {
            if (!AuthenticateUser(user.UserID, user.Password,
                                  out ClaimsPrincipal principal))
            {
                ViewData["Message"] = "Incorrect User ID or Password";
                return View();
            }
            else
            {
                HttpContext.SignInAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   principal);

                if (TempData["returnUrl"] != null)
                {
                    string returnUrl = TempData["returnUrl"].ToString();
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Travel");
            }
        }

        private bool AuthenticateUser(string uid, string pw,
                                      out ClaimsPrincipal principal)
        {
            principal = null;

            string sql = @" SELECT * FROM TravelUser WHERE UserId = '{0}' AND UserPw = HASHBYTES('SHA1', '{1}') ";

            string select = String.Format(sql, uid, pw);
            int ds;
           
            return false;
        }

    }
}