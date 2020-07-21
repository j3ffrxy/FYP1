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
            DataTable dt = DBUtl.GetTable(@"SELECT L.Loan_id, L.Exercise_id, E.nric AS [SAF11B], P.Name AS [Weapon Package], 
                                            E.start_date AS [Start Date], E.end_date AS [End Date]
                                            FROM Loan L 
                                            INNER JOIN Exercise E ON E.Exercise_id = L.Exercise_id 
                                            INNER JOIN Package P ON E.Package_id = P.Package_id
                                            WHERE E.archive = 0");
            return View("Index", dt.Rows);
        }
    }
}
