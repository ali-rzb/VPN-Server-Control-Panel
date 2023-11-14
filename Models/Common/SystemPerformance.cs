using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    [Serializable()]
    public class SystemPerformance
    {
        public float NetSpeed { get; set; }
        public float NetUsageSent { get; set; }
        public float NetUsageRecieved { get; set; }

        public float RamTotal { get; set; }
        public float RamUsed { get; set; }

        public float CpuUsage { get; set; }
    }
}
