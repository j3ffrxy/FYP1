using System;
using System.Collections;
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

namespace FYP.Controllers
{
    public class ExerciseController : Microsoft.AspNetCore.Mvc.Controller
    {
        [Authorize(Roles = "Admin, Platoon Commander, Officer Commander, Commandant Officer")]
        public IActionResult Index()
        {
            updatearchive();
            DataTable dt = DBUtl.GetTable(@"SELECT Exercise_id, E.Package_id, U.nric AS [SAF11B], E.company AS [Company], 
                                            E.unit AS [Unit], P.Name AS [Weapon Package], E.start_date AS [Start Date], E.end_date AS [End Date], 
                                            E.description AS [Description], E.status AS [Status]
                                            FROM Exercise E 
                                            INNER JOIN Users U ON E.nric = U.nric 
                                            INNER JOIN Package P ON E.Package_id = P.Package_id
                                            WHERE E.archive = 0");
            return View("Index", dt.Rows);
        }

        [Authorize(Roles = "Admin, Platoon Commander, Officer Commander, Commandant Officer")]
        public IActionResult ViewArchive()
        {
            updatearchive();
            DataTable dt = DBUtl.GetTable(@"SELECT Exercise_id, E.Package_id, U.nric AS [SAF11B], E.company AS [Company], 
                                            E.unit AS [Unit], P.Name AS [Weapon Package], E.start_date AS [Start Date], E.end_date AS [End Date], 
                                            E.description AS [Description], E.status AS [Status]
                                            FROM Exercise E 
                                            INNER JOIN Users U ON E.nric = U.nric 
                                            INNER JOIN Package P ON E.Package_id = P.Package_id
                                            WHERE E.archive = 1");
            return View("ViewArchive", dt.Rows);
        }

        private void updatearchive()
        {
            var list = DBUtl.GetList<Exercise>("SELECT * FROM Exercise");
            DateTime currentdate = DateTime.Now;

            foreach (var a in list)
            {
                DateTime enddate = a.end_date;
                if (enddate < currentdate)
                {
                    var update = "UPDATE Exercise SET archive = '{0}' WHERE Exercise_id = '{1}'";
                    DBUtl.ExecSQL(update, true, a.Exercise_id);

                }
            }
        }

        [Authorize(Roles = "Admin, Platoon Commander, Officer Commander, Commandant Officer")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Platoon Commander, Officer Commander, Commandant Officer")]
        [HttpPost]
        public IActionResult Create(Exercise e)
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
                    @"INSERT INTO Exercise(nric, Package_id, company, unit, description, start_date, end_date, archive, status, assigned_status)
                      Values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5:yyyy-MM-dd}', '{6:yyyy-MM-dd}', '{7}', '{8}', '{9}')";

                string nricSql = @"SELECT * FROM Users WHERE nric = '" + User.Identity.Name + "'";
                List<Users> assigned = DBUtl.GetList<Users>(nricSql);
                string nricFinal = assigned[0].nric;

                bool archived = false;
                bool assignedStatus = false;
                int res = DBUtl.ExecSQL(insert, nricFinal, e.Package_id, e.company, e.unit, e.description, e.start_date, e.end_date, archived, "Pending", assignedStatus);

                if (res == 1)
                {
                    TempData["Message"] = "Exercise Created";
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

        [Authorize(Roles = "Admin, Platoon Commander, Officer Commander, Commandant Officer")]
        public IActionResult Edits(int id)
        {
            string select = "SELECT * FROM Exercise WHERE Exercise_id = '{0}'";
            List<Exercise> list = DBUtl.GetList<Exercise>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "Exercise does not exist.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Edits");
            }
        }

        [Authorize(Roles = "Admin, Platoon Commander, Officer Commander, Commandant Officer")]
        [HttpPost]
        public IActionResult Edits(Exercise e)
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
                   @"UPDATE Exercise
                    SET Package_id = '{1}', company = '{2}', unit = '{3}', description = '{4}', 
                        start_date = '{5:yyyy-MM-dd}', end_date = '{6:yyyy-MM-dd}'
                        WHERE Exercise_id = '{0}'";

                int res = DBUtl.ExecSQL(update, e.Exercise_id, e.Package_id, e.company, e.unit, e.description, e.start_date, e.end_date);
                if (res == 1)
                {
                    TempData["Message"] = "Exercise Updated";
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

        [Authorize(Roles = "Admin, Platoon Commander, Officer Commander, Commandant Officer")]
        public IActionResult Delete(int id)
        {
            string select = "SELECT * FROM Exercise WHERE Exercise_id = {0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Exercise does not exist";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Exercise WHERE Exercise_id = {0}";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Exercise Deleted";
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


