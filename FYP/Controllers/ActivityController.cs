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
    }
}