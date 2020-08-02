using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FYP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Data.OleDb;
using System.IO;
using System.Text;


namespace FYP.Controllers
{
    public class UserController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IActionResult Index()
        {

            return View("About");
        }

        public string GetEquipment()
        {
            var list = DBUtl.GetList<Equipment>("Select * from Equipment WHERE Assigned = 'False' AND Type_desc = 'SAR-21' ");
            var equip = list[0];
            string update = "Update Equipment Set Assigned = 'True' Where Serial_no = '" + equip.Serial_no + "'";
            DBUtl.ExecSQL(update);
            return equip.Serial_no;
        }

        public IActionResult About()
        {
            DataTable dt = DBUtl.GetTable(@"SELECT User_id ,  full_name AS [Full Name] , rank AS [Rank] , dob AS [Date of Birth] , unit AS [Unit] , company AS [Company] FROM Users");
            return View("Index", dt.Rows);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Create(Users user)
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
                    @"INSERT INTO Users(Serial_no , nric , password , full_name , dob , rank , unit , company , role )
                      Values ('{0}' , '{1}' , HASHBYTES('SHA1', '{2}'), '{3}', '{4:yyyy-MM-dd}' , '{5}' , '{6}' , '{7}' , '{8}' )";

                List<Users> existuser = DBUtl.GetList<Users>("SELECT * FROM Users");
                bool exists = false;

                

                foreach(var a in existuser)
                {
                    if (a.nric.Equals(user.nric))
                    {
                        exists = true;
                    }
                }
                if(exists == false)
                {
                    int res = DBUtl.ExecSQL(insert, GetEquipment() , user.nric , user.password , user.full_name , user.dob , user.rank , user.unit , user.company , user.role);
                    if (res == 1)
                    {
                        TempData["Message"] = "User Created";
                        TempData["MsgType"] = "success";
                    }
                    else
                    {
                        TempData["Message"] = DBUtl.DB_Message;
                        TempData["MsgType"] = "danger";
                    }
                }
                else
                {
                    TempData["Message"] = "User already exists";
                    TempData["MsgType"] = "danger";
                }

               
                return RedirectToAction("About");
            }
        }
        public IActionResult Edits(int id)
        {
            string select = "SELECT * FROM Users WHERE User_id = '{0}'";
            List<Users> list = DBUtl.GetList<Users>(select, id);
            if (list.Count == 1)
            {
                return View(list[0]);
            }
            else
            {
                TempData["Message"] = "User not found";
                TempData["MsgType"] = "warning";
                return RedirectToAction("Edits");
            }
        }

        [HttpPost]
        public IActionResult Edits(Users user)
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
                  @"UPDATE Users
                     SET nric ='{1}', password = HASHBYTES('SHA1', '{2}') , full_name = '{3}' , dob = '{4:yyyy-MM-dd}' ,  rank = '{5}', 
                        unit = '{6}' , company = '{7}' , role = '{8}'
                        WHERE User_id = '{0}'";
                int res = DBUtl.ExecSQL(update, user.User_id, user.nric, user.password, user.full_name, user.dob, user.rank, user.unit, user.company, user.role);
                if (res == 1)
                {
                    TempData["Message"] = "User Updated";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
                return RedirectToAction("About");
            }
        }
        public IActionResult Delete(int id)
        {
            var user = DBUtl.GetList<Users>(@"SELECT * FROM Users WHERE  User_id = '" + id + "'");
            
            if (user.Count != 1)
            {
                TempData["Message"] = "User does not exist";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Users WHERE User_id={0}";
                string unassign = "Update Equipment Set Assigned = 'False' Where Serial_no = '" + user[0].Serial_no + "'";
                DBUtl.ExecSQL(unassign);
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "User Deleted";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("About");
        }
        public IActionResult VerifyDate(DateTime dob)
        {
            DateTime firstdate = DateTime.Today;
            DateTime seconddate = dob;

            String diff = (firstdate - seconddate).TotalDays.ToString();
            int legible = Int32.Parse(diff);
            if (legible < 5840)
            {
                return Json($"User is still not eligible for NS");
            }
            return Json(true);
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


                    var user = new List<Users>();
                    using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split(',');

                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            user.Add(new Users
                            {
                                nric = rows[0].ToString(),
                                Serial_no = GetEquipment(),
                                password = rows[1].ToString(),
                                full_name = rows[2].ToString(),
                                dob = DateTime.Parse(rows[3].ToString()),
                                rank = rows[4].ToString(),
                                unit = rows[5].ToString(),
                                company = rows[6].ToString(),
                                role = rows[7].ToString()

                            });
                        }

                    }
                    int count = 0;
                    
                    foreach (Users u in user)
                    {
                        List<Users> existuser = DBUtl.GetList<Users>("SELECT * FROM Users");
                        bool exists = false;
                        foreach (var a in existuser)
                        {
                      
                            if (u.nric.Equals(a.nric))
                            {
                                exists = true;
                            }
                        }
                        if (exists == false)
                        {
                            string insert =
                   @"INSERT INTO Users(Serial_no , nric , password , full_name , dob , rank , unit , company , role )
                      Values ('{0}' , '{1}' , HASHBYTES('SHA1', '{2}'), '{3}', '{4:yyyy-MM-dd}' , '{5}' , '{6}' , '{7}' , '{8}' )";

                            int res = DBUtl.ExecSQL(insert,u.Serial_no , u.nric, u.password, u.full_name, u.dob, u.rank, u.unit, u.company, u.role);
                            if (res == 1)
                            {
                                count++;
                            }
                        }
                        else
                        {
                            TempData["Message"] = "User already exists";
                            TempData["MsgType"] = "danger";
                        }

                    }
                    if (count == user.Count)
                    {
                        TempData["Message"] = "All users have been created";
                        TempData["MsgType"] = "success";
                    }
                    else
                    {
                        TempData["Message"] = "Not all users have been created";
                        TempData["MsgType"] = "danger";
                    }

                    return RedirectToAction("About");
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