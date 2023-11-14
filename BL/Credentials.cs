using Models;
using System;
using System.IO;
using System.Net;
using System.Security;
using System.Xml.Serialization;

namespace BL
{
    public class Credentials
    {
        public ServerSetting settings;
        public string PublicAddress;

        public Credentials()
        {
            if (settings == null)
            {
                if (File.Exists(ServerSettingsFilePath))
                {
                    try
                    {
                        XmlSerializer reader = new XmlSerializer(typeof(ServerSetting));
                        using (StreamReader file = new StreamReader(Credentials.ServerSettingsFilePath))
                        {
                            settings = (ServerSetting)reader.Deserialize(file);
                            PublicAddress = settings.IpAddress;
                        }
                        if (string.IsNullOrEmpty(settings.IpAddress))
                        {
                            throw new ConnectionError("Enter connection info!");
                        }
                    }
                    catch
                    {
                        throw new ConnectionError("Enter connection info!");
                    }
                }
                else
                {
                    throw new ConnectionError("Enter connection info!");
                }
            }
        }
        

        public static string VpnClientsGroup = "VPN Users";
        public static string VpnClientsBlockGroup = "Denied Users";

        public static string EncriptionKey = "123";
        

        public static string ServerSettingsFilePath = @"C:/ServerSettings/ServerSettings.xml";
        public static string UsersExtraInfoFilePath = @"C:/ServerSettings/UsersExtraInfo.xml";

    }
}
