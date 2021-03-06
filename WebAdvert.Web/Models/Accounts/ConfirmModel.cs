using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAdvert.Web.Models.Accounts
{
    public class ConfirmModel
    {
        [Required(ErrorMessage = "The email is required.")]
        [EmailAddress]
        [Display(Name = "EMail")]
        public string Email { get; set; }
        [Required(ErrorMessage ="The code is required.")]
        public string Code { get; set; }
    }
}
