using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Equipment_accessories
    {

        public int Equipment_accessories_id { get; set; }

        public string Accessories_details { get; set; }
        [Required(ErrorMessage = "Enter storage location")]

        public string Storage_location { get; set; }

        [Required(ErrorMessage = "Enter quantity")]
        public int Quantity { get; set; }


      

     
    }
}
