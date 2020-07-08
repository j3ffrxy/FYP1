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
    public class AppointmentController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IActionResult Index()
        {

            DataTable dt = DBUtl.GetTable(@"SELECT Appt_id, A.date AS [Date], E.Equipment_name AS [Equipment], 
                                            A.quantity AS [Quantity], A.appt_desc AS [Description], A.nric AS [SAF11B], A.status AS [Status], A.Activity_id AS [Activity], A.remarks AS [Remarks]
                                            FROM Appointment A 
                                            INNER JOIN Equipment E ON A.Equipment_id = E.Equipment_id 
                                            INNER JOIN Activity AC ON A.Activity_id = AC.Activity_id");
            return View("Index", dt.Rows);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Appointment a)
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
                    @"INSERT INTO Appointment(Equipment_id, Activity_id, nric, date, quantity, appt_desc, status)
                      Values ('{0}' , '{1}' , '{2}' , '{3:yyyy-MM-dd}' , '{4}', '{5}', '{6}')";

                int res = DBUtl.ExecSQL(insert, a.Equipment_id, a.Activity_id, a.nric, a.date, a.quantity, a.appt_desc, a.status);

                if (res == 1)
                {
                    TempData["Message"] = "Appointment Created";
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
            string select = "SELECT * FROM Appointment WHERE Appt_id = '{0}'";
            List<Appointment> list = DBUtl.GetList<Appointment>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "Appointment does not exist.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Edits");
            }
        }

        [HttpPost]
        public IActionResult Edits(Appointment a)
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
                   @"UPDATE Appointment
                    SET Equipment_id='{1}', 
                        date ='{2:yyyy-MM-dd}' , quantity = {3}, appt_desc = '{4}' , nric ='{5}', status = '{6}', Activity_id = '{7}', remarks = '{8}'
                        WHERE Appt_id = '{0}'";
                int res = DBUtl.ExecSQL(update, a.Appt_id, a.Equipment_id, a.date, a.quantity, a.appt_desc, a.nric, a.status, a.Activity_id, a.remarks);
                
                if (res == 1)
                {
                    TempData["Message"] = "Appointment Updated";
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
            string select = "SELECT * FROM Appointment WHERE Appt_id = {0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Appointment does not exist";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Appointment WHERE Appt_id={0}";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Appointment Deleted";
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