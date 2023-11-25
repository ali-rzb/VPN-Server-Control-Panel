using Models.Common;
using Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModel
{
    public class UserLogVm : ModelObject
    {
        public string SID { get; set; }
        public string UserName { get; set; }
        public bool Enabled { get; set; }
        public bool ChangePassNextTime { get; set; }

        public bool FullConnection { get; set; }

        public InfoTransferUnit Total_Download { get; set; }
        public InfoTransferUnit Total_Upload { get; set; }
        public string Total_Duration { get; set; }
        public int Connections_Count { get; set; }
        public int Active_Connections { get; set; }
        public string Active_Duration { get; set; }
        public DateTime FirstLog { get; set; }

    }
}
