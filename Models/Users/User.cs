using Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Users
{
    public class User : Person
    {
        public string SID { get; set; }
        [Required]
        public string UserName { get; set; }
        public bool Enabled { get; set; }
        [Display(Name = "Need to change pass")]
        public bool ChangePassNextTime { get; set; }
        public UserExtraInfo ExtraInfo { get; set; }
        [Display(Name = "Reserved Ip")]
        public string ReservedIpAddress { get; set; }
        public bool FullConnection { get; set; }

        public InfoTransferUnit TotalDownload { get; set; }
        public InfoTransferUnit AverageDownloadEachTime { get; set; }
        public InfoTransferUnit TotalUpload { get; set; }
        public InfoTransferUnit AverageUploadEachTime { get; set; }

        public int ConnectionsCount { get; set; }

        public TimeSpan TotalConnectionTime { get; set; }
        public TimeSpan AverageConnectionTime { get; set; }

        public List<ConnectionInfo> ActiveConnections { get; set; }
        public List<ConnectionInfo> Connections { get; set; }

        public List<string> Groups { get; set; }
    }
}
