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

            DataTable dt = DBUtl.GetTable(@"SELECT Activity_id, P.name AS [Platoon], C.name AS [Company], B.name AS [Brigade], type AS [Activity Type], activity_description AS [Description], activity_date AS [Date]
                                            FROM Activity A
                                            INNER JOIN Brigade B ON B.Brigade_id = A.Brigade_id 
                                            INNER JOIN Company C ON C.Company_id = A.Company_id
                                            INNER JOIN Platoon P ON P.Platoon_id = A.Platoon_id ");
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
                    @"INSERT INTO Activity(Platoon_id, Company_id, Brigade_id, type, activity_description, activity_date)
                      Values ('{0}' , '{1}' , '{2}', '{3}', '{4}', '{5:yyyy-MM-dd}')";

                int res = DBUtl.ExecSQL(insert, a.Platoon_id, a.Company_id, a.Brigade_id, a.type, a.activity_description, a.activity_date);

                if (res == 1)
                {
                    TempData["Message"] = "Activity Created";
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
                return RedirectToAction("Edits");
            }
        }

        [HttpPost]
        public IActionResult Edits(Activity a)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("Edits");
            }
            else
            {
                string update =
                   @"UPDATE Activity
                    SET Platoon_id ='{1}', Company_id ='{2}', Brigade_id ='{3}', type='{4}', 
                        activity_description ='{5}' , activity_date ='{6:yyyy-MM-dd}'
                        WHERE Activity_id = '{0}'";
                int res = DBUtl.ExecSQL(update, a.Activity_id, a.Platoon_id, a.Company_id, a.Brigade_id, a.type, a.activity_description, a.activity_date);
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