using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public IActionResult login()
        {



            return View();
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