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

        public string Serial_no { get; set; }

        [Required(ErrorMessage = "Enter your NRIC")]
        [RegularExpression("[STFG]\\d{7}[A-Z]", ErrorMessage = "Invalid NRIC format")]
        public string nric { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required(ErrorMessage = "Enter your name")]
        public string full_name { get; set; }

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





    }
}
