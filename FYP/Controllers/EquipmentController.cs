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
    public class EquipmentController : Microsoft.AspNetCore.Mvc.Controller
    {

        /*public IActionResult Index()
        {
            DataTable dt = DBUtl.GetTable("SELECT * FROM Equipment");
            return View("Index", dt.Rows);

        }



        [HttpGet]
        public IActionResult AddEquipment()
        {

            return View();

        }

        [HttpPost]
        public IActionResult AddEquipment(Equipment newEquipment)
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
                   @"INSERT INTO Equipment(Serial_no,Equipment_name,Storage_location,Type_desc, Status)
                                 VALUES('{0}','{1}','{2}','{3}', '{4}')";


                int result = DBUtl.ExecSQL(insert, newEquipment.Serial_no, newEquipment.Equipment_name,
                    newEquipment.Storage_location, newEquipment.Type_desc, newEquipment.Status);

                if (result == 1)
                {
                    TempData["Message"] = "Equipment added";
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
        public IActionResult EditEquipment(int id)
        {

            // Get the record from the database using the id
            string select = "SELECT * FROM Equipment WHERE  Serial_no='{0}'";
            List<Equipment> list = DBUtl.GetList<Equipment>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "Equipment not found.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public IActionResult EditEquipment(Equipment EditEquip)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Please fill up all the blanks";
                ViewData["MsgTye"] = "warning";
                return View();
            }


            string update = @"UPDATE Equipment SET Equipment_name ='{1}', Storage_location = '{2}', Type_desc='{3}', Status = '{4}' WHERE Serial_no ='{0}'";



            int res = DBUtl.ExecSQL(update, EditEquip.Serial_no, EditEquip.Equipment_name, EditEquip.Storage_location,
                     EditEquip.Type_desc, EditEquip.Status);
            if (res == 1)
            {
                TempData["Message"] = "Successfully updated Equipment";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Message"] = DBUtl.DB_Message;
                TempData["MsgType"] = "danger";
            }

            return RedirectToAction("Index");
        }
        public IActionResult DeleteEquipment(string id)
        {
            string select = @"SELECT * FROM Equipment 
                              WHERE Serial_no='{0}'";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Equipment record no longer exists.";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Equipment WHERE Serial_no='{0}'";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Equipment Deleted";
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
        public IActionResult MassAdd()
        {
            return View();
        }
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


                    var equipment = new List<Equipment>();
                    using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split(',');

                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            equipment.Add(new Equipment
                            {
                                Serial_no = rows[0].ToString(),
                                Equipment_name = rows[1].ToString(),
                                Storage_location = rows[2].ToString(),
                                Type_desc = rows[3].ToString(),
                                Status =  rows[4].ToString(),
                            });
                        }

                    }
                    int count = 0;
                    foreach (Equipment u in equipment)
                    {
                        bool exists = false;

                        List<Equipment> list = DBUtl.GetList<Equipment>("SELECT * FROM Equipment");
                        foreach (var a in list)
                        {
                            if (u.Serial_no==(a.Serial_no))
                            {
                                exists = true;
                            }
                        }
                        if (exists == false)
                        {
                             string insert =
                   @"INSERT INTO Equipment(Serial_no,Equipment_name,Storage_location,Type_desc)
                                 VALUES('{0}','{1}','{2}','{3}')";


                            int res = DBUtl.ExecSQL(insert, u.Serial_no, u.Equipment_name, u.Storage_location, u.Type_desc);
                            if (res == 1)
                            {
                                count++;
                            }
                        }
                        else
                        {
                            TempData["Message"] = "Equipment already exists";
                            TempData["MsgType"] = "danger";
                        }


                    }
                    if (count == equipment.Count)
                    {
                        TempData["Message"] = "All equipments have been created";
                        TempData["MsgType"] = "success";
                    }
                    else
                    {
                        TempData["Message"] = "Not all equipments have been created";
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
        }*/
    }
}