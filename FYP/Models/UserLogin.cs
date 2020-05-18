using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter User ID")]
        public string User_id { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}