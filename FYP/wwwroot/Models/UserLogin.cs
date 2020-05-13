﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.wwwroot.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Please enter User ID")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        public string Password { get; set; }
    }
}