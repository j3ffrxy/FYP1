using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace FYP.Models
{
    public class Loan
    {
        public int Loan_id { get; set; }
        public int Exercise_id { get; set; }
        public bool archive { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string nric { get; set; }
    }
}
