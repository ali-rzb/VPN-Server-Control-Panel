using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class ServiceManagement
    {
        public static void ChangeServiceStatus(string ServiceName, bool status, string username = null, SecureString password = null)
        {
            string result = "";
            if (!status)
            {
                result = RunCommand.run(string.Format("sc queryex {0}", ServiceName), username, password);
                if (result.Contains("PID"))
                {
                    String[] spearator = { "PID                :", "FLAGS" };
                    String[] strlist = result.Split(spearator, 10, StringSplitOptions.RemoveEmptyEntries);
                    int pid = int.Parse(strlist[1].Trim());
                    result = RunCommand.run(string.Format("taskkill /pid {0} /f", pid), username, password);
                    result = RunCommand.run(string.Format("net stop {0} /y", ServiceName), username, password);
                }
            }
            else
            {
                result = RunCommand.run(string.Format("net start {0}", ServiceName), username, password);
            }
            if (!result.Contains("successfully"))
            {
                throw new Exception(result);
            }
        }

        public static bool? ServiceStatus(string ServiceName, string username = null, SecureString password = null)
        {
            string result = RunCommand.run("sc query " + ServiceName, username, password);
            if (result.Contains("RUNNING"))
            {
                return true;
            }
            else if (result.Contains("STOPPED"))
            {
                return false;
            }
            else
            {
                return null;
            }
        }

    }
}
