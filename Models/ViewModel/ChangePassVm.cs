using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModel
{
    public class ChangePassVm
    {
        public string Username { get; set; }
        [MinLength(2)]
        [Required]
        [DisplayName("Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [MinLength(2)]
        [Required]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [MinLength(2)]
        [Required]
        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string GoogleRecaptchaToken { get; set; }
    }
}
