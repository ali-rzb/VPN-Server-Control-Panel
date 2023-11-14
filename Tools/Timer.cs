using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class Timer
    {
        public DateTime StartTime { get; set; }
        public List<string> TimesString { get; set; }
        public List<TimeSpan> Times { get; set; }
        public Timer()
        {
            TimesString = new List<string>();
            Times = new List<TimeSpan>();
        }
        public void Start()
        {
            StartTime = DateTime.Now;
        }
        public TimeSpan Stop()
        {
            var result = new TimeSpan((DateTime.Now - StartTime).Ticks);
            Times.Add(result);
            TimesString.Add(result.ToFriendlyString());
            return result;
        }
    }
}
