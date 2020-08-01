using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Announcement
    {
        public int Announcement_id { get; set; }

        public string Announcement_desc { get; set; }

        [Required(ErrorMessage = "Enter a valid date")]
        [DataType(DataType.DateTime)]
        public DateTime Start_date { get; set; }

        [Required(ErrorMessage = "Enter a valid date")]
        [DataType(DataType.DateTime)]
        public DateTime End_date { get; set; }


    }
}
