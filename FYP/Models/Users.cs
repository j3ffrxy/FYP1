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

        public int Group_id { get; set; }

        [Required(ErrorMessage = "PLease select a brigade")]
        public int Brigade_id { get; set; }

        [Required(ErrorMessage = "Please select a company ")]

        public int Company_id { get; set; }

        public int Platoon_id { get; set; }

        public int Equipment_id { get; set; }

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

        

    }
}
