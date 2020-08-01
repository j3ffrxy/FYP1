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

        public int Package_id { get; set; }

        public string Type_desc { get; set; }

        public int Equipment_accessories_id { get; set; }    

        public string Name { get; set; }


      

     
    }
}
