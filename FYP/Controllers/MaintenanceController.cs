using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FYP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FYP.Controller
{
    public class MaintenanceController : Microsoft.AspNetCore.Mvc.Controller
    {

        private const string REDIRECT_CNTR = "Announcement";
        private const string REDIRECT_ACTN = "Forbidden";
        public IActionResult Index()
        {
            DataTable dt = DBUtl.GetTable("SELECT * FROM Maintenance");
            return View("Index", dt.Rows);

        }
        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Start()
        {
            return View();

        }



        [HttpPost]
        public IActionResult Start(Maintenance start)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Please fill up all the blanks";
                ViewData["MsgTye"] = "warning";
                return View();
            }


            string insert =
               @"INSERT INTO Maintenance(Maintenance_id,Start_date,End_date, description,maint_type, archive)
                                 VALUES('{0}','{1:yyyy-MM-dd}','{2:yyyy-MM-dd}','{3}', '{4}')";

            bool archive = false;
            bool maint_type = false;

            int res = DBUtl.ExecSQL(insert, start.Start_date, start.End_date, start.description, maint_type, archive);
            if (res == 1)
            {
                TempData["Message"] = "Successfully started Maintenance";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Message"] = DBUtl.DB_Message;
                TempData["MsgType"] = "danger";
            }

            return RedirectToAction("Start");
        }
        [Authorize(Roles = "Admin")]

        public IActionResult ToggleMaint(string id)
        {
            string select = @"SELECT * FROM Users WHERE role !='Admin'";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count < 0)
            {
                TempData["Message"] = "Toggle failed";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string set = "UPDATE Users SET Maintenance_status = 'True' WHERE role != 'Admin'";
                int res = DBUtl.ExecSQL(set, id);
                if (res > 0)
                {
                    TempData["Message"] = "Maintenance Toggle successful";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Toggle maintenance unsuccessful";
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("Index");




        }
        [Authorize(Roles = "Admin")]

        public IActionResult StopToggleMaint(string id)
        {
            string select = @"SELECT * FROM Users WHERE role !='Admin'";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count < 0)
            {
                TempData["Message"] = "Stop Toggle failed";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string set = "UPDATE Users SET Maintenance_status = 'False' WHERE role != 'Admin'";
                int res = DBUtl.ExecSQL(set, id);
                if (res > 0)
                {
                    TempData["Message"] = "Maintenance stop Toggle successful";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Toggle stop maintenance unsuccessful";
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("Index");




        }
    }
}