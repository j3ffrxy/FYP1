﻿using System;
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
                if(archivable > 30)
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
                                Serial_no = rows[0].ToString(),
                               
                                Equipment_name = rows[2].ToString(),
                                Storage_location = rows[3].ToString(),
                                Quantity = int.Parse(rows[4].ToString()),

                            });
                        }

                    }


                    String message = "";
                    int total = 0;
                    var user = DBUtl.GetList<Users>("SELECT * FROM Users WHERE nric = '" + User.Identity.Name + " ' ");
                    var archive = false;

                    
                    foreach (var a in equipment)
                    {
                        total += a.Quantity;
                        bool correct = false;
                        var currequip = DBUtl.GetList<Equipment>("SELECT * FROM Equipment");


                        foreach (var b in currequip)
                        {
                            if (a.Quantity == b.Quantity)
                            {
                                correct = true;

                            }

                        }
                        if (correct != true)
                        {
                            message += String.Format("Equipment {0} quantity do not match" + Environment.NewLine, a.Serial_no);
                        }


                    }

                    String insert = @"INSERT INTO Stocktaking(User_id , quantity , date_created , comments , archive)
                                     Values ('{0}' , '{1}' , '{2:yyyy-MM-dd}' , '{3}' , '{4}')";



                    if (message == "")
                    {
                        message += "No discrepancies found during stocktaking";
                        DBUtl.ExecSQL(insert, user[0].User_id, total, DateTime.Now, message , archive);

                    }
                    else
                    {
                        DBUtl.ExecSQL(insert, user[0].User_id, total, DateTime.Now, message, archive);

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

            
        public IActionResult ViewComments(int id)
        {
            bool newstocktake = true;
            if(id > 0)
            {
                newstocktake = false;
            }
            if(newstocktake == true)
            {
                var list = DBUtl.GetList<Stocktake>("Select * From Stocktaking");
                var count = 0;
                foreach (var a in list)
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
            else
            {
                string select = "SELECT * FROM Stocktaking WHERE Stocktake_id = '{0}'";
                List<Stocktake> list1 = DBUtl.GetList<Stocktake>(select, id);
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
}