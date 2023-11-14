using Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModel
{
    public class LoginPageVm : ModelObject
    {
        public Admin AdminModel { get; set; }
        public bool RememberMe { get; set; }
        public string GoogleRecaptchaToken { get; set; }
    }
}
