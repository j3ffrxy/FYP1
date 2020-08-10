using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FYP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FYP.Controller
{
    public class StocktakeController : Microsoft.AspNetCore.Mvc.Controller
    {

        [Authorize(Roles = "Admin , Store Supervisor")]
        public IActionResult ViewStocktake()
        {
            updatearchive();
            List<Stocktaking> list = DBUtl.GetList<Stocktaking>(@"Select Stocktaking_id , total_equipment_quantity , total_accessories_quantity, date_created, u.full_name, diff_equip, diff_accessory from Stocktaking s 
                                            INNER JOIN Users u ON s.User_id = u.User_id
                                            Where s.archive = 0 ORDER BY date_created DESC");
            return View("ViewStocktake", list);
        }
        [Authorize(Roles = "Admin , Store Supervisor")]
        public IActionResult ViewArchive()
        {
            updatearchive();
            List<Stocktaking> list = DBUtl.GetList<Stocktaking>(@"Select Stocktaking_id , total_equipment_quantity , total_accessories_quantity, date_created, u.full_name, diff_equip, diff_accessory from Stocktaking s 
                                            INNER JOIN Users u ON s.User_id = u.User_id
                                            Where s.archive = 1 ORDER BY date_created DESC");
            return View("ViewArchive", list);
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
                if (archivable > 30)
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
        [Authorize(Roles = "Admin , Store Supervisor")]
        public IActionResult AddStocktake()
        {
            return View();
        }
        [Authorize(Roles = "Admin , Store Supervisor")]
        [HttpPost]
        public IActionResult AddStocktake(IFormFile postedFile, String storage_location)
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
                                Type_desc = rows[3].ToString(),
                                Status = rows[4].ToString(),
                                Assigned = Boolean.Parse(rows[5])

                            }); ;

                            accessory.Add(new Equipment_Accessories
                            {
                                Equipment_accessories_id = Int32.Parse(rows[6].ToString()),
                                Accessories_details = rows[7].ToString(),
                                Storage_location = rows[8].ToString(),
                                Quantity = Int32.Parse(rows[9].ToString())

                            }); ;



                        }

                    }




                    var user = DBUtl.GetList<Users>("SELECT * FROM Users WHERE nric = '" + User.Identity.Name + " ' ");
                    var currequip = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Storage_location = '" + storage_location + "'");
                    var curracess = DBUtl.GetList<Equipment_Accessories>("Select * From Equipment_accessories WHERE Storage_location ='" + storage_location + "'");
                    ArrayList diffequip = new ArrayList();
                    ArrayList diffaccess = new ArrayList();
                    ArrayList equip = new ArrayList();
                    var archive = false;
                    int curr_equip_total = currequip.Count;
                    int curr_access_total = 0;

                    foreach (var z in curracess)
                    {
                        curr_access_total += z.Quantity;
                    }


                    foreach (var a in equipment)
                    {
                        if (a.Storage_location.Equals(storage_location))
                        {
                            if (a.Status.Equals("Available"))
                            {
                                equip.Add(a);
                            }
                        }
                        
                        foreach (var b in currequip)
                        {
                            if (a.Storage_location.Equals(storage_location))
                            {
                                if (a.Serial_no.Equals(b.Serial_no))
                                {
                                    if (a.Status != b.Status)
                                    {
                                        diffequip.Add(a);
                                    }
                                }
                            }

                        }
                    }
                    int stock_access = 0;
                    foreach (var c in accessory)
                    {
                        if (c.Storage_location.Equals(storage_location))
                        {
                            stock_access += c.Quantity;
                        }

                        foreach (var d in curracess)
                        {
                            if (c.Storage_location.Equals(storage_location))
                            {
                                if (c.Equipment_accessories_id == d.Equipment_accessories_id)
                                {
                                    if (c.Quantity != d.Quantity)
                                    {
                                        diffaccess.Add(c);
                                    }
                                }
                            }
                        }
                    }

                    int stock_equip = equip.Count;

                    int curr_equip_avail = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Storage_location = '" + storage_location + "' AND Status = 'Available'").Count;

                    int diff_equip = stock_equip - curr_equip_avail;
                    int diff_access = stock_access - curr_access_total;



                    String insert = @"INSERT INTO Stocktaking(User_id , total_equipment_quantity , total_accessories_quantity, date_created, archive , diff_equip , diff_accessory , storage_location)
                                     Values ('{0}' , '{1}' , '{2}' , '{3:yyyy-MM-dd HH:mm:ss}', '{4}' , '{5}' , '{6}' , '{7}')";

                    int resu = DBUtl.ExecSQL(insert, user[0].User_id, stock_equip, stock_access, DateTime.Now, archive, diff_equip, diff_access, storage_location);






                    foreach (var e in equipment)
                    {
                        if (e.Storage_location.Equals(storage_location))
                        {
                            if (diffequip.Contains(e))
                            {
                                String insertequip = @"INSERT INTO Stocktaking_Equipment(Serial_no , Stocktaking_id , Equipment_name , Storage_location , Type_desc , Status , Assigned , Matching)
                                     Values ('{0}' , '{1}' , '{2}' , '{3}' , '{4}' , '{5}' , '{6}' , '{7}')";

                                DBUtl.ExecSQL(insertequip, e.Serial_no, GetCurrentStocktake(), e.Equipment_name, e.Storage_location, e.Type_desc, e.Status, e.Assigned, false);
                            }
                            else
                            {
                                String insertequip = @"INSERT INTO Stocktaking_Equipment(Serial_no , Stocktaking_id , Equipment_name , Storage_location , Type_desc , Status , Assigned , Matching)
                                     Values ('{0}' , '{1}' , '{2}' , '{3}' , '{4}' , '{5}' , '{6}' , '{7}')";

                                DBUtl.ExecSQL(insertequip, e.Serial_no, GetCurrentStocktake(), e.Equipment_name, e.Storage_location, e.Type_desc, e.Status, e.Assigned, true);
                            }
                        }
                        



                    }

                    foreach (var f in accessory)
                    {
                        if (f.Storage_location.Equals(storage_location))
                        {
                            if (diffaccess.Contains(f))
                            {
                                String insertacess = @"INSERT INTO Stocktaking_Accessories(Stocktaking_id , Equipment_accessories_id, Accessories_details , Storage_location , Quantity , Matching)
                                     Values ('{0}' , '{1}' , '{2}' , '{3}' , '{4}' , '{5}')";

                                DBUtl.ExecSQL(insertacess, GetCurrentStocktake(), f.Equipment_accessories_id, f.Accessories_details, f.Storage_location, f.Quantity, false);
                            }
                            else
                            {
                                String insertacess = @"INSERT INTO Stocktaking_Accessories(Stocktaking_id , Equipment_accessories_id, Accessories_details , Storage_location , Quantity , Matching)
                                     Values ('{0}' , '{1}' , '{2}' , '{3}' , '{4}' , '{5}')";

                                DBUtl.ExecSQL(insertacess, GetCurrentStocktake(), f.Equipment_accessories_id, f.Accessories_details, f.Storage_location, f.Quantity, true);
                            }
                        }
                        
                    }

                    return RedirectToAction("ViewCurrentStocktake");

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

        [Authorize(Roles = "Admin , Store Supervisor")]
        public IActionResult ViewEquipment(int id)
        {
            var stockequiplist = DBUtl.GetList<Stocktaking_Equipment>("Select * FROM Stocktaking_Equipment WHERE Stocktaking_id = '" + id + "' AND Matching = 'false'");
            return View("ViewEquipment", stockequiplist);
        }
        [Authorize(Roles = "Admin , Store Supervisor")]
        public IActionResult ViewAccessory(int id)
        {
            var stockaccesslist = DBUtl.GetList<Stocktaking_Accessories>("Select * FROM Stocktaking_Accessories WHERE Stocktaking_id = '" + id + "' AND Matching = 'false'");
            return View("ViewAccessory", stockaccesslist);
        }
        [Authorize(Roles = "Admin , Store Supervisor")]
        public IActionResult ViewCurrentStocktake()
        {
            int stocktake_id = GetCurrentStocktake();
            var currstocktake = DBUtl.GetList<Stocktaking>(@"Select Stocktaking_id , total_equipment_quantity , total_accessories_quantity, date_created, u.full_name, diff_equip, diff_accessory from Stocktaking s 
                                            INNER JOIN Users u ON s.User_id = u.User_id
                                            Where Stocktaking_id = '" + stocktake_id + "'");
            return View("ViewCurrentStocktake", currstocktake);
        }
    }
}

            

      
    

