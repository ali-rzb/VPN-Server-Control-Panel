using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Common
{
    public class InfoTransferUnit : ModelObject
    {
        public Int64 TotalB { get; set; }
        public Int64 TB { get; set; }
        public Int64 GB { get; set; }
        public Int64 MB { get; set; }
        public Int64 KB { get; set; }
        public Int64 B { get; set; }
        public static InfoTransferUnit ParseBytes(Int64 bytes)
        {
            InfoTransferUnit result = new InfoTransferUnit();
            result.TotalB = bytes;
            Int64 temp = bytes;

            result.TB = (Int64)Math.Floor(temp * Math.Pow(10, -12));
            temp -= result.TB * (Int64)Math.Pow(10, 12);

            result.GB = (Int64)Math.Floor(temp * Math.Pow(10, -9));
            temp -= result.GB * (Int64)Math.Pow(10, 9);

            result.MB = (Int64)Math.Floor(temp * Math.Pow(10, -6));
            temp -= result.MB * (Int64)Math.Pow(10, 6);

            result.KB = (Int64)Math.Floor(temp * Math.Pow(10, -3));
            temp -= result.KB * (Int64)Math.Pow(10, 3);

            result.B = temp;

            return result;
        }

        public static InfoTransferUnit ParseBytes(string bytes)
        {
            InfoTransferUnit result = new InfoTransferUnit();
            result.TotalB = Int64.Parse(bytes);
            Int64 temp = result.TotalB;

            result.TB = (Int64)Math.Floor(temp * Math.Pow(10, -12));
            temp -= result.TB * (Int64)Math.Pow(10, 12);

            result.GB = (Int64)Math.Floor(temp * Math.Pow(10, -9));
            temp -= result.GB * (Int64)Math.Pow(10, 9);

            result.MB = (Int64)Math.Floor(temp * Math.Pow(10, -6));
            temp -= result.MB * (Int64)Math.Pow(10, 6);

            result.KB = (Int64)Math.Floor(temp * Math.Pow(10, -3));
            temp -= result.KB * (Int64)Math.Pow(10, 3);

            result.B = temp;

            return result;
        }
        public new string ToString()
        {
            if (TB != 0) { return Math.Round(TotalB * Math.Pow(10, -12), 2).ToString() + " T"; }
            if (GB != 0) { return Math.Round(TotalB * Math.Pow(10, -9), 2).ToString() + " G"; }
            if (MB != 0) { return Math.Round(TotalB * Math.Pow(10, -6)).ToString() + " M"; }
            if (KB != 0) { return Math.Round(TotalB * Math.Pow(10, -3)).ToString() + " K"; }

            return B.ToString() + " B";
        }
    }
}
