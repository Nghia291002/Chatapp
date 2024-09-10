using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Chatapp.ViewModel
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username cannot be blank. ")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Username cannot be blank. ")]
        public string Password { get; set; }

    }
}