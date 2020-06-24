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
    public class EquipmentController : Microsoft.AspNetCore.Mvc.Controller
    {

        public IActionResult Index()
        {
            string reload = "DELETE FROM bluet";
            DBUtl.ExecSQL(reload);
            List<equipment> equipList = DBUtl.GetList<equipment>(
                  @"SELECT * FROM Equipment");
            return View(equipList);
        }

       

        [HttpGet]
        public IActionResult AddEquipment()
        {
       
            return View();

        }

        [HttpPost]
        public IActionResult AddEquipment(equipment newEquipment)
        {

            if (!ModelState.IsValid)
            {

              
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("AddEquipment");
            }
            else
            {
                string equipmentID = newEquipment.Serial_id;

                string insert =
                   @"INSERT INTO Equipment(Equipment_id,Serial_id,Storage_location,Quantity,Equipment_name)
                                 VALUES('{0}','{1}','{2}','{3}','{4}'";


                int result = DBUtl.ExecSQL(insert, equipmentID,
                    newEquipment.Serial_id,
                    newEquipment.Storage_location,
                    newEquipment.Quantity, newEquipment.Equipment_name);

                if (result == 1)
                {
                    TempData["Message"] = "Equipment Created";
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
        public IActionResult EditEquipment(string id)
        {

            // Get the record from the database using the id
            string equipmentSql = @"SELECT * FROM Equipment WHERE  Equipment_id='{0}'";
            List<equipment> lstEquipment = DBUtl.GetList<equipment>(equipmentSql, id);

            // If the record is found, pass the model to the View
            if (lstEquipment.Count == 1)
            {
                return View(lstEquipment[0]);
            }
            {
                TempData["Message"] = "Equipment not found.";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public IActionResult EditEquipment(equipment newEquipment)
        {

            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Please fill up all the blanks";
                ViewData["MsgTye"] = "warning";
                return View();
            }

        
            // Write the SQL statement
            string update = "UPDATE Equipment SET Storage_location='{1}',Quantity ={3},Equipment_name='{4}' WHERE Equipment_id='{5}'";

            // Check the result and branch
            // If successful set a TempData success Message and MsgType
            // If unsuccessful, set a TempData message that equals the DBUtl error message

            if (DBUtl.ExecSQL(update, newEquipment.Equipment_id, newEquipment.Storage_location,
                    newEquipment.Quantity, newEquipment.Equipment_name) == 1)
            {
                TempData["Message"] = "Successfully updated Equipment";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Message"] = DBUtl.DB_Message;
                TempData["MsgType"] = "danger";
            }

            // Call the action ListMovies to show the result of the update
            return RedirectToAction("Index");
        }
        public IActionResult DeleteEquipment(string id)
        {
            string select = @"SELECT * FROM Equipment 
                              WHERE Equipment_id='{0}'";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Equipment record no longer exists.";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Equipment WHERE Equipment_id='{0}'";
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

          

         
            }

         
        }

      
  

    
