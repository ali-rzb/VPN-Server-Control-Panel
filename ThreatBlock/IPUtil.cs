using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ThreatBlock
{
    internal class IPUtil
    {

        public class IPInfo
        {
            public string ISP { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public string CountryCode { get; set; }
            public string Region { get; set; }
            public string RegionName { get; set; }
            public string Zip { get; set; }
            public float? Lat { get; set; }
            public float? Lon { get; set; }
            public string Timezone { get; set; }
            public string Org { get; set; }
            public string As { get; set; }
        }
        public static IPInfo GetIPInfo(string ipAddress)
        {
            IPInfo ipInfo = null;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"http://ip-api.com/json/{ipAddress}";

                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();

                    string json = response.Content.ReadAsStringAsync().Result;
                    ipInfo = JsonConvert.DeserializeObject<IPInfo>(json);
                }
                catch (Exception)
                {
                }
            }

            return ipInfo;
        }
    }
}
