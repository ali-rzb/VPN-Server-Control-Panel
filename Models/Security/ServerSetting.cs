using Models.Firewall;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable()]
    public class ServerSetting
    {
        [DisplayName("Clients Can Change Pass")]
        public bool ClientsCanChangePass { get; set; }
        [DisplayName("Open RDP Firewall Port")]
        public bool RdpRuleStatus { get; set; }
        [DisplayName("RDP Service Working")]
        public bool RdpServiceStatus { get; set; }
        public bool RdpServiceStatusPending { get; set; }
        public string IpAddress { get; set; }

        public ServerSetting()
        {
            this.ClientsCanChangePass = false;
            this.RdpRuleStatus = true;
            this.RdpServiceStatusPending = false;
            this.RdpServiceStatus = true;
            this.RdpServiceStatusPending = false;
            this.IpAddress = "";
        }
    }
}
