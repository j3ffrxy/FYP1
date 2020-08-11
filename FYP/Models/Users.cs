using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class Users
    {
        public int User_id { get; set; }

        public String Serial_no { get; set; }

        [Required(ErrorMessage = "Enter Users NRIC")]
        [RegularExpression("[STFG]\\d{7}[A-Z]", ErrorMessage = "Invalid NRIC format")]
        public String nric { get; set; }

        [Required(ErrorMessage = "Enter Users password")]
        [DataType(DataType.Password)]
        public String password { get; set; }

        [Required(ErrorMessage = "Enter Users name")]
        [StringLength(40, ErrorMessage = "Max 40 chars")]
        public String full_name { get; set; }

        [DataType(DataType.DateTime)]
        [Remote(action: "VerifyDate", controller: "User")]
        public DateTime dob { get; set; }

        [Required(ErrorMessage = "Select a rank")]
        public String rank { get; set; }

        [Required(ErrorMessage = "Select a unit")]
        public String unit { get; set; }

        public String company { get; set; }

        [Required(ErrorMessage = "Select a role")]
        public String role { get; set; }

        public String Person_Id {get; set;}

        public String Maintenance_status { get; set; }

        public String deployed_status {get; set;}

        public DateTime loa_start_date { get; set; }

        public DateTime loa_end_date { get; set; }


    }
}
