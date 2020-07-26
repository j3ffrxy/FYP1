using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Stocktaking_Equipment
    {
        public int Stocktaking_Equipment_id { get; set; }

        public int Stocktaking_id { get; set; }

        public string Serial_no { get; set; }
        
        public string Equipment_name { get; set; }

        public string Storage_location { get; set; }

        public string Type_desc { get; set; }

        public string Status { get; set; }

        public string Assigned { get; set; }

        public bool Matching { get; set; }
    }
}
