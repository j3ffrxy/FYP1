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
            DataTable dt = DBUtl.GetTable(@"Select Stocktaking_id , total_equipment_quantity as [Total Equipment Quantity] , total_accessories_quantity as [Total Accessories Quantity] , date_created as [Date Created] , full_name as [Created By] from Stocktaking s 
                                            INNER JOIN Users u ON s.User_id = u.User_id
                                            Where s.archive = 0");
            return View("ViewStocktake", dt.Rows);
        }
        public IActionResult ViewArchive()
        {
            updatearchive();
            DataTable dt = DBUtl.GetTable(@"Select Stocktaking_id , total_equipment_quantity as [Total Equipment Quantity] , total_accessories_quantity as [Total Accessories Quantity] , date_created as [Date Created] , full_name as [Created By] from Stocktaking s 
                                            INNER JOIN Users u ON s.User_id = u.User_id
                                            Where s.archive = ");
            return View("ViewArchive", dt.Rows);
        }

        private void updatearchive()
        {
            var list = DBUtl.GetList<Stocktaking>("Select * from Stocktaking");
            DateTime firstdate = DateTime.Now;
            
            foreach (var a in list)
            {
                DateTime seconddate = a.date_created;
                String diff = (firstdate - seconddate).TotalDays.ToString();
                double archivable = Double.Parse(diff);
                if(archivable > 30)
                {
                    var update = "Update Stocktaking Set archive = '{0}' Where Stocktaking_id = '{1}'";
                    DBUtl.ExecSQL(update, true, a.Stocktaking_id);

                }
            }
        }

        public int GetCurrentStocktake()
        {
            var user = DBUtl.GetList<Users>("SELECT * FROM Users WHERE nric = '" + User.Identity.Name + " ' ");
            var stocktakingPerson = DBUtl.GetList<Stocktaking>(@"SELECT * FROM Stocktaking WHERE User_id ='" + user[0].User_id + "' ORDER BY date_created DESC");
            var model = stocktakingPerson[0].Stocktaking_id;
            return model;

            
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
                    var accessory = new List<Equipment_Accessories>();
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
                                Type_desc = rows[4].ToString(),
                                Status = rows[4].ToString(),


                            }); ;
                           
                            accessory.Add(new Equipment_Accessories
                            {
                                 Equipment_accessories_id = Int32.Parse(rows[5].ToString()),
                                 Accessories_details = rows[6].ToString(),
                                 Storage_location = rows[7].ToString(),
                                 Quantity = Int32.Parse(rows[8].ToString())

                            }); ;
                            

                            
                        }

                    }


                    String message = "";
                    int equip_total = 0;
                    int access_total = 0;
                    var user = DBUtl.GetList<Users>("SELECT * FROM Users WHERE nric = '" + User.Identity.Name + " ' ");
                    var archive = false;

                    
                    foreach (var a in equipment)
                    {
                        equip_total += a.Quantity;
                        var currequip = DBUtl.GetList<Equipment>("SELECT * FROM Equipment");
                        foreach(var b in currequip)
                        {
                            if(a.Serial_no == b.Serial_no)
                            {
                                if(a.Quantity != b.Quantity)
                                {
                                    message += String.Format("Equipment {0} quantity do not match" + Environment.NewLine, a.Serial_no);
                                }
                            }
                           
                        }
                    }

                    foreach(var c in accessory)
                    {
                        access_total += c.Quantity;
                        
                        var curracess = DBUtl.GetList<Equipment_Accessories>("Select * From Equipment_accessories");
                        foreach (var d in curracess)
                        {
                            if(c.Equipment_accessories_id == d.Equipment_accessories_id)
                            {
                                if (c.Quantity != d.Quantity)
                                {
                                    message += String.Format("Accessory {0} quantity do not match" + Environment.NewLine, c.Equipment_accessories_id);
                                }
                            }
                        }
                    }

                    String insert = @"INSERT INTO Stocktaking(User_id , total_equipment_quantity , total_accessories_quantity, date_created , comments , archive)
                                     Values ('{0}' , '{1}' , '{2}' , '{3:yyyy-MM-dd HH:mm:ss}' , '{4}' , '{5}')";



                    if (message == "")
                    {
                        message += "No discrepancies found during stocktaking";
                        DBUtl.ExecSQL(insert, user[0].User_id, equip_total,  access_total , DateTime.Now, message , archive);

                    }
                    else
                    {
                        DBUtl.ExecSQL(insert, user[0].User_id, equip_total,  access_total, DateTime.Now, message, archive);

                    }

                    var stocktake = GetCurrentStocktake();
                  
                   
                    foreach ( var e in equipment)
                    {
                        String insertequip = @"INSERT INTO Stocktaking_Equipment(Serial_no , Stocktaking_id , Equipment_name , Quantity , Storage_location , Type_desc)
                                     Values ('{0}' , '{1}' , '{2}' , '{3}' , '{4}' , '{5}')";

                       int res = DBUtl.ExecSQL(insertequip, e.Serial_no, stocktake, e.Equipment_name, e.Quantity, e.Storage_location, e.Type_desc);
                       
                        
                    }

                    foreach(var f in accessory)
                    {
                        String insertacess = @"INSERT INTO Stocktaking_Accessories(Stocktaking_id , Equipment_accessories_id, Accessories_details , Storage_location , Quantity)
                                     Values ('{0}' , '{1}' , '{2}' , '{3}' , '{4}')";

                        DBUtl.ExecSQL(insertacess, stocktake, f.Equipment_accessories_id, f.Accessories_details, f.Storage_location, f.Quantity);
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

            
        public IActionResult ViewComments()
        {

            string select = "SELECT * FROM Stocktaking WHERE Stocktaking_id = '{0}'";
            var stocktake = GetCurrentStocktake();
            
            List<Stocktaking> list1 = DBUtl.GetList<Stocktaking>(select, stocktake);
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
      
    

