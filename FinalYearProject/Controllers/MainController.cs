using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinalYearProject.Models;
using System;
using System.Data;
using System.Security.Claims;

namespace FinalYearProject.Controllers
{
   public class MainController : Controller
   {
      [Authorize]
      public IActionResult Logoff(string returnUrl = null)
      {
         HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
         return RedirectToAction("Index", "Travel");
      }

      [AllowAnonymous]
      public IActionResult Login(string returnUrl = null)
      {
         TempData["ReturnUrl"] = returnUrl;
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

          
            return false;
      }

   }
}