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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FYP.Controllers
{
    public class PackageController : Microsoft.AspNetCore.Mvc.Controller
    {

        public IActionResult Index()
        {
            DataTable dt = DBUtl.GetTable("SELECT * FROM Package");
            return View("Index", dt.Rows);

        }



        [HttpGet]
        public IActionResult AddPackage()
        {

            return View();

        }

        [HttpPost]
        public IActionResult AddPackage(Package newPackage)
        {

            if (!ModelState.IsValid)
            {


                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("AddPackage");
            }
            else
            {

                string insert =
                   @"INSERT INTO Package(Serial_no,Equipment_accessories_id,Name)
                                 VALUES('{0}','{1}','{2}')";


                int result = DBUtl.ExecSQL(insert, newPackage.Serial_no, newPackage.Equipment_accessories_id,
                    newPackage.Name);

                if (result == 1)
                {
                    TempData["Message"] = "Package Created";
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


        [HttpGet]
        public IActionResult EditPackage(int id)
        {

            // Get the record from the database using the id
            string select = "SELECT * FROM Package WHERE  Package_id='{0}'";
            List<Package> list = DBUtl.GetList<Package>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "Package not found.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public IActionResult EditPackage(Package EditPack)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Please fill up all the blanks";
                ViewData["MsgTye"] = "warning";
                return View();
            }


            string update = @"UPDATE Package SET Serial_no='{1}', Equipment_accessories_id='{2}', Name = '{3}' WHERE Package_id ='{0}'";



            int res = DBUtl.ExecSQL(update, EditPack.Serial_no, EditPack.Equipment_accessories_id, EditPack.Name);
            if (res == 1)
            {
                TempData["Message"] = "Successfully updated Package";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Message"] = DBUtl.DB_Message;
                TempData["MsgType"] = "danger";
            }

            return RedirectToAction("Index");
        }
        public IActionResult DeletePackage(string id)
        {
            string select = @"SELECT * FROM Package 
                              WHERE Serial_no='{0}'";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Package record no longer exists.";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Pacakge WHERE Package_id='{0}'";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Package Deleted";
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