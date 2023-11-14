using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Tools
{
    public static class Ip
    {

        public static List<int> Backup_Format = new List<int>{109, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
        32, 32, 32, 32, 32, 100, 1, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
        80, 2, 46, 52, 1, 109, 115, 82, 65, 68, 73, 85, 83, 70, 114, 97, 109, 101, 100, 73, 80, 65, 100, 100, 114, 101, 115, 115, 13107,
        12336, 24883, 12336, 14643, 12336, 24883, 12336, 12595, 12336, 13875, 12336, 14387, 12336, 13363, 12336, 13107, 12336, 12339, 12336,
        12339, 12336, 14387, 12336, 12851, 12336, 50, 52, 1, 109, 115, 82, 65, 83, 83, 97, 118, 101, 100, 70, 114, 97, 109, 101, 100, 73, 80,
        65, 100, 100, 114, 101, 115, 115, 13107, 12336, 24883, 12336, 14643, 12336, 24883, 12336, 12595, 12336, 13875, 12336, 14387, 12336,
        13363, 12336, 13107, 12336, 12339, 12336, 12339, 12336, 14387, 12336, 12851, 12336};


        public static List<int> encodeIP(string ipAddress)
        {
            var ip = IPAddress.Parse(ipAddress);
            var decimalIp = BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);

            bool higherThan127Ind = decimalIp > 2147483647;
            if (higherThan127Ind)
            {
                decimalIp = (uint)(4294967296 - decimalIp);
            }

            var decimals = new List<double>();
            while (decimalIp > 0)
            {
                decimals.Add(decimalIp % 10);
                decimalIp = (uint)Math.Floor((double)(decimalIp / 10));
            }
            decimals.Reverse();

            int ipLength = decimals.Count();
            decimals.InsertRange(0, new double[] { 3, 49, 49 });
            if (higherThan127Ind)
            {
                decimals.Insert(3, 51);
            }
            decimals.Insert(2, ipLength);

            var input = new List<int>();
            foreach (var decimalVal in decimals)
            {
                input.Add((int)(decimalVal*256 + 12339));
                input.Add(12336);
            }

            return input;
        }

        public static string decodeIP(List<int> input)
        {
            // Step 1: Remove constants
            // 12336 is repeated after every item
            input.RemoveAll(x => x == 12336);

            // Step 2: Remove constant value
            for (int i = 0; i < input.Count(); i++)
            {
                input[i] -= 12339;
            }

            // Step 3: Divide by 256
            var decimals = new List<double>();
            for (int i = 0; i < input.Count(); i++)
            {
                decimals.Add(input[i] / 256);
            }


            int firstIndicator = decimals.IndexOf(49);
            int secondIndicator = decimals.IndexOf(49, firstIndicator + 1);
            int calc_len = 0;
            for (int i = secondIndicator - 1; i > firstIndicator; i--)
            {
                calc_len = calc_len + (int)(decimals[i] * Math.Pow(10, secondIndicator - 1 - i));
            }
            decimals.RemoveRange(0, (int)(decimals.Count() - calc_len));

            bool higher_than_127_ind = decimals[0] == 51;
            if (higher_than_127_ind)
            {
                decimals.RemoveAt(0);
            }
            string decimalIp_str = "";
            for (int i = 0; i < decimals.Count(); i++)
            {
                decimalIp_str += decimals[i].ToString();
            }
            var decimalIp = double.Parse(decimalIp_str);
            if (higher_than_127_ind)
            {
                decimalIp = 4294967296 - decimalIp;
            }

            return IPAddress.Parse(decimalIp.ToString()).ToString();
        }

        public static string get_ip_from_dialin_parameters(string input)
        {
            var encoded = new List<int>();
            string key_flag = "msRADIUSFramedIPAddress";

            var text = "";
            for (int i = 0; i < input.Count(); i++)
            {
                text += ", " + ((int)input[i]).ToString();
            }
            
            int index = input.IndexOf(key_flag);
            if (index == -1)
            {
                return "";
            }
            char c = new char();
            for (int i = index + key_flag.Length; i < input.Count(); i++)
            {
                c = input[i];
                if (c < 12336)
                {
                    break;
                }
                encoded.Add(c);
            }
            if (encoded.Count <= 0)
            {
                return "";
            }
            return decodeIP(encoded);
        }
        public static string int_list_to_string(List<int> input)
        {
            string output = "";
            foreach (var item in input) output += (char)item;
            return output;
        }
        public static string update_dialin_parameters(string input, string static_ip)
        {
            var text = new List<int>();

            if (string.IsNullOrEmpty(input))
            {
                text = Backup_Format;
                input = int_list_to_string(text);
            }
            else
            {
                foreach (var item in input) text.Add(item);
            }
            
            string[] key_flag_list = { "msRADIUSFramedIPAddress", "msRASSavedFramedIPAddress" };
            var encodedIp = encodeIP(static_ip);
            encodedIp.Reverse();
            foreach (var key_flag in key_flag_list)
            {
                int index = input.IndexOf(key_flag);
                if (index == -1) continue;
                index = index + key_flag.Length;
                while (text.Count() != index && text[index] >= 12336)
                {
                    text.RemoveAt(index);
                }
                foreach (var item in encodedIp)
                {
                    text.Insert(index, item);
                }
                input = int_list_to_string(text);
            }
            string output = "";
            foreach (var item in text)
            {
                output += (char)item;
            }
            return output;
        }

    }
}
