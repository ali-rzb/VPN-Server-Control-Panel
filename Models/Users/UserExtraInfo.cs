using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Models.Users
{
    [Serializable()]
    public class UserExtraInfo
    {
        public string Username { get; set; }
        [Display(Name = "Multilink Capacity")]
        public uint MultilinkCapacity { get; set; }
    }
}
