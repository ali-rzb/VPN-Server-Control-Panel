using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Models.Firewall;

namespace Tools
{
    public class FirewallManagement
    {
        public static FirewallRule CheckRule(string rule_name, string username = null, SecureString password = null)
        {
            string command_result = RunCommand.run("netsh advfirewall firewall show rule name=\"" + rule_name + "\"", username, password);
            if (string.IsNullOrEmpty(command_result))
            {
                throw new Exception($"Error Checking Rule Status!, ({command_result})");
            }
            else if (!command_result.Contains("Ok."))
            {
                throw new Exception(command_result);
            }
            string[] seperators = { "Enabled:", "Direction:", "Profiles:", "Grouping:", "LocalIP:", "RemoteIP:", "Protocol:", "LocalPort:", "RemotePort:", "Edge traversal:", "Action:", "Ok." };
            var result = (from s in command_result.Split(seperators, 13, StringSplitOptions.None) select s.Trim()).ToList();
            var res = new FirewallRule()
            {
                Enabled = result[1] == "Yes",
                Direction = result[2],
                Profiles = result[3],
                Grouping = result[4],
                LocalIP = result[5],
                RemoteIP = result[6],
                Protocol = result[7],
                LocalPort = result[8],
                RemotePort = result[9],
                EdgeTraversal = result[10] == "Yes",
                Action = result[11],
            };
            return res;
        }
        public static void ChangeRuleStatus(string rule_name, bool new_status, string username = null, SecureString password = null)
        {
            string command_result = RunCommand.run("netsh advfirewall firewall set rule name=\"" + rule_name 
                + "\" new enable="+(new_status?"yes":"no"), username, password);
            if (!command_result.Contains("Ok."))
            {
                throw new Exception(command_result);
            }
        }
    }
}
