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


        [Required(ErrorMessage = "Enter a valid  weapon type")]

        public int Type_id { get; set; }

        [Required(ErrorMessage = "Enter storage location")]
        public string Storage_location { get; set; }
    

        [Required(ErrorMessage = "Enter quantity")]
        public int Quantity { get; set; }

        public string Type_desc { get; set; }
      

     
    }
}
