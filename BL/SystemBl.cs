using Microsoft.VisualBasic.Devices;
using Models;
using Models.Common;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tools;

namespace BL
{
    public class ConnectionError : Exception
    {
        public ConnectionError(string message)
            : base(message)
        {
        }
    }
    public sealed class PerformanceReader
    {
        private static NetworkInterface _network;
        public static NetworkInterface Network
        {
            get
            {
                if (_network == null)
                {
                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        _network = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(x => x.Name.Contains("public"));
                    }
                }

                return _network;
            }
        }

        private static PerformanceCounter _cpu;
        public static PerformanceCounter Cpu
        {
            get
            {
                if (_cpu == null)
                {
                    _cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                }

                return _cpu;
            }
        }
    }

    public class SystemBl
    {
        public SystemPerformance GetServerPerformance()
        {
            // Network
            float speed = 0;
            float send = 0;
            float rec = 0;
            try
            {
                NetworkInterface network = PerformanceReader.Network;
                if (network != null)
                {
                    speed = (float)Math.Round(network.Speed / 1000000.0, 1);
                    IPv4InterfaceStatistics ipv4Stats = network.GetIPv4Statistics();
                    send = (float)Math.Round(ipv4Stats.BytesSent / 1000000.0, 1);
                    rec = (float)Math.Round(ipv4Stats.BytesReceived / 1000000.0, 1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting network info! : " + ex.Message);
            }

            // CPU
            float cpu = 0;
            try
            {
                cpu = PerformanceReader.Cpu.NextValue();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting cpu info! : " + ex.Message);
            }

            // RAM
            long Total = 0;
            long Used = 0;
            try
            {
                ComputerInfo computerInfo = new ComputerInfo();

                Total = (long)(computerInfo.TotalPhysicalMemory / 1048576);
                long Free = (long)(computerInfo.AvailablePhysicalMemory / 1048576);
                Used = Total - Free;

            }
            catch (Exception ex)
            {
                throw new Exception("Error getting ram info! : " + ex.Message);
            }

            // Return
            return new SystemPerformance
            {
                CpuUsage = (int)cpu,
                NetSpeed = speed,
                NetUsageRecieved = rec,
                NetUsageSent = send,
                RamTotal = (float)Math.Round(Total / 1024.0, 2),
                RamUsed = (float)Math.Round(Used / 1024.0, 2)
            };
        }
        public ServerSetting GetServerSetting()
        {
            ServerSetting settings = null;
            if (File.Exists(Credentials.ServerSettingsFilePath))
            {
                XmlSerializer reader = new XmlSerializer(typeof(ServerSetting));
                StreamReader file = new StreamReader(Credentials.ServerSettingsFilePath);
                settings = (ServerSetting)reader.Deserialize(file);
                file.Close();
            }
            else
            {
                settings = new ServerSetting();
            }
            settings = new ServerSetting()
            {
                ClientsCanChangePass = settings != null ? settings.ClientsCanChangePass : false,
                RdpRuleStatus = FirewallManagement.CheckRule("RDP TCP").Enabled,
                IpAddress = settings.IpAddress,
            };
            
            XmlSerializer serializer = new XmlSerializer(typeof(ServerSetting));
            using (TextWriter writer = new StreamWriter(Credentials.ServerSettingsFilePath))
            {
                serializer.Serialize(writer, settings);
            }
            
            //try
            //{
            //    settings.Password = Encryption.Decrypt(settings.Password, Credentials.EncriptionKey);
            //}
            //catch
            //{
            //    settings.Password = string.Empty;
            //}
            return settings;
        }
        public void UpdateServerSettings(ServerSetting settings)
        {
            if (File.Exists(Credentials.ServerSettingsFilePath))
            {
                File.Delete(Credentials.ServerSettingsFilePath);
            }
            XmlSerializer serializer = new XmlSerializer(typeof(ServerSetting));
            using (TextWriter writer = new StreamWriter(Credentials.ServerSettingsFilePath))
            {
                serializer.Serialize(writer, settings);
            }
        }
        public FailedLoginsVm CountConnectionsPerDay(int interval = 12, int step = 12)
        {
            // intervals : how many data in output
            // step      : how many hour for each data
            Credentials cred = new Credentials();
            var result = new FailedLoginsVm()
            {
                stats = new List<int> { },
                dates = new List<DateTime>()
            };
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            EventLogSession session = new EventLogSession(cred.PublicAddress);

            string query = "*[System[(EventID=20274)]]";
            EventLogQuery evntquery = new EventLogQuery("System", PathType.LogName, query) { Session = session };
            EventLogReader reader = new EventLogReader(evntquery);
            var eventInstance = reader.ReadEvent();

            if (eventInstance != null)
            {
                var curent_date = eventInstance.TimeCreated.Value;
                var count_steps_per_day = (int)(24 / step);
                var differences = new List<TimeSpan>();
                for (int i = 0; i < count_steps_per_day; i++)
                {
                    differences.Add(curent_date.Date.AddHours(i * step) - curent_date);
                }
                int ind = differences.IndexOf(differences.Max());
                curent_date = curent_date.Date.AddHours(ind * step);
                while (curent_date >= eventInstance.TimeCreated)
                {
                    eventInstance = reader.ReadEvent();
                }
                
                while (true)
                {
                    int count = 0;

                    try
                    {
                        eventInstance = reader.ReadEvent();
                    }
                    catch
                    {
                        break;
                    }
                                       
                    if(eventInstance == null)
                    {
                        break;
                    }
                    
                    while (null != eventInstance && DateTime.Compare(curent_date, eventInstance.TimeCreated.Value) <= 0 && DateTime.Compare(eventInstance.TimeCreated.Value, curent_date.AddHours(step)) < 0)
                    {
                        count++;
                        eventInstance = reader.ReadEvent();
                    }
                    result.stats.Add(count);
                    result.dates.Add(curent_date);
                    curent_date = curent_date.AddHours(step);
                    if (curent_date > DateTime.Now)
                    {
                        break;
                    }
                }
                if (result.stats.Count > interval)
                {
                    result.stats = result.stats.Skip(result.stats.Count - interval).ToList();
                    result.dates = result.dates.Skip(result.dates.Count - interval).ToList();
                }
            }
            return result;
        }
    }
}
