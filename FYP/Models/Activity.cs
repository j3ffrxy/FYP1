using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Activity
    {
        public int Activity_id { get; set; }

        [Required(ErrorMessage = "Specify a valid platoon")]
        public int Platoon_id { get; set; }

        [Required(ErrorMessage = "Specify a valid company")]
        public int Company_id { get; set; }

        [Required(ErrorMessage = "Specify a valid brigade")]
        public int Brigade_id { get; set; }

        [Required(ErrorMessage = "Enter a valid activity type")]
        public char type { get; set; }

        [Required(ErrorMessage = "Enter a valid description")]
        public string activity_description { get; set; }

        [Required(ErrorMessage = "Enter a date")]
        [DataType(DataType.DateTime)]
        public DateTime activity_date { get; set; }
    }
}
