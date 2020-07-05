using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FYP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FYP.Controller
{
    public class StocktakeController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IActionResult ViewStocktake()
        {
            updatearchive();
            DataTable dt = DBUtl.GetTable(@"Select Stocktake_id , quantity as [Quantity] , date_created as [Date Created] , full_name as [Created By] from Stocktaking s 
                                            INNER JOIN Users u ON s.User_id = u.User_id
                                            Where s.archive = 0");
            return View("ViewStocktake", dt.Rows);
        }
        public IActionResult ViewArchive()
        {
            updatearchive();
            DataTable dt = DBUtl.GetTable(@"Select Stocktake_id , quantity as [Quantity] , date_created as [Date Created] , full_name as [Created By] from Stocktaking s 
                                            INNER JOIN Users u ON s.User_id = u.User_id
                                            Where s.archive = 1");
            return View("ViewArchive", dt.Rows);
        }

        private void updatearchive()
        {
            var list = DBUtl.GetList<Stocktake>("Select * from Stocktaking");
            DateTime firstdate = DateTime.Now;
            
            foreach (var a in list)
            {
                DateTime seconddate = a.date_created;
                String diff = (firstdate - seconddate).TotalDays.ToString();
                double archivable = Double.Parse(diff);
                if(archivable > 365)
                {
                    var update = "Update Stocktaking Set archive = '{0}' Where Stocktake_id = '{1}'";
                    DBUtl.ExecSQL(update, true, a.Stocktake_id);

                }
            }
        }

        public IActionResult AddStocktake()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddStocktake(IFormFile postedFile)
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

                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            equipment.Add(new Equipment
                            {
                                Equipment_id = int.Parse(rows[0].ToString()),
                                Serial_id = int.Parse(rows[1].ToString()),
                                storage_location = rows[2].ToString(),
                                storage_detail = rows[3].ToString(),
                                quantity = int.Parse(rows[4].ToString()),

                            });
                        }

                    }


                    String message = "";
                    int total = 0;
                    var user = DBUtl.GetList<Users>("SELECT * FROM Users WHERE nric = '" + User.Identity.Name + " ' ");
                    var user_id = 0;
                    var archive = false;

                    foreach (var c in user)
                    {
                        user_id = c.User_id;
                    }
                    foreach (var a in equipment)
                    {
                        total += a.quantity;
                        bool correct = false;
                        var currequip = DBUtl.GetList<Equipment>("SELECT * FROM Equipment");


                        foreach (var b in currequip)
                        {
                            if (a.quantity == b.quantity)
                            {
                                correct = true;

                            }

                        }
                        if (correct != true)
                        {
                            message += String.Format("Equipment {0} quantity do not match" + Environment.NewLine, a.Equipment_id);
                        }


                    }

                    String insert = @"INSERT INTO Stocktaking(User_id , quantity , date_created , comments , archive)
                                     Values ('{0}' , '{1}' , '{2:yyyy-MM-dd}' , '{3}' , '{4}')";



                    if (message == " ")
                    {
                        message += "No discrepancies found during stocktaking";
                        DBUtl.ExecSQL(insert, user_id, total, DateTime.Now, message , archive);
                        TempData["Message"] = "No discrepancies found during stocktaking";
                        TempData["MsgType"] = "success";

                    }
                    else
                    {
                        DBUtl.ExecSQL(insert, user_id, total, DateTime.Now, message, archive);
                        TempData["Message"] = "Discrepancies were found during stocktaking , check comments for more details";
                        TempData["MsgType"] = "warning";

                    }
                    var list = DBUtl.GetList<Stocktake>("Select * From Stocktaking");
                    var count = 0;
                    foreach(var a in list)
                    {
                        count++;
                    }
                    return RedirectToAction("ViewComments");
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
        public IActionResult Delete(int id)
        {
            string select = @"SELECT * FROM Stocktaking WHERE  Stocktake_id ={0}";
            DataTable ds = DBUtl.GetTable(select, id);
            if (ds.Rows.Count != 1)
            {
                TempData["Message"] = "Stocktake does not exist";
                TempData["MsgType"] = "warning";
            }
            else
            {
                string delete = "DELETE FROM Stocktaking WHERE Stocktake_id={0}";
                int res = DBUtl.ExecSQL(delete, id);
                if (res == 1)
                {
                    TempData["Message"] = "Stocktake Deleted";
                    TempData["MsgType"] = "success";
                }
                else
                {
                    TempData["Message"] = DBUtl.DB_Message;
                    TempData["MsgType"] = "danger";
                }
            }
            return RedirectToAction("ViewStocktake");
        }
        public IActionResult ViewComments()
        {
            var list = DBUtl.GetList<Stocktake>("Select * From Stocktaking");
            var count = 0;
            foreach(var a in list)
            {
                count++;
            }
            string select = "SELECT * FROM Stocktaking WHERE Stocktake_id = '{0}'";
            List<Stocktake> list1 = DBUtl.GetList<Stocktake>(select, count);
            if (list1.Count == 1)
            {
                return View(list1[0]);
            }
            else
            {
                TempData["Message"] = "Stocktaking not found";
                TempData["MsgType"] = "warning";
                return RedirectToAction("ViewStocktake");
            }
        }
    }
}
