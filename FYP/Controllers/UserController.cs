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

            DataTable dt = DBUtl.GetTable(@"SELECT User_id ,  full_name AS [Full Name] , rank AS [Rank] , dob AS [Date of Birth] , B.name AS [Brigade] , C.name AS [Company] , P.name AS [Platoon] FROM Users U 
                                            INNER JOIN User_group UG ON U.Group_id = UG.Group_id 
                                            INNER JOIN Brigade B ON B.Brigade_id = U.Brigade_id 
                                            INNER JOIN Company C ON C.Company_id = U.Company_id
                                            INNER JOIN Platoon P ON P.Platoon_id = U.Platoon_id ");
            return View("Index", dt.Rows);
        }

        public IActionResult About()
        {
            return View("About");
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
                    @"INSERT INTO Users(Group_id , Brigade_id, Company_id , Platoon_id ,  full_name , dob , nric , password, rank)
                      Values ('{0}' , '{1}', '{2}', '{3}' , '{4}' , '{5:yyyy-MM-dd}' , '{6}' , HASHBYTES('SHA1', '{7}') , '{8}')";
                List<Users> existuser = DBUtl.GetList<Users>("SELECT * FROM USERS");
                bool exist = false;
                foreach(Users a in existuser)
                {
                    if( a.nric == user.nric)
                    {
                        exist = true;
                    }if(exist != true)
                    {
                        int res = DBUtl.ExecSQL(insert, user.Group_id, user.Brigade_id, user.Company_id, user.Platoon_id, user.full_name, user.dob, user.nric, user.password, user.rank);

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
                }
               
                return RedirectToAction("Index");
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
                return View("Edit");
            }
            else
            {
                string update =
                   @"UPDATE Users
                    SET Group_id ='{1}', Brigade_id = '{2}' , Company_id = '{3}' , Platoon_id = '{4}' ,  full_name='{5}', 
                        dob ='{6:dd-MM-yyyy}' , nric = '{7}' , password = HASHBYTES('SHA1', '{8}') , rank = '{9}' 
                        WHERE User_id = '{0}'";
                int res = DBUtl.ExecSQL(update, user.User_id ,user.Group_id, user.Brigade_id, user.Company_id, user.Platoon_id, user.full_name, user.dob, user.nric, user.password, user.rank);
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
                return RedirectToAction("Index");
            }
        }
        public IActionResult Delete(int id)
        {
            string select = @"SELECT * FROM Users WHERE  User_id ={0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "User does not exist";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Users WHERE User_id={0}";
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
            return RedirectToAction("Index");
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

                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            user.Add(new Users
                            {
                                Group_id = int.Parse(rows[0].ToString()),
                                Brigade_id = int.Parse(rows[1].ToString()),
                                Company_id = int.Parse(rows[2].ToString()),
                                Platoon_id = int.Parse(rows[3].ToString()),
                                full_name = rows[4].ToString(),
                                dob = DateTime.Parse(rows[5].ToString()),
                                nric = rows[6].ToString(),
                                password = rows[7].ToString(),
                                rank = rows[8].ToString()

                            });
                        }

                    }
                    int count = 0;
                    bool exists = false;
                    foreach (Users u in user)
                    {
                        List<Users> existuser = DBUtl.GetList<Users>("SELECT * FROM USERS");
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
                    @"INSERT INTO Users(Group_id , Brigade_id, Company_id , Platoon_id ,  full_name , dob , nric , password, rank)
                      Values ('{0}' , '{1}', '{2}', '{3}' , '{4}' , '{5:yyyy-MM-dd}' , '{6}' , HASHBYTES('SHA1', '{7}') , '{8}')";

                            int res = DBUtl.ExecSQL(insert, u.Group_id, u.Brigade_id, u.Company_id, u.Platoon_id, u.full_name, u.dob, u.nric, u.password, u.rank);

                            
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