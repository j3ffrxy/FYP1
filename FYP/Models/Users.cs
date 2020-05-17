using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Users
    {
        public int User_id { get; set; }

        public int Group_id { get; set; }

        public string full_name { get; set; }

        public DateTime dob { get; set; }
        
    }
}
