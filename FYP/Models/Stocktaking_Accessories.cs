using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Stocktaking_Accessories
    {
        public int Stocktaking_Accessories_id { get; set; }

        public int Stocktaking_id { get; set; }

        public int Equipment_accessories_id { get; set; }

        public string Accessories_details { get; set; }
       

        public string Storage_location { get; set; }

        public int Quantity { get; set; }

        public bool Matching { get; set; }
    }
}
