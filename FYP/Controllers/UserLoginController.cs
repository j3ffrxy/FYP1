using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FYP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FYP.Controllers
{
    public class UserLoginController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Login(UserLogin user)
        {
            if (!AuthenticateUser(user.User_id, user.Password, out ClaimsPrincipal principal))
            {
                ViewData["Message"] = "Incorrect User ID or Password";
                ViewData["MsgType"] = "warning";
                return View();
            }
            else
            {

                HttpContext.SignInAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   principal,
                   new AuthenticationProperties
                   {
                       IsPersistent = user.RememberMe 
               });

          

                if (TempData["returnUrl"] != null)
                {
                    string returnUrl = TempData["returnUrl"].ToString();
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                }

                return RedirectToAction("Login");
            }


          
        }

        private bool AuthenticateUser(string uid, string pw,
                                   out ClaimsPrincipal principal)
        {
            principal = null;


            string sql = @"Select * from User where User_id=={0} and userPW = HASHBYTES{'SHA1','{1}')";
            string select = String.Format(sql, uid, pw);
            DataTable ds = DBUtl.GetTable(select);
            if (ds.Rows.Count == 1)
            {
                principal =
                   new ClaimsPrincipal(
                      new ClaimsIdentity(
                         new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, uid),
                         },
                         CookieAuthenticationDefaults.AuthenticationScheme));
                return true;
            }
            return false;
        }


    }


}