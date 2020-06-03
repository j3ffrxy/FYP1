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
    public class ActivityController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IActionResult Index()
        {

            DataTable dt = DBUtl.GetTable("SELECT platoon AS [Platoon], type AS [Activity Type], activity_description AS [Description], activity_date AS [Date], status AS [Status] FROM Activity");
            return View("Index", dt.Rows);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Activity a)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("Create");
            }
            else
            {
                string insert =
                    @"INSERT INTO Activity(Activity_id, platoon, type, activity_description, activity_date, status)
                      Values ('{0}' , '{1}' , '{2}' , '{3}' , '{4}', '{5:dd-MM-yyyy}', '{6}')";

                int res = DBUtl.ExecSQL(insert, a.Activity_id, a.platoon, a.type, a.activity_description, a.activity_date, a.status);

                if (res == 1)
                {
                    TempData["Message"] = "Activity Created";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Activity Creation Failed";
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("Index");
            }
        }
        public IActionResult Edits(int id)
        {
            string select = "SELECT * FROM Activity WHERE Activity_id = '{0}'";
            List<Activity> list = DBUtl.GetList<Activity>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "Activity does not exist.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Edit");
            }
        }

        [HttpPost]
        public IActionResult Edits(Activity a)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("Edit");
            }
            else
            {
                string update =
                   @"UPDATE Users
                    SET platoon ='{1}', type='{2}', 
                        activity_description ='{3}' , activity_date ='{4:dd-MM-yyyy}' , status = '{5}'
                        WHERE Activity_id = '{0}'";
                int res = DBUtl.ExecSQL(update, a.Activity_id, a.platoon, a.type, a.activity_description, a.activity_date, a.status);
                if (res == 1)
                {
                    TempData["Message"] = "Activity Updated";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("Index");
            }
        }

        public IActionResult Delete(int id)
        {
            string select = "SELECT * FROM Activity WHERE Activity_id = {0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Activity does not exist";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Activity WHERE Activity_id = {0}";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Activity Deleted";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("Index");
        }
    }
}