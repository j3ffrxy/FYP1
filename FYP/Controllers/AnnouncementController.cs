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

namespace FYP.Controllers
{
    public class AnnouncementController : Microsoft.AspNetCore.Mvc.Controller
    {
     

        public IActionResult Index()
        {
            DataTable dt = DBUtl.GetTable("SELECT * FROM Announcement");
            return View("Index", dt.Rows);

        }


        [Authorize(Roles = "Admin,Store Supervisor")]
        [HttpGet]
        public IActionResult AddAnnouncement()
        {

            return View();

        }
        [Authorize(Roles = "Admin,Store Supervisor")]

        [HttpPost]
        public IActionResult AddAnnouncement(Announcement newAnnounce)
        {

            if (!ModelState.IsValid)
            {


                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("AddEquipment");
            }
            else
            {

                string insert =
                   @"INSERT INTO Announcement(Announcement_desc,Start_date,End_date)
                                 VALUES('{0}', '{1:yyyy-MM-dd}', '{2:yyyy-MM-dd}')";


                int result = DBUtl.ExecSQL(insert, newAnnounce.Announcement_desc,
                    newAnnounce.Start_date, newAnnounce.End_date);

                if (result == 1)
                {
                    TempData["Message"] = "Announcement added";
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

        [Authorize(Roles = "Admin,Store Supervisor")]

        [HttpGet]
        public IActionResult EditAnnouncement(int id)
        {

            // Get the record from the database using the id
            string select = "SELECT * FROM Announcement WHERE  Announcement_id='{0}'";
            List<Announcement> list = DBUtl.GetList<Announcement>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "Announcement not found.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Index");
            }

        }
        [Authorize(Roles = "Admin,Store Supervisor")]

        [HttpPost]
        public IActionResult EditAnnouncement(Announcement EditAnnounce)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Please fill up all the blanks";
                ViewData["MsgTye"] = "warning";
                return View();
            }


            string update = @"UPDATE Announcement SET Announcement_desc ='{1}', Start_date = '{2:yyyy-MM-dd}', End_date = '{3:yyyy-MM-dd}' WHERE Announcement_id ='{0}'";



            int res = DBUtl.ExecSQL(update, EditAnnounce.Announcement_id, EditAnnounce.Announcement_desc, EditAnnounce.Start_date,
                     EditAnnounce.End_date);
            if (res == 1)
            {
                TempData["Message"] = "Successfully updated Announcement";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Message"] = DBUtl.DB_Message;
                TempData["MsgType"] = "danger";
            }

            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin,Store Supervisor")]

        public IActionResult DeleteAnnouncement(string id)
        {
            string select = @"SELECT * FROM Announcement 
                              WHERE Announcement_id='{0}'";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Announcement record no longer exists.";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Announcement WHERE Announcement_id='{0}'";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Announcement Deleted";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Please delete related records before deleting this record!";
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("Index");
        }
      
    }
       
    }

        