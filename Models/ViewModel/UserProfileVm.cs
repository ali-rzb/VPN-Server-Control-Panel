using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModel
{
    public class UserProfileVm : Users.User
    {
        [MinLength(2)]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [MinLength(2)]
        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        public string confirm_password { get; set; }
        public string GoogleRecaptchaToken { get; set; }

    }
}
