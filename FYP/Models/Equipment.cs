﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Equipment
    {
        public string Serial_no { get; set; }

        public string Equipment_name { get; set; }
  
       
      
        public int type_id { get; set; }

        [Required(ErrorMessage = "Enter storage location")]
        public string Storage_location { get; set; }
    


        [Required(ErrorMessage = "Enter quantity")]
        public int Quantity { get; set; }


      

     
    }
}
