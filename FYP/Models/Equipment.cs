using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FYP.Models
{
    public class equipment
    {
        public string Equipment_id { get; set; }

        public string Equipment_name { get; set; }
  
       
      
        public string Serial_id { get; set; }

        [Required(ErrorMessage = "Enter storage location")]
        public string Storage_location { get; set; }
    


        [Required(ErrorMessage = "Enter quantity")]
        public int Quantity { get; set; }


      

     
    }
}
