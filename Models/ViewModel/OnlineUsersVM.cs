using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Common;

namespace Models.ViewModel
{
    public class OnlineUsersVM : ModelObject
    {
        public string UserName { get; set; }
        public string ConnectionDuration { get; set; }
        public string Connection_Download { get; set; }
        public string Connection_Upload { get; set; }
        public string Ip { get; set; }

    }
}
