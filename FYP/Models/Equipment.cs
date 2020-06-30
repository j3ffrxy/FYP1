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

        public String storage_location { get; set; }

        public String storage_detail { get; set; }

        public int quantity { get; set; }
    }
}
