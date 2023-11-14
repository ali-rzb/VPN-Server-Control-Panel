using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModel
{
    public class FailedLoginsVm : ModelObject
    {
        public List<int> stats { get; set; }
        public List<DateTime> dates { get; set; }
    }
}
