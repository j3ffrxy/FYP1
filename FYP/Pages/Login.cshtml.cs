using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace FYP.Pages
{
    public class Login : PageModel
    {
        private readonly ILogger<Login> _logger;

        public Login(ILogger<Login> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
        [Required(ErrorMessage ="please enter User ID")]
        public string UserID { get; set; }

        [Required(ErrorMessage ="please enter password")]
        public string Password { get; set; }
    }
}
