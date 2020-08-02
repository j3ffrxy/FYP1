using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Maintenance
    {
        public int Maintenance_id { get; set; }

        public string Serial_no { get; set; }

        [Required(ErrorMessage = "Enter a valid date")]
        [DataType(DataType.DateTime)]
        public DateTime Start_date { get; set; }

        [Required(ErrorMessage = "Enter a valid date")]
        [DataType(DataType.DateTime)]
        public DateTime End_date { get; set; }

        public string maint_type { get; set; }

        public string description { get; set; }

        public bool archive { get; set; }
    }
}
