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
    public class EquipmentController : Microsoft.AspNetCore.Mvc.Controller
    {

        public IActionResult Index()
        {
            DataTable dt = DBUtl.GetTable("SELECT * FROM Equipment");
            return View("Index", dt.Rows);
            
        }

        [Authorize(Roles = "Admin, Store Supervisor")]
        public IActionResult EquipmentMaint()
        {
            updateMaint();
            List<Equipment> dt = DBUtl.GetList<Equipment>(@"SELECT * FROM Equipment WHERE Status = 'Available'");
            return View("EquipmentMaint", dt);
        }

        [Authorize(Roles = "Admin, Store Supervisor")]
        public IActionResult EquipmentMaintCancel()
        {
            updateMaint();
            List<Equipment> dt = DBUtl.GetList<Equipment>(@"SELECT * FROM Equipment WHERE Status = 'Maintenance'");
            return View("EquipmentMaintCancel", dt);
        }
        [Authorize(Roles = "Admin, Store Supervisor")]

        [HttpGet]
        public IActionResult AddEquipment()
        {

            return View();

        }
        [Authorize(Roles = "Admin, Store Supervisor")]

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
                   @"INSERT INTO Equipment(Serial_no,Equipment_name,Storage_location,Type_desc, Status, Assigned )
                                 VALUES('{0}','{1}','{2}','{3}', 'Available', '{5}')";


                int result = DBUtl.ExecSQL(insert, newEquipment.Serial_no, newEquipment.Equipment_name,
                    newEquipment.Storage_location, newEquipment.Type_desc, newEquipment.Status, newEquipment.Assigned);

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

        [Authorize(Roles = "Admin, Store Supervisor")]

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
        [Authorize(Roles = "Admin, Store Supervisor")]


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
        [Authorize(Roles = "Admin, Store Supervisor")]

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
        [Authorize(Roles = "Admin, Store Supervisor")]

        public IActionResult MassAdd()
        {
            return View();
        }
        [Authorize(Roles = "Admin, Store Supervisor")]

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
        }

        [Authorize(Roles = "Admin, Store Supervisor")]
        public IActionResult ConductMaint(int id)
        {

            string select = "SELECT * FROM Equipment WHERE  Serial_no='{0}'";
            List<Equipment> list = DBUtl.GetList<Equipment>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "No records found.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("EquipmentMaint");
            }

        }

        [Authorize(Roles = "Admin, Store Supervisor")]
        [HttpPost]
        public IActionResult ConductMaint(Equipment e)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View();
            }
            else
            {
                bool archive = false;
                bool maint_type = true;
                string statUpdate =
                   @"INSERT INTO Maintenance(Serial_no, Start_date, End_date, description, maint_type, archive)
                   VALUES('{0}', '{1:yyyy-MM-dd}','{2:yyyy-MM-dd}', '{3}', '{4}', '{5}')";
                int plswork = DBUtl.ExecSQL(statUpdate, e.Serial_no, e.m_start_date, e.m_end_date, "Equipment Maintenance", maint_type, archive);
                
                string insert =
                    @"UPDATE Equipment SET Status = '{0}', m_start_date = '{1:yyyy-MM-dd}', m_end_date = '{2:yyyy-MM-dd}' WHERE Serial_no = '{3}' AND Status = 'Available'";
                                
               
                int result = DBUtl.ExecSQL(insert, "Maintenance", e.m_start_date, e.m_end_date, e.Serial_no);

                if (result == 1 && plswork == 1)
                {
                    TempData["Message"] = "Sent for Maintenance";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("EquipmentMaint");
            }
        }


        [Authorize(Roles = "Admin, Store Supervisor")]
        public IActionResult CancelMaint(string id)
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
                string update = "UPDATE Equipment SET Status = 'Available' WHERE Serial_no = '{0}' AND Status = 'Maintenance'";
                int res = DBUtl.ExecSQL(update, id);
                if (res == 1)
                {
                    TempData["Message"] = "Maintenance Notice Cancelled";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = "Something went wrong.";
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("EquipmentMaintCancel");
        }

        private void updateMaint()
        {
            var list = DBUtl.GetList<Maintenance>("SELECT * FROM Maintenance WHERE maint_type = '{0}'", true);
            DateTime currentdate = DateTime.Now;

            foreach (var a in list)
            {
                DateTime enddate = a.End_date;
                if (enddate < currentdate)
                {
                    var updateEq = "UPDATE Equipment SET Status = 'Available' WHERE Status = 'Maintenance' AND Serial_no = '{0}' AND m_end_date < '{1}'";
                    DBUtl.ExecSQL(updateEq, a.Serial_no, currentdate);

                    var update = "UPDATE Maintenance SET archive = '{0}' WHERE End_date < '{1}'";
                    DBUtl.ExecSQL(update, true, currentdate);

                }
            }
        }
    }
}