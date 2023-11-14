using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThreatBlock
{
    internal static class FirewallClass
    {
        public static string RoleName = "ThreatBlock";
        public static void CreateFirewallRule()
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            INetFwRules firewallRules = firewallPolicy.Rules;

            string ruleName = RoleName;
            bool ruleExists = false;

            foreach (INetFwRule rule in firewallRules)
            {
                if (rule.Name.Equals(ruleName, StringComparison.OrdinalIgnoreCase))
                {
                    ruleExists = true;
                    break;
                }
            }

            if (!ruleExists)
            {
                // Create the firewall rule
                INetFwRule newRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                newRule.Name = ruleName;
                newRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                newRule.Enabled = true;
                // Set other properties as needed, such as Protocol, LocalPorts, etc.
                newRule.RemoteAddresses = "10.10.10.255";

                // Add the rule to the firewall
                firewallRules.Add(newRule);
                Console.WriteLine("Firewall rule created successfully.");
            }
            else
            {
                Console.WriteLine("Firewall rule already exists.");
            }
        }
        public static void AddIPAddressToRule(string[] IPList)
        {
            // Get the Windows Firewall instance
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            // Retrieve the current set of firewall rules
            INetFwRules firewallRules = firewallPolicy.Rules;

            // Find the rule you want to update
            INetFwRule rule = firewallRules.Item(RoleName);


            // Add the IP address to the rule
            foreach (var IP in IPList)
            {
                if (rule.RemoteAddresses == "*" || rule.RemoteAddresses == "")
                {
                    rule.RemoteAddresses = IP;
                }
                else
                {
                    rule.RemoteAddresses += "," + IP;
                }
            }

        }
        public static void RemoveIPAddressFromRule(string[] IPList)
        {
            // Get the Windows Firewall instance
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            // Retrieve the current set of firewall rules
            INetFwRules firewallRules = firewallPolicy.Rules;

            // Find the rule you want to update
            INetFwRule rule = firewallRules.Item(RoleName);

            List<string> ipAddressList = rule.RemoteAddresses.Split(',')
                                             .Select(ip => ip.Trim().Split('/')[0])
                                             .ToList();

            var RemoteAddresses = rule.RemoteAddresses;
            foreach (var IP in IPList)
            {
                if(IP != "10.10.10.255")
                {
                    RemoteAddresses = RemoteAddresses.Replace(IP.Split('/')[0], "");
                }
            }

            string pattern = @",/\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}";
            RemoteAddresses = Regex.Replace(RemoteAddresses, pattern, string.Empty);
            
            pattern = @"/\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}";
            var checkIfFirstIP = Regex.Matches(RemoteAddresses, pattern)[0];
            if (checkIfFirstIP.Index == 0)
            {
                RemoteAddresses = RemoteAddresses.Substring(checkIfFirstIP.Length + 1);
            }

            rule.RemoteAddresses = RemoteAddresses;
        }
        public static List<string> GetIPAddress()
        {
            // Get the Windows Firewall instance
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            // Retrieve the current set of firewall rules
            INetFwRules firewallRules = firewallPolicy.Rules;

            // Find the rule you want to update
            INetFwRule rule = firewallRules.Item(RoleName);

            List<string> ipAddressList = rule.RemoteAddresses.Split(',')
                                             .Select(ip => ip.Trim().Split('/')[0])
                                             .ToList();
            return ipAddressList;
        }
    }
}
