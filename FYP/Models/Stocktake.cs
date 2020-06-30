using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Stocktake
    {
        public int Stocktake_id { get; set; }

        public int User_id { get; set; }

        public int quantity { get; set; }

        public DateTime date_created { get; set; }

        public String comments { get; set; }

    }
}
