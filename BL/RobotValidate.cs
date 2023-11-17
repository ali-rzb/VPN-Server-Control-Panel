using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BL
{
    public class RobotValidate
    {
        private Dictionary<string, string> Keys = new Dictionary<string, string>() {
            {"Admin CP", "XXXXX" },
            {"Client CP", "YYYYY" },
        };
        private string SecretKey { get; set; }
        public RobotValidate(string site = null)
        {
            if (site == null)
            {
                if (Keys.Keys.Count == 1)
                {
                    site = Keys.Keys.ToList()[0];
                }
                else
                {
                    throw new Exception("Site Name Not Entered!");
                }
            }
            SecretKey = Keys[site];
        }
        public bool ValidateV2(string response)
        {
            string Response = response;//Getting Response String Append to Post Method
            bool Valid = false;
            //Request to Google Server
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
            (" https://www.google.com/recaptcha/api/siteverify?secret=" + SecretKey + "&response=" + Response);
            try
            {
                //Google recaptcha Response
                using (WebResponse wResponse = req.GetResponse())
                {

                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        JavaScriptSerializer js = new JavaScriptSerializer();
                        V2ResponseObject data = js.Deserialize<V2ResponseObject>(jsonResponse);// Deserialize Json

                        Valid = Convert.ToBoolean(data.success);
                    }
                }

                return Valid;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
        public async Task<bool> ValidateV3(string response, string ip, string action)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>()
                    {
                        {"secret", SecretKey },
                        {"response", response },
                        {"remoteip", ip}
                    };
                    var content = new FormUrlEncodedContent(values);
                    var verify = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                    var captchaResponseJson = await verify.Content.ReadAsStringAsync();
                    var captchaResult = JsonConvert.DeserializeObject<V3ResponseObject>(captchaResponseJson);
                    return captchaResult.Success
                        && captchaResult.Action == action
                        && captchaResult.Score > 0.5;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public class V2ResponseObject
        {
            public string success { get; set; }
        }

        public class V3ResponseObject
        {
            public bool Success { get; set; }
            [JsonProperty(PropertyName = "error-codes")]
            public IEnumerable<string> ErrorCodes { get; set; }
            [JsonProperty(PropertyName = "challenge_ts")]
            public DateTime ChallengeTime { get; set; }
            public string HostName { get; set; }
            public double Score { get; set; }
            public string Action { get; set; }

        }
    }



}