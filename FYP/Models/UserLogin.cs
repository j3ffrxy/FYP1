using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace FYP.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter NRIC")]
        public string nric { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        public string password { get; set; }

        public bool RememberMe { get; set; }
    }
}