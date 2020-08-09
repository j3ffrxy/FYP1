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
    public class AccessoriesController : Microsoft.AspNetCore.Mvc.Controller
    {

        public IActionResult Index()
        {
            DataTable dt = DBUtl.GetTable("SELECT * FROM Equipment_accessories");
            return View("Index", dt.Rows);

        }


        [Authorize(Roles = "Admin,Store Supervisor")]

        [HttpGet]
        public IActionResult AddAccessories()
        {

            return View();

        }
        [Authorize(Roles = "Admin,Store Supervisor")]

        [HttpPost]
        public IActionResult AddAccessories(Equipment_Accessories newAccessories)
        {

            if (!ModelState.IsValid)
            {


                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("AddAccessories");
            }
            else
            {

                string insert =
                   @"INSERT INTO Equipment_accessories(Accessories_details,Storage_location,Quantity)
                                 VALUES('{0}','{1}','{2}')";


                int result = DBUtl.ExecSQL(insert, newAccessories.Accessories_details,
                    newAccessories.Storage_location,
                    newAccessories.Quantity);

                if (result == 1)
                {
                    TempData["Message"] = "Accessory Created";
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
        public IActionResult EditAccessories(int id)
        {

            // Get the record from the database using the id
            string select = "SELECT * FROM Equipment_accessories WHERE  Equipment_accessories_id='{0}'";
            List<Equipment_Accessories> list = DBUtl.GetList<Equipment_Accessories>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "Accessory not found.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Index");
            }

        }
        [Authorize(Roles = "Admin,Store Supervisor")]

        [HttpPost]
        public IActionResult EditAccessories(Equipment_Accessories editAccessories)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Please fill up all the blanks";
                ViewData["MsgTye"] = "warning";
                return View();
            }


            string update = @"UPDATE Equipment_accessories SET Accessories_details='{1}', Quantity = '{2}',Storage_location='{3}' WHERE Equipment_accessories_id ='{0}'";



            int res = DBUtl.ExecSQL(update, editAccessories.Equipment_accessories_id, editAccessories.Accessories_details, editAccessories.Quantity,
                      editAccessories.Storage_location);
            if (res == 1)
            {
                TempData["Message"] = "Successfully updated Accessory";
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

        public IActionResult DeleteAccessory(string id)
        {
            string select = @"SELECT * FROM Equipment_accessories 
                              WHERE Equipment_accessories_id='{0}'";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Accessory record no longer exists.";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Equipment_accessories WHERE Equipment_accessories_id='{0}'";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Accessory Deleted";
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
        [Authorize(Roles = "Admin,Store Supervisor")]

        public IActionResult MassAdd()
        {
            return View();
        }
        [Authorize(Roles = "Admin,Store Supervisor")]

        [HttpPost]
        public ActionResult MassAdd(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                try
                {
                    string fileExtension = Path.GetExtension(postedFile.FileName);

                    //Validate uploaded file and return error.
                    if (fileExtension != ".csv")
                    {
                        ViewBag.Message = "Please select the csv file with .csv extension";
                        return View();
                    }


                    var accessory = new List<Equipment_Accessories>();
                    using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.

                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            accessory.Add(new Equipment_Accessories
                            {
                                Equipment_accessories_id = int.Parse(rows[0].ToString()),
                                Accessories_details = rows[1].ToString(),
                                Storage_location = rows[2].ToString(),
                                Quantity = int.Parse(rows[3].ToString()),
                            });
                        }

                    }
                    int count = 0;
                    bool exists = false;
                    foreach (Equipment_Accessories u in accessory)
                    {
                        List<Equipment_Accessories> list = DBUtl.GetList<Equipment_Accessories>("SELECT * FROM Equipment_accessories");
                        foreach (var a in list)
                        {
                            if (u.Equipment_accessories_id == (a.Equipment_accessories_id))
                            {
                                exists = true;
                            }
                        }
                        if (exists == false)
                        {
                            string insert =
                                      @"INSERT INTO Equipment(Accessories_details, Storage_location , Quantity )
                                     Values ('{0}' , '{1}' , '{2}')";

                            int res = DBUtl.ExecSQL(insert, u.Accessories_details, u.Storage_location, u.Quantity);
                            if (res == 1)
                            {
                                count++;
                            }
                        }
                        else
                        {
                            TempData["Message"] = "Accessory already exists";
                            TempData["MsgType"] = "danger";
                        }


                    }
                    if (count == accessory.Count)
                    {
                        TempData["Message"] = "All accessory have been created";
                        TempData["MsgType"] = "success";
                    }
                    else
                    {
                        TempData["Message"] = "Not all accessory have been created";
                        TempData["MsgType"] = "danger";
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Please select the file first to upload.";
            }
            return View();
        }
    }
}