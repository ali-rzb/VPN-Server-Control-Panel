using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using BL;
using Tools;

namespace PreventMultilink
{
    internal class Program
    {
        public static List<string> debug_values = new List<string>()
        {
            "\r\nUser:               mary.f                              \r\nDomain:             CORP                              \r\nConnected from:                                   \r\nDuration:           0 days 3 hours 10 mins 34 secs \r\nRestriction state:  Unknown                              \r\n\r\nUser:               ayda.f                              \r\nDomain:             CORP                              \r\nConnected from:                                   \r\nDuration:           0 days 16 hours 4 mins 5 secs \r\nRestriction state:  Unknown                              \r\n\r\nUser:               maryam.gh                              \r\nDomain:             CORP                              \r\nConnected from:                                   \r\nDuration:           0 days 0 hours 21 mins 39 secs \r\nRestriction state:  Unknown                              \r\n\r\nUser:               hossein.f                              \r\nDomain:             CORP                              \r\nConnected from:                                   \r\nDuration:           0 days 0 hours 55 mins 51 secs \r\nRestriction state:  Unknown                              \r\n\r\nUser:               ali.r                              \r\nDomain:             CORP                              \r\nConnected from:                                   \r\nDuration:           0 days 0 hours 0 mins 29 secs \r\nRestriction state:  Unknown                              \r\n\r\nUser:               aban.m                              \r\nDomain:             CORP                              \r\nConnected from:                                   \r\nDuration:           0 days 1 hours 52 mins 2 secs \r\nRestriction state:  Unknown                              \r\n\r\nUser:               ali.r                              \r\nDomain:             CORP                              \r\nConnected from:                                   \r\nDuration:           0 days 0 hours 0 mins 1 secs \r\nRestriction state:  Unknown                              \r\n\r\nUser:               mina.f                              \r\nDomain:             CORP                              \r\nConnected from:                                   \r\nDuration:           0 days 0 hours 10 mins 22 secs \r\nRestriction state:  Unknown                              \r\n\r\n",
            "\r\nClient Status    \r\n=============    \r\n                 \r\nConnection        : ali.r     \r\nDuration          : 0 Days 0 Hours 2 Mins 1 secs \r\nRestriction state : Unknown                              \r\n                  \r\nStatistics  \r\n----------  \r\nBytes In       : 36176        Bytes out       : 190109       \r\nFrames in      : 291          Frames out      : 260          \r\nCompression in : 0 %              Compression out : 0 %  \r\n                 \r\nErrors           \r\n------ \r\nCRC      : 0                  Framing           : 0             \r\nTime-out : 0                  Hardware Overruns : 0             \r\nAlignment: 0                  Buffer Overruns   : 0             \r\n               \r\nNetwork Registration      \r\n--------------------      \r\nIP Address  : 10.10.10.28 \r\nIPv6 Prefix :   \r\nIPv6 Address:   \r\nNetBEUI name:  \r\n\r\n\r\nClient Status    \r\n=============    \r\n                 \r\nConnection        : ali.r     \r\nDuration          : 0 Days 0 Hours 0 Mins 1 secs \r\nRestriction state : Unknown                              \r\n                  \r\nStatistics  \r\n----------  \r\nBytes In       : 5955         Bytes out       : 8872         \r\nFrames in      : 37           Frames out      : 23           \r\nCompression in : 0 %              Compression out : 0 %  \r\n                 \r\nErrors           \r\n------ \r\nCRC      : 0                  Framing           : 0             \r\nTime-out : 0                  Hardware Overruns : 0             \r\nAlignment: 0                  Buffer Overruns   : 0             \r\n               \r\nNetwork Registration      \r\n--------------------      \r\nIP Address  : 10.10.10.23 \r\nIPv6 Prefix :   \r\nIPv6 Address:   \r\nNetBEUI name:  \r\n\r\n\r\n",
            "David",
            "Eve"
        };
        static bool plot = false;
        static void Main(string[] args)
        {
            try
            {
                // Check if .NET Framework 4.8 is installed
                // "Please install .NET Framework 4.8: https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer"
                UserBl userBl = new UserBl();
                if (args.Length != 0)
                {
                    if (args[0] == "conn")
                    {
                        Connection();
                    }
                    else if (args[0] == "disconn")
                    {
                        Disconnection();
                    }
                    else
                    {
                        if(plot)
                            Console.WriteLine("Invalid argument. Please use 'conn' or 'disconn'.");
                    }
                    if (args.Length == 2)
                    {
                        try
                        {
                            plot = bool.Parse(args[1]);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                else
                {
                    if (plot)
                        Console.WriteLine("Please provide an argument. Use 'conn' to block users from connecting and 'disconn' to unblock them.");
                }
            }
            catch (Exception ex)
            {
                if (plot)
                    Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        public static int ConvertToSeconds(string input)
        {
            string[] parts = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            for (int i = 0; i < parts.Length; i += 2)
            {
                int value = int.Parse(parts[i]);
                string unit = parts[i + 1];

                switch (unit)
                {
                    case "days":
                        days = value;
                        break;
                    case "hours":
                        hours = value;
                        break;
                    case "mins":
                        minutes = value;
                        break;
                    case "secs":
                        seconds = value;
                        break;
                }
            }
            int totalSeconds = (days * 24 * 60 * 60) + (hours * 60 * 60) + (minutes * 60) + seconds;
            return totalSeconds;
        }
        public static List<string> GetUsersForGroupNet(string GroupName)
        {
            string commandOutput = RunCommand.run($"net localgroup \"{GroupName}\"", seperate_lines: true);

            List<string> usernames = new List<string>();

            var lines = commandOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            int startIndex = lines.FindIndex(c => c.Contains("Members"));
            int endIndex = lines.FindLastIndex(c => c.Contains(">"));

            return lines.GetRange(startIndex + 2, endIndex - startIndex - 3);
        }
        public static List<string> GetConnections()
        {
            var clients_text = RunCommand.run("netsh ras show client");
            //var clients_text = debug_values[0];
            var splitted = clients_text.Split(new string[] { "User:", "Domain" }, 50, StringSplitOptions.RemoveEmptyEntries);
            var conn_users = new List<string>();
            for (int i = 1; i < splitted.Length; i += 2)
            {
                conn_users.Add(splitted[i].Trim().ToLower());
            }
            return conn_users;
        }
        public static (List<string>, List<string>) GetUsersToCheck()
        {
            var clients_text = RunCommand.run("netsh ras show client");
            //var clients_text = debug_values[0];
            var splitted = clients_text.Split(new string[] { "User:", "Domain", "Duration:", "Restriction state:" }, 50, StringSplitOptions.RemoveEmptyEntries);

            List<(string, int)> users_with_sec = new List<(string, int)>();

            var connections = new List<string>();
            for (int i = 1; i < splitted.Length; i += 4)
            {
                users_with_sec.Add((splitted[i].Trim().ToLower(), ConvertToSeconds(splitted[i + 2].Trim().ToLower())));
                connections.Add(users_with_sec.Last().Item1);
            }
            users_with_sec.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            Dictionary<string, int> userTimes = new Dictionary<string, int>();
            foreach (var result in users_with_sec)
            {
                string username = result.Item1;
                int timeInSeconds = result.Item2;

                if (userTimes.ContainsKey(username))
                {
                    if (timeInSeconds < userTimes[username])
                    {
                        userTimes[username] = timeInSeconds;
                    }
                }
                else
                {
                    userTimes[username] = timeInSeconds;
                }
            }
            int counter = 0;
            var users = new List<string>();
            foreach (var userTime in userTimes)
            {
                counter++;
                users.Add(userTime.Key);
                if (counter == 2)
                {
                    break;
                }
            }
            return (users, connections);
        }
        static void Connection()
        {
            UserBl userBl = new UserBl();
            if (plot)
                Console.WriteLine("Connection Check");
            try
            {
                var users = new List<string>();
                var connections = new List<string>();
                (users, connections) = GetUsersToCheck();
                var users_multilink_capacities = userBl.getUsersExtraInfo();

                for (int i = 0; i < users.Count(); i++)
                {
                    var temp = users_multilink_capacities.Where(m => m.Username.Trim().ToLower() == users[i]).FirstOrDefault();
                    var multilink_limit = temp == null ? 1 : temp.MultilinkCapacity;

                    int conn_count = (from conn in connections where conn == users[i] select conn).Count();
                    if (plot)
                        Console.WriteLine("\n" + users[i] + " : " + conn_count + " / " + multilink_limit);
                    if (conn_count >= multilink_limit)
                    {
                        if (plot)
                            Console.WriteLine("Closing " + users[i] + "...");
                        try
                        {
                            userBl.AddToGroup(users[i], Credentials.VpnClientsBlockGroup);
                        }
                        catch (Exception ex)
                        {
                            if (plot)
                                Console.WriteLine(ex.Message);
                        }
                        if (plot)
                            Console.WriteLine("done!");
                        if (conn_count > multilink_limit)
                        {
                            var res = RunCommand.run("netsh ras show client " + users[i]);
                            //var res = debug_values[1];

                            var splitted = res.Split(new string[] { "IP Address  : ", "IPv6 Prefix :" }, 50, StringSplitOptions.RemoveEmptyEntries);
                            var ip_list = (from j in Enumerable.Range(0, ((splitted.Length - 1) / 2)) select splitted[1 + j * 2].Trim()).ToList();
                            for (uint j = multilink_limit; j < ip_list.Count(); j++)
                            {
                                var ip = splitted[j].Trim();
                                if (plot)
                                    Console.WriteLine(ip);
                                res = RunCommand.run("Disconnect-VpnUser -HostIPAddress " + ip, target: "powershell.exe");

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (plot)
                    Console.Write(ex.Message);
            }
        }
        static void Disconnection()
        {
            if (plot)
                Console.WriteLine("\nDisconnection Check : ");
            UserBl userBl = new UserBl();
            try
            {
                var conn_users = GetConnections();
                var target_list = new List<string>();
                for (int i = 0; i < 60; i++)
                {
                    Thread.Sleep(500);
                    var new_conn_users = GetConnections();
                    if(plot)
                        Console.WriteLine($"{i} - [{string.Join(" - ", new_conn_users)}]");
                    foreach (var conn_user in conn_users)
                    {
                        if (!new_conn_users.Contains(conn_user))
                        {
                            target_list.Add(conn_user);
                        }
                    }
                    if(target_list.Count > 0 || new_conn_users.Count == 0)
                    {
                        conn_users = new_conn_users;
                        break;
                    }
                }
                
                var users_multilink_capacities = userBl.getUsersExtraInfo();
                var users = GetUsersForGroupNet(Credentials.VpnClientsBlockGroup);

                for (int i = 0; i < users.Count(); i++)
                {
                    var temp = users_multilink_capacities.Where(m => m.Username == users[i]).FirstOrDefault();
                    var multilink_limit = temp == null ? 1 : temp.MultilinkCapacity;
                    int conn_count = (from user in conn_users where user == users[i] select user).Count();
                    if (plot) 
                        Console.WriteLine("\n" + users[i] + " : " + conn_count + " / " + multilink_limit);
                    if (conn_count < multilink_limit)
                    {
                        if (plot)
                            Console.WriteLine("Re-Openning " + users[i] + "...");
                        try
                        {
                            userBl.RemoveFromGroup(users[i], Credentials.VpnClientsBlockGroup);
                        }
                        catch (Exception ex)
                        {
                            if (plot)
                                Console.Write(ex.Message);
                        }
                        if (plot)
                            Console.WriteLine("done!");
                    }
                }
            }
            catch (Exception ex)
            {
                if (plot)
                    Console.Write(ex.Message);
            }
            Console.ReadKey();
        }
    }
}
