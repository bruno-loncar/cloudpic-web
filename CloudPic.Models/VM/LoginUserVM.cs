using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CloudPic.Models.VM
{
    public class LoginUserVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }

        public string Serialize => $"email={Email}&password={Password}";
    }
}
