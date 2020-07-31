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
    public class LoanController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IActionResult Index()
        {
            
            DataTable dt = DBUtl.GetTable(@"SELECT Exercise_id, E.Package_id, U.nric AS [SAF11B], E.company AS [Company], 
                                            E.unit AS [Unit], P.Name AS [Weapon Package], E.start_date AS [Start Date], E.end_date AS [End Date], 
                                            E.description AS [Description]
                                            FROM Exercise E 
                                            INNER JOIN Users U ON E.nric = U.nric 
                                            INNER JOIN Package P ON E.Package_id = P.Package_id
                                            WHERE E.archive = 0");

            var dtList = DBUtl.GetList<Exercise>(@"SELECT Exercise_id, E.Package_id, U.nric AS [SAF11B], E.company AS [Company], 
                                                   E.unit AS [Unit], P.Name AS [Weapon Package], E.start_date AS [Start Date], E.end_date AS [End Date], 
                                                   E.description AS [Description]
                                                   FROM Exercise E 
                                                   INNER JOIN Users U ON E.nric = U.nric 
                                                   INNER JOIN Package P ON E.Package_id = P.Package_id
                                                   WHERE E.archive = 0");
            foreach (var a in dtList)
            {
                a.status = Status();
            }
            return View("Index", dt.Rows);
        }

        public string Status()
        {
            string defaultStatus = "Pending";
            var exercises = DBUtl.GetList<Exercise>(@"SELECT Exercise_id, E.Package_id, U.nric AS [SAF11B], E.company AS [Company], 
                                                      E.unit AS [Unit], P.Name AS [Weapon Package], E.start_date AS [Start Date], E.end_date AS [End Date], 
                                                      E.description AS [Description]
                                                      FROM Exercise E 
                                                      INNER JOIN Users U ON E.nric = U.nric 
                                                      INNER JOIN Package P ON E.Package_id = P.Package_id
                                                      WHERE E.archive = 0");
            foreach (var a in exercises)
            {
                var user = DBUtl.GetList<Users>("SELECT * FROM Users WHERE company = '"+ a.company +"' AND unit = '"+ a.unit +"'");

                int count = user.Count;
                int pack = a.Package_id;
                if (pack == 1)
                {
                    var pack1 = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SAR-21' AND Status = 'Available' AND Assigned = False");
                    int eqCount = pack1.Count;
                    if (count > eqCount)
                    {
                        defaultStatus = "Insufficient Equipment Quantity";
                    }
                    else
                    {                       
                        for (int x = 0; x < count - 1; x++)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                           SET Status = 'Unavailable', Assigned = True
                                                           WHERE Serial_no = '{0}'", pack1[x].Serial_no);
                            defaultStatus = "Loaned Out";
                        }
                    }
                }
                else if (pack == 2)
                {
                    var pack2 = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SIG Sauer P226' AND Status = 'Available' AND Assigned = False");
                    int eqCount = pack2.Count;
                    if (count > eqCount)
                    {
                        defaultStatus = "Insufficient Equipment Quantity";
                    }
                    else
                    {
                        for (int x = 0; x < count - 1; x++)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                           SET Status = 'Unavailable', Assigned = True
                                                           WHERE Serial_no = '{0}'", pack2[x].Serial_no);
                            defaultStatus = "Loaned Out";
                        }
                    }
                }
                else if (pack == 3)
                {
                    var pack3 = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'SAR-21' AND Status = 'Available' AND Assigned = False");
                    int eqCount = pack3.Count;
                    if (count > eqCount)
                    {
                        defaultStatus = "Insufficient Equipment Quantity";
                    }
                    else
                    {
                        for (int x = 0; x < count - 1; x++)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                           SET Status = 'Unavailable', Assigned = True
                                                           WHERE Serial_no = '{0}'", pack3[x].Serial_no);
                            defaultStatus = "Loaned Out";
                        }
                    }
                }
                else if (pack == 4)
                {
                    var pack4 = DBUtl.GetList<Equipment>("SELECT * FROM Equipment WHERE Type_desc = 'AK-47' AND Status = 'Available' AND Assigned = False");
                    int eqCount = pack4.Count;
                    if (count > eqCount)
                    {
                        defaultStatus = "Insufficient Equipment Quantity";
                    }
                    else
                    {
                        for (int x = 0; x < count - 1; x++)
                        {
                            var updateEq = DBUtl.ExecSQL(@"UPDATE Equipment
                                                           SET Status = 'Unavailable', Assigned = True
                                                           WHERE Serial_no = '{0}'", pack4[x].Serial_no);
                            defaultStatus = "Loaned Out";
                        }
                    }
                }

            }

            return defaultStatus;
        }

    }
}
