using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Stocktaking
    {
        public int Stocktaking_id { get; set; }

        public int User_id { get; set; }

        public int total_equipment_quantity { get; set; }

        public int total_accessories_quantity { get; set; }

        public DateTime date_created { get; set; }

        public String comments { get; set; }

        public bool archive { get; set; }
    }
}
