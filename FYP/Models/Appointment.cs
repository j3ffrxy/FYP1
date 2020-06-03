using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Appointment
    {
        public int Appt_id { get; set; }

        public int Equipment_id { get; set; }

        [Required(ErrorMessage = "Enter a valid date")]
        [DataType(DataType.DateTime)]
        public DateTime date { get; set; }

        [Required(ErrorMessage = "Enter a valid quantity")]
        public int quantity { get; set; }

        [Required(ErrorMessage = "Enter a description")]
        public string appt_desc { get; set; }

        [Required(ErrorMessage = "Enter an NRIC")]
        [RegularExpression("[STFG]\\d{7}[A-Z]", ErrorMessage = "Invalid NRIC format")]
        public string nric { get; set; }
    }
}
