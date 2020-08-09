using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FYP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FYP.Controller
{
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private const string LOGIN_SQL =
         @"SELECT * FROM Users 
            WHERE nric = '{0}' 
              AND password =  HASHBYTES('SHA1', '{1}')";

        private const string ROLE_COL = "role";
        private const string NAME_COL = "nric";

        private const string REDIRECT_CNTR = "User";
        private const string REDIRECT_ACTN = "Index";

        //private const string forbiddenpage = "Forbidden";
        //private const string controller = "Maintenance";

        private const string LOGIN_VIEW = "UserLogin";


        public IActionResult Login(string returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View(LOGIN_VIEW);
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLogin user)
        {
            if (!AuthenticateUser(user.nric, user.password, out ClaimsPrincipal principal))
            {
                ViewData["Message"] = "Incorrect NRIC or Password";
                ViewData["MsgType"] = "warning";
                return View(LOGIN_VIEW);
            }
            else

                {

                var listt = DBUtl.GetList<Users>("SELECT * FROM Users WHERE nric = '{0}'", user.nric);

                    HttpContext.SignInAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   principal,
                   new AuthenticationProperties
                   {
                       IsPersistent = user.RememberMe
                   });
                    if (listt[0].Maintenance_status == "True")

                    {
                        return RedirectToAction("Forbidden", "Maintenance");

                    }
                    else
                    {

                        if (TempData["returnUrl"] != null)
                        {

                            string returnUrl = TempData["returnUrl"].ToString();
                            if (Url.IsLocalUrl(returnUrl))
                                return Redirect(returnUrl);
                        }

                        return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
                    }
                }
            
        }

        public IActionResult Logoff(string returnUrl = null)
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Face");
        }

        private bool AuthenticateUser(string uid, string pw, out ClaimsPrincipal principal)
        {
            principal = null;

            DataTable ds = DBUtl.GetTable(LOGIN_SQL, uid, pw);
            if (ds.Rows.Count == 1)
            {
                principal =
                   new ClaimsPrincipal(
                      new ClaimsIdentity(
                         new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, uid),
                        new Claim(ClaimTypes.Name, ds.Rows[0][NAME_COL].ToString()),
                        new Claim(ClaimTypes.Role, ds.Rows[0][ROLE_COL].ToString())
                         }, "Basic"
                      )
                   );
                if (principal != null)
                {
                    bool d = true;
                    return d;
                }
                return false;
            }
            return false;
        }
    }
}