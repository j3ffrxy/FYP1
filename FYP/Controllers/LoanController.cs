using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FYP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FYP.Controllers
{
    public class LoanController : Microsoft.AspNetCore.Mvc.Controller
    {
        [Authorize(Roles = "Admin, Store Supervisor")]
        public IActionResult Index()
        {
            List<Exercise> dt = DBUtl.GetList<Exercise>(@"SELECT Exercise_id, E.Package_id, U.nric, E.company, 
                                            E.unit, P.Name, E.start_date, E.end_date, 
                                            E.description, E.status
                                            FROM Exercise E 
                                            INNER JOIN Users U ON E.nric = U.nric 
                                            INNER JOIN Package P ON E.Package_id = P.Package_id
                                            WHERE E.status = 'Returned'");

            return View("Index", dt);
        }
                
        public IActionResult UserView()
        {
            return View();
        }
        public IActionResult SuccessLoan()
        {
            return View();
        }
        public IActionResult ReturnView()
        {
            return View();
        }
        public IActionResult SuccessReturn()
        {
            return View();
        }
        private void updatearchive()
        {
            var list = DBUtl.GetList<Exercise>("SELECT * FROM Exercise");
            DateTime currentdate = DateTime.Now;

            foreach (var a in list)
            {
                DateTime enddate = a.end_date;
                if (enddate < currentdate)
                {
                    var update = "UPDATE Exercise SET archive = '{0}' WHERE Exercise_id = '{1}'";
                    DBUtl.ExecSQL(update, true, a.Exercise_id);

                }
            }
        }

        [Authorize(Roles = "Admin, Store Supervisor")]
        public IActionResult Loan()
        {
            updateStatus();
            updatearchive();
            var loanList = DBUtl.GetList<Exercise>(@"SELECT * FROM Exercise E 
                                                   INNER JOIN Users U ON E.nric = U.nric 
                                                   INNER JOIN Package P ON E.Package_id = P.Package_id
                                                   WHERE E.archive = 0
                                                   AND status != 'Returned'");

            return View("Loan", loanList);
        }

        public void updateStatus()
        {
            var loanList = DBUtl.GetList<Exercise>(@"SELECT * FROM Exercise E 
                                                   INNER JOIN Users U ON E.nric = U.nric 
                                                   INNER JOIN Package P ON E.Package_id = P.Package_id
                                                   WHERE E.archive = 0
                                                   AND status != 'Returned'");
       
            foreach (var a in loanList)
            {
                if (a.assigned_status == false)
                {
                    string newStatus = Status(a);
                    string sqlstatement = "UPDATE Exercise SET status = '{0}', assigned_status = '{1}' WHERE Exercise_id = '{2}'";
                    var abc = DBUtl.ExecSQL(sqlstatement, newStatus, true, a.Exercise_id);
                }
            }

        }

        public string Status(Exercise newex)
        {
            var user = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '" + newex.company + "' AND unit = '" + newex.unit + "'");

            int userCount = user.Count;
            int pack = newex.Package_id;

            int counter = packAvail(pack, userCount);
            if (userCount > counter)
            {
                return "Insufficient Equipment Quantity";
            }
            else
            {

                return "Ready to Loan";
            }
        }

        public int packAvail(int packid, int users)
        {
            int entries = 0;

            if (packid.Equals(1))
            {
                var pack1 = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SAR-21' AND Status = 'Available' AND Assigned = 1");
                var pack1two = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SAR-21' AND Status = 'Available' AND Assigned = 0");

                entries = pack1.Count;
                int availcounter = pack1two.Count;


                if (entries == 0)
                {
                    if (availcounter == 0)
                    {
                        entries = 0;
                    }
                    else
                    {
                        int x = 0;
                        while (x <= users - 1)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                SET Assigned = '{0}'
                                                WHERE Serial_no = '{1}'", true, pack1two[x].Serial_no);
                            x++;
                        }
                        entries = availcounter;
                    }
                }

            }
            else if (packid.Equals(2))
            {
                var pack2 = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SIG Sauer P226' AND Status = 'Available' AND Assigned = 1");
                var pack2two = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SIG Sauer P226' AND Status = 'Available' AND Assigned = 0");

                entries = pack2.Count;
                int availcounter = pack2two.Count;

                if (entries == 0)
                {
                    if (availcounter == 0)
                    {
                        entries = 0;
                    }
                    else
                    {
                        int x = 0;
                        while (x <= users - 1)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                SET Assigned = '{0}'
                                                WHERE Serial_no = '{1}'", true, pack2two[x].Serial_no);
                            x++;
                        }
                        entries = availcounter;
                    }
                }

            }
            else if (packid.Equals(3))
            {
                var pack3 = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SAR-21' AND Status = 'Available' AND Assigned = 1");
                var pack3two = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SAR-21' AND Status = 'Available' AND Assigned = 0");

                entries = pack3.Count;
                int availcounter = pack3two.Count;

                if (entries == 0)
                {
                    if (availcounter == 0)
                    {
                        entries = 0;
                    }
                    else
                    {
                        int x = 0;
                        while (x <= users - 1)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                SET Assigned = '{0}'
                                                WHERE Serial_no = '{1}'", true, pack3two[x].Serial_no);
                            x++;
                        }
                        entries = availcounter;
                    }
                }

            }
            else if (packid.Equals(4))
            {
                var pack4 = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'AK-47' AND Status = 'Available' AND Assigned = 1");
                var pack4two = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'AK-47' AND Status = 'Available' AND Assigned = 0");

                entries = pack4.Count;
                int availcounter = pack4two.Count;

                if (entries == 0)
                {
                    if (availcounter == 0) {
                        entries = 0;
                    }
                    else
                    {
                        int x = 0;
                        while (x <= users - 1)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                SET Assigned = '{0}'
                                                WHERE Serial_no = '{1}'", true, pack4two[x].Serial_no);
                            x++;
                        }
                        entries = availcounter;
                    }
                }

            }

            return entries;
        }

        [Authorize(Roles = "Admin, Store Supervisor")]
        public IActionResult LoanProcess()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Store Supervisor")]
        [HttpPost]
        public IActionResult LoanProcess(IFormFile postedFile)
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

                    var exercise = new List<Exercise>();
                    var userr = new List<Users>();
                    using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split(',');

                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            userr.Add(new Users
                            {
                                User_id = Int32.Parse(rows[0].ToString()),
                                Serial_no = rows[1].ToString(),
                                nric = rows[2].ToString(),
                                password = rows[3].ToString(),
                                full_name = rows[4].ToString(),
                                dob = DateTime.Parse(rows[5].ToString()),
                                rank = rows[6].ToString(),
                                unit = rows[7].ToString(),
                                company = rows[8].ToString(),
                                role = rows[9].ToString()
                            }); ;

                            exercise.Add(new Exercise
                            {
                                Exercise_id = Int32.Parse(rows[10].ToString()),
                                nric = rows[11].ToString(),
                                company = rows[8].ToString(),
                                unit = rows[7].ToString(),
                                description = rows[12].ToString(),
                                start_date = DateTime.Parse(rows[13].ToString()),
                                end_date = DateTime.Parse(rows[14].ToString()),
                                archive = Boolean.Parse(rows[15]),
                                Package_id = Int32.Parse(rows[16].ToString())
                            }); ;
                                                        
                        }

                    }

                    var userList = DBUtl.GetList<Users>("SELECT * FROM Users WHERE User_id = " + userr[0].User_id + "");
                    var exerciseList = DBUtl.GetList<Exercise>("SELECT * FROM Exercise WHERE Exercise_id = " + exercise[0].Exercise_id + "");
                    var roleList = DBUtl.GetList<Users>("SELECT * FROM Users WHERE nric = '" + exerciseList[0].nric + "'");
                    var packageList = DBUtl.GetList<Package>("SELECT * FROM Package WHERE Package_id = " + exercise[0].Package_id + "");
                    var equipmentList = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = '" + packageList[0].Type_desc + "'");
                    var accessoryList = DBUtl.GetList<Equipment_Accessories>("SELECT * FROM Equipment_accessories WHERE Equipment_accessories_id = " + packageList[0].Equipment_accessories_id + "");

                    var userListCheck = DBUtl.GetList<Users>("SELECT * FROM Users");
                    int usersNo = userList.Count;
                    int quantPerPack = 0;

                    if (packageList[0].Equipment_accessories_id.Equals(1))
                    {
                        quantPerPack += 1;
                    }
                    else if (packageList[0].Equipment_accessories_id.Equals(2))
                    {
                        quantPerPack += 5;
                    }
                    else if (packageList[0].Equipment_accessories_id.Equals(3))
                    {
                        quantPerPack += 1;
                    }

                    int totalAcc = quantPerPack * usersNo;
                    if (accessoryList[0].Quantity > totalAcc)
                    {
                        string accLoan = "UPDATE Equipment_accessories SET Quantity = (Quantity - {0}) WHERE Equipment_accessories_id = {1}";
                        int accUpdate = DBUtl.ExecSQL(accLoan, totalAcc, packageList[0].Equipment_accessories_id);
                    }

                    if (packageList[0].Type_desc == "SAR-21")
                    {
                        var packList = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SAR-21' AND Status = 'Available' AND Assigned = '{0}'", true);

                        foreach (var a in userList)
                        {
                            foreach (var b in packList)
                            {
                                if (b.Serial_no == a.Serial_no)
                                {
                                    if (roleList[0].role == "Platoon Commander")
                                    {
                                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Deployed' WHERE User_id = '{0}' AND deployed_status = 'Standby' AND role = 'Serviceman'", userr[0].User_id);
                                        int statChange = DBUtl.ExecSQL("UPDATE Equipment SET Status = 'Unavailable' WHERE Assigned = '{0}' AND Status = 'Available' AND Serial_no = '{1}'", true, b.Serial_no);

                                        var deployCheck = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '{0}' AND unit = '{1}' AND deployed_status = 'Standby' AND role = 'Serviceman'", exercise[0].company, exercise[0].unit);
                                        if (deployCheck.Count == 0)
                                        {
                                            int exLoaned = DBUtl.ExecSQL("UPDATE Exercise SET status = 'Loaned Out' WHERE Exercise_id = '{0}'", exercise[0].Exercise_id);
                                        }
                                    }
                                    else if (roleList[0].role == "Officer Commander")
                                    {
                                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Deployed' WHERE User_id = '{0}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander')", userr[0].User_id);
                                        int statChange = DBUtl.ExecSQL("UPDATE Equipment SET Status = 'Unavailable' WHERE Assigned = '{0}' AND Status = 'Available' AND Serial_no = '{1}'", true, b.Serial_no);

                                        var deployCheck = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '{0}' AND unit = '{1}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander')", exercise[0].company, exercise[0].unit);
                                        if (deployCheck.Count == 0)
                                        {
                                            int exLoaned = DBUtl.ExecSQL("UPDATE Exercise SET status = 'Loaned Out' WHERE Exercise_id = '{0}'", exercise[0].Exercise_id);
                                        }
                                    }
                                    else if (roleList[0].role == "Commandant Officer")
                                    {
                                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Deployed' WHERE User_id = '{0}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander' OR role = 'Officer Commander')", userr[0].User_id);
                                        int statChange = DBUtl.ExecSQL("UPDATE Equipment SET Status = 'Unavailable' WHERE Assigned = '{0}' AND Status = 'Available' AND Serial_no = '{1}'", true, b.Serial_no);


                                        var deployCheck = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '{0}' AND unit = '{1}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander' OR role = 'Officer Commander')", exercise[0].company, exercise[0].unit);
                                        if (deployCheck.Count == 0)
                                        {
                                            int exLoaned = DBUtl.ExecSQL("UPDATE Exercise SET status = 'Loaned Out' WHERE Exercise_id = '{0}'", exercise[0].Exercise_id);
                                        }
                                    }
                                    
                                }
                            }
                        }

                    }

                    else if (packageList[0].Type_desc == "AK-47")
                    {
                        var packList = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'AK-47' AND Status = 'Available' AND Assigned = '{0}'", true);
                        
                        foreach (var a in userList)
                        {
                            foreach (var b in packList)
                            {
                                if (b.Serial_no == a.Serial_no)
                                {
                                    if (roleList[0].role == "Platoon Commander")
                                    {
                                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Deployed' WHERE User_id = '{0}' AND deployed_status = 'Standby' AND role = 'Serviceman'", userr[0].User_id);
                                        int statChange = DBUtl.ExecSQL("UPDATE Equipment SET Status = 'Unavailable' WHERE Assigned = '{0}' AND Status = 'Available' AND Serial_no = '{1}'", true, b.Serial_no);

                                        var deployCheck = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '{0}' AND unit = '{1}' AND deployed_status = 'Standby' AND role = 'Serviceman'", exercise[0].company, exercise[0].unit);
                                        if (deployCheck.Count == 0)
                                        {
                                            int exLoaned = DBUtl.ExecSQL("UPDATE Exercise SET status = 'Loaned Out' WHERE Exercise_id = '{0}'", exercise[0].Exercise_id);
                                        }
                                    }
                                    else if (roleList[0].role == "Officer Commander")
                                    {
                                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Deployed' WHERE User_id = '{0}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander')", userr[0].User_id);
                                        int statChange = DBUtl.ExecSQL("UPDATE Equipment SET Status = 'Unavailable' WHERE Assigned = '{0}' AND Status = 'Available' AND Serial_no = '{1}'", true, b.Serial_no);

                                        var deployCheck = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '{0}' AND unit = '{1}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander')", exercise[0].company, exercise[0].unit);
                                        if (deployCheck.Count == 0)
                                        {
                                            int exLoaned = DBUtl.ExecSQL("UPDATE Exercise SET status = 'Loaned Out' WHERE Exercise_id = '{0}'", exercise[0].Exercise_id);
                                        }
                                    }
                                    else if (roleList[0].role == "Commandant Officer")
                                    {
                                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Deployed' WHERE User_id = '{0}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander' OR role = 'Officer Commander')", userr[0].User_id);
                                        int statChange = DBUtl.ExecSQL("UPDATE Equipment SET Status = 'Unavailable' WHERE Assigned = '{0}' AND Status = 'Available' AND Serial_no = '{1}'", true, b.Serial_no);


                                        var deployCheck = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '{0}' AND unit = '{1}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander' OR role = 'Officer Commander')", exercise[0].company, exercise[0].unit);
                                        if (deployCheck.Count == 0)
                                        {
                                            int exLoaned = DBUtl.ExecSQL("UPDATE Exercise SET status = 'Loaned Out' WHERE Exercise_id = '{0}'", exercise[0].Exercise_id);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (packageList[0].Type_desc == "SIG Sauer P226")
                    {
                        var packList = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SIG Sauer P226' AND Status = 'Available' AND Assigned = '{0}'", true);
                        
                        foreach (var a in userList)
                        {
                            foreach (var b in packList)
                            {
                                if (b.Serial_no == a.Serial_no)
                                {
                                    if (roleList[0].role == "Platoon Commander")
                                    {
                                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Deployed' WHERE User_id = '{0}' AND deployed_status = 'Standby' AND role = 'Serviceman'", userr[0].User_id);
                                        int statChange = DBUtl.ExecSQL("UPDATE Equipment SET Status = 'Unavailable' WHERE Assigned = '{0}' AND Status = 'Available' AND Serial_no = '{1}'", true, b.Serial_no);

                                        var deployCheck = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '{0}' AND unit = '{1}' AND deployed_status = 'Standby' AND role = 'Serviceman'", exercise[0].company, exercise[0].unit);
                                        if (deployCheck.Count == 0)
                                        {
                                            int exLoaned = DBUtl.ExecSQL("UPDATE Exercise SET status = 'Loaned Out' WHERE Exercise_id = '{0}'", exercise[0].Exercise_id);
                                        }
                                    }
                                    else if (roleList[0].role == "Officer Commander")
                                    {
                                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Deployed' WHERE User_id = '{0}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander')", userr[0].User_id);
                                        int statChange = DBUtl.ExecSQL("UPDATE Equipment SET Status = 'Unavailable' WHERE Assigned = '{0}' AND Status = 'Available' AND Serial_no = '{1}'", true, b.Serial_no);

                                        var deployCheck = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '{0}' AND unit = '{1}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander')", exercise[0].company, exercise[0].unit);
                                        if (deployCheck.Count == 0)
                                        {
                                            int exLoaned = DBUtl.ExecSQL("UPDATE Exercise SET status = 'Loaned Out' WHERE Exercise_id = '{0}'", exercise[0].Exercise_id);
                                        }
                                    }
                                    else if (roleList[0].role == "Commandant Officer")
                                    {
                                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Deployed' WHERE User_id = '{0}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander' OR role = 'Officer Commander')", userr[0].User_id);
                                        int statChange = DBUtl.ExecSQL("UPDATE Equipment SET Status = 'Unavailable' WHERE Assigned = '{0}' AND Status = 'Available' AND Serial_no = '{1}'", true, b.Serial_no);


                                        var deployCheck = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '{0}' AND unit = '{1}' AND deployed_status = 'Standby' AND (role = 'Serviceman' OR role = 'Platoon Commander' OR role = 'Officer Commander')", exercise[0].company, exercise[0].unit);
                                        if (deployCheck.Count == 0)
                                        {
                                            int exLoaned = DBUtl.ExecSQL("UPDATE Exercise SET status = 'Loaned Out' WHERE Exercise_id = '{0}'", exercise[0].Exercise_id);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    
                    return RedirectToAction("Loan");

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
        public IActionResult ReturnProcess()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Store Supervisor")]
        [HttpPost]
        public IActionResult ReturnProcess(IFormFile postedFile)
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

                    var exercise = new List<Exercise>();
                    var userr = new List<Users>();
                    using (var sreader = new StreamReader(postedFile.OpenReadStream()))
                    {
                        //First line is header. If header is not passed in csv then we can neglect the below line.
                        string[] headers = sreader.ReadLine().Split(',');

                        //Loop through the records
                        while (!sreader.EndOfStream)
                        {
                            string[] rows = sreader.ReadLine().Split(',');

                            userr.Add(new Users
                            {
                                User_id = Int32.Parse(rows[0].ToString()),
                                Serial_no = rows[1].ToString(),
                                nric = rows[2].ToString(),
                                password = rows[3].ToString(),
                                full_name = rows[4].ToString(),
                                dob = DateTime.Parse(rows[5].ToString()),
                                rank = rows[6].ToString(),
                                unit = rows[7].ToString(),
                                company = rows[8].ToString(),
                                role = rows[9].ToString()
                            }); ;

                            exercise.Add(new Exercise
                            {
                                Exercise_id = Int32.Parse(rows[10].ToString()),
                                nric = rows[11].ToString(),
                                company = rows[8].ToString(),
                                unit = rows[7].ToString(),
                                description = rows[12].ToString(),
                                start_date = DateTime.Parse(rows[13].ToString()),
                                end_date = DateTime.Parse(rows[14].ToString()),
                                archive = Boolean.Parse(rows[15]),
                                Package_id = Int32.Parse(rows[16].ToString())
                            }); ;


                        }

                    }

                    var userList = DBUtl.GetList<Users>("SELECT * FROM Users WHERE User_id = " + userr[0].User_id + "");
                    var exerciseList = DBUtl.GetList<Exercise>("SELECT * FROM Exercise WHERE Exercise_id = " + exercise[0].Exercise_id + "");
                    var roleList = DBUtl.GetList<Users>("SELECT * FROM Users WHERE nric = '" + exerciseList[0].nric + "'");
                    var packageList = DBUtl.GetList<Package>("SELECT * FROM Package WHERE Package_id = " + exercise[0].Package_id + "");
                    var equipmentList = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = '" + packageList[0].Type_desc + "'");
                    var accessoryList = DBUtl.GetList<Equipment_Accessories>("SELECT * FROM Equipment_accessories WHERE Equipment_accessories_id = " + packageList[0].Equipment_accessories_id + "");

                    var userListCheck = DBUtl.GetList<Users>("SELECT * FROM Users");
                    int usersNo = userList.Count;
                    int quantPerPack = 0;

                    if (packageList[0].Equipment_accessories_id.Equals(1))
                    {
                        quantPerPack += 1;
                    }
                    else if (packageList[0].Equipment_accessories_id.Equals(2))
                    {
                        quantPerPack += 5;
                    }
                    else if (packageList[0].Equipment_accessories_id.Equals(3))
                    {
                        quantPerPack += 1;
                    }

                    int totalAcc = quantPerPack * usersNo;
                    if (accessoryList[0].Quantity > totalAcc)
                    {
                        string accLoan = "UPDATE Equipment_accessories SET Quantity = (Quantity + {0}) WHERE Equipment_accessories_id = {1}";
                        int accUpdate = DBUtl.ExecSQL(accLoan, totalAcc, packageList[0].Equipment_accessories_id);
                    }

                    if (packageList[0].Type_desc == "SAR-21")
                    {
                        var packList = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SAR-21' AND Status = 'Unavailable' AND Assigned = '{0}'", true);
                        int loops = packList.Count;

                        int x = 0;
                        while (x < loops)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                SET Status = 'Available'
                                                WHERE Assigned = '{0}' AND Status = 'Unavailable' AND Serial_no = '{1}'", true, packList[x].Serial_no);
                            x++;
                        }
                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Standby' WHERE User_id = '{0}' AND deployed_status = 'Deployed'", userr[0].User_id);

                    }

                    else if (packageList[0].Type_desc == "AK-47")
                    {
                        var packList = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'AK-47' AND Status = 'Unavailable' AND Assigned = '{0}'", true);
                        int loops = packList.Count;
                        int x = 0;
                        while (x < loops)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                SET Status = 'Available'
                                                WHERE Assigned = '{0}' AND Status = 'Unavailable' AND Serial_no = '{1}'", true, packList[x].Serial_no);
                            x++;
                        }
                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Standby' WHERE User_id = '{0}' AND deployed_status = 'Deployed'", userr[0].User_id);

                    }
                    else if (packageList[0].Type_desc == "SIG Sauer P226")
                    {
                        var packList = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SIG Sauer P226' AND Status = 'Unavailable' AND Assigned = '{0}'", true);
                        int loops = packList.Count;
                        int x = 0;
                        while (x < loops)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                SET Status = 'Available', Assigned = '{0}'
                                                WHERE Assigned = '{0}' AND Status = 'Unavailable' AND Serial_no = '{1}'", true, packList[x].Serial_no);
                            x++;
                        }
                        int deploying = DBUtl.ExecSQL("UPDATE Users SET deployed_status = 'Standby' WHERE User_id = '{0}' AND deployed_status = 'Deployed'", userr[0].User_id);

                    }

                    var deployCheck = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '{0}' AND unit = '{1}' AND deployed_status = 'Deployed'", exercise[0].company, exercise[0].unit);

                    if (deployCheck.Count == 0)
                    {
                        int exLoaned = DBUtl.ExecSQL("UPDATE Exercise SET status = 'Returned' WHERE Exercise_id = '{0}'", exercise[0].Exercise_id);
                    }



                    return RedirectToAction("Loan");

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


        private const string LOGIN_SQL =
        @"SELECT * FROM Users 
            WHERE nric = '{0}' 
              AND password =  HASHBYTES('SHA1', '{1}')";

        private const string ROLE_COL = "role";
        private const string NAME_COL = "nric";

        private const string REDIRECT_CNTR = "Loan";
        private const string REDIRECT_ACTN = "UserView";
        private const string LOGIN_VIEW = "UserAuthen";
        public IActionResult UserAuthen(string returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View(LOGIN_VIEW);
        }

        [HttpPost]
        public IActionResult UserAuthen(UserLogin u)
        {
            if (!AuthenticateUser(u.nric, u.password, out ClaimsPrincipal principal))
            {
                ViewData["Message"] = "Incorrect NRIC or Password";
                ViewData["MsgType"] = "warning";
                return View(LOGIN_VIEW);
            }
            else

            {
                    if (TempData["returnUrl"] != null)
                    {

                        string returnUrl = TempData["returnUrl"].ToString();
                        if (Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                    }

                    return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
                
            }
        }


        private const string REDIRECT_CNTRR = "Loan";
        private const string REDIRECT_ACTNR = "ReturnView";
        private const string LOGIN_VIEWR = "UserAuthenR";
        public IActionResult UserAuthenR(string returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View(LOGIN_VIEWR);
        }

        [HttpPost]
        public IActionResult UserAuthenR(UserLogin u)
        {
            if (!AuthenticateUser(u.nric, u.password, out ClaimsPrincipal principal))
            {
                ViewData["Message"] = "Incorrect NRIC or Password";
                ViewData["MsgType"] = "warning";
                return View(LOGIN_VIEWR);
            }
            else

            {
                if (TempData["returnUrl"] != null)
                {

                    string returnUrl = TempData["returnUrl"].ToString();
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                }

                return RedirectToAction(REDIRECT_ACTNR, REDIRECT_CNTRR);

            }
        }

        private bool AuthenticateUser(string uid, string pw, out ClaimsPrincipal principal)
        {
            principal = null;

            DataTable ds = DBUtl.GetTable(LOGIN_SQL, uid, pw);
            if (ds.Rows.Count == 1)
            {
                principal =
                   new ClaimsPrincipal(
                      new ClaimsIdentity(
                         new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, uid),
                        new Claim(ClaimTypes.Name, ds.Rows[0][NAME_COL].ToString()),
                        new Claim(ClaimTypes.Role, ds.Rows[0][ROLE_COL].ToString())
                         }, "Basic"
                      )
                   );
                if (principal != null)
                {
                    bool d = true;
                    return d;
                }
                return false;
            }
            return false;
        }

        [Authorize(Roles = "Admin, Store Supervisor")]
        public IActionResult ViewPackage(int id)
        {
            var packageList = DBUtl.GetList<Package>("SELECT * FROM Package WHERE Package_id = '" + id + "'");
            return View("ViewPackage", packageList);
        }

        [Authorize(Roles = "Admin, Store Supervisor")]
        public IActionResult ViewAccessory(int id)
        {
            var accessoryList = DBUtl.GetList<Equipment_Accessories>("SELECT * FROM Equipment_accessories WHERE Equipment_accessories_id = '" + id + "'");
            return View("ViewAccessory", accessoryList);
        }
    }
}
