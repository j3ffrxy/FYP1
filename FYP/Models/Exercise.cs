using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Exercise
    {
        public int Exercise_id { get; set; }
        public int Package_id { get; set; }
        public string nric { get; set; }

        [Required(ErrorMessage = "Enter a valid company")]
        public string company { get; set; }

        [Required(ErrorMessage = "Enter a valid unit")]
        public string unit { get; set; }

        [Required(ErrorMessage = "Enter a valid description")]
        public string description { get; set; }

        [Required(ErrorMessage = "Enter a valid date")]
        [DataType(DataType.DateTime)]
        public DateTime start_date { get; set; }

        [Required(ErrorMessage = "Enter a valid date")]
        [DataType(DataType.DateTime)]
        public DateTime end_date { get; set; }

        public bool archive { get; set; }
        public string status { get; set; }
        public bool assigned_status { get; set; }
    }
}
