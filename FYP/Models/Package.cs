using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Package
    {

        public string Package_id { get; set; }

        public int Serial_no { get; set; }

        public int Equipment_accessories_id { get; set; }

        public string Package_details { get; set; }
    

        public string Name { get; set; }


      

     
    }
}
