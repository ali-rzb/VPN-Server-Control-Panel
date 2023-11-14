using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class EventBl
    {
        Credentials cred = new Credentials();
        public void CreateLog(string path, long eventId, int catId, EventLogEntryType eventMode, string[] data)
        {
            EventLog.WriteEvent(path, new EventInstance(eventId, catId, eventMode), data);
        }
        
        public virtual IEnumerable<EventRecord> ReadLogs(string path, int[] eventId = null)
        {
            try
            {
                EventLogSession session = new EventLogSession(cred.PublicAddress);
                string id_list = "";
                for (int i = 0; i < eventId.Length; i++)
                {
                    if(i != 0)
                    {
                        id_list += " or ";
                    }
                    id_list += "EventID=" + eventId[i];
                }
                string query = String.Format("*[{0}[({1})]]", path, id_list);
                EventLogQuery evntquery = new EventLogQuery(path, PathType.LogName, query) { Session = session };
                List<EventRecord> results = new List<EventRecord>();
                EventLogReader reader = new EventLogReader(evntquery);
                EventRecord eventInstance = reader.ReadEvent();
                for (; null != eventInstance; eventInstance = reader.ReadEvent())
                {
                    results.Add(eventInstance);
                }
                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
