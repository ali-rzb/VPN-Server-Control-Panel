using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Firewall
{
    public class FirewallRule : ModelObject
    {
        public bool Enabled { get; set; }
        public string Direction { get; set; }
        public string Profiles { get; set; }
        public string Grouping { get; set; }
        public string LocalIP { get; set; }
        public string RemoteIP { get; set; }
        public string Protocol { get; set; }
        public string LocalPort { get; set; }
        public string RemotePort { get; set; }
        public bool EdgeTraversal { get; set; }
        public string Action { get; set; }
    }
}
