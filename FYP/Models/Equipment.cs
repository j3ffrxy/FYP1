using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Equipment
    {
        public int Equipment_id { get; set; }

        public int Serial_id { get; set; }

        public String Storage_location { get; set; }

        public String Equipment_name { get; set; }

        public int Quantity { get; set; }
    }
}
