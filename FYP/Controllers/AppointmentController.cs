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

            DataTable dt = DBUtl.GetTable("SELECT A.User_id ,  U.full_name AS [Full Name], A.date AS [Date], SD.description AS [Equipment], A.quantity AS [Quantity], A.appt_desc AS [Description] FROM Appointment A INNER JOIN Users U ON U.User_id = A.User_id AND INNER JOIN Equipment E ON A.Equipment_id = E.Equipment_id AND INNER JOIN Serial_detail SD ON SD.Serial_id = E.Serial_id");
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
                    @"INSERT INTO Users(Equipment_id, date, quantity, appt_desc, nric)
                      Values ('{0}' , '{1:dd-MM-yyyy}' , {2} , '{3}' , '{4}')";

                int res = DBUtl.ExecSQL(insert, a.Equipment_id, a.date, a.quantity, a.appt_desc, a.nric);

                if (res == 1)
                {
                    TempData["Message"] = "Appointment Created";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Appointment Creation Failed";
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("Index");
            }
        }

        public IActionResult Edit(int id)
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
                return RedirectToAction("Edit");
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
            return RedirectToAction("View");
        }
    }
}