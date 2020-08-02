using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Equipment
    {
        [Required(ErrorMessage = "Specify a valid Serial no")]
        public string Serial_no { get; set; }
        [Required(ErrorMessage = "Enter Equipment name")]

        public string Equipment_name { get; set; }

        [Required(ErrorMessage = "Enter storage location")]
        public string Storage_location { get; set; }
  
        [Required(ErrorMessage = "Enter a valid  weapon type")]
        public string Type_desc { get; set; }

        public string Status { get; set; }
        
        public bool Assigned { get; set; }

        [Required(ErrorMessage = "Enter a valid date")]
        [DataType(DataType.DateTime)]
        public DateTime m_start_date { get; set; }

        [Required(ErrorMessage = "Enter a valid date")]
        [DataType(DataType.DateTime)]
        public DateTime m_end_date { get; set; }



    }
}
