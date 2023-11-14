using Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Users
{
    public class ConnectionInfo : ModelObject
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string Port { get; set; }
        public DateTime ConnectionStartDate { get; set; }
        public DateTime? ConnectionEndDate { get; set; }
        public TimeSpan ConnectionDuration { get; set; }
        public InfoTransferUnit Upload { get; set; }
        public InfoTransferUnit Download { get; set; }
        public string Ip { get; set; }
    }
}
