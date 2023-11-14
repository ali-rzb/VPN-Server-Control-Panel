using System;

namespace Tools
{
    public static class DateTimeUtils
    {
        public static string ToFriendlyString(this TimeSpan dt, int shortMode = 0)
        {
            if (shortMode == 0)
            {
                if (dt.Days != 0)
                {
                    return dt.ToString(@"d\d\ h\h\ m\m");
                }
                else if (dt.Hours != 0)
                {
                    return dt.ToString(@"h\h\ m\m");
                }
                else if (dt.Minutes != 0)
                {
                    return dt.ToString(@"m\m");
                }
                else if (dt.Seconds != 0)
                {
                    return dt.ToString(@"s\s\ fff\m\s");
                }
                else
                {
                    return dt.ToString(@"fff\m\s");
                }
            }else if (shortMode == 1)
            {
                if (dt.Days != 0)
                {
                    return String.Format("{0} d", Math.Round(dt.Days + dt.Hours / 24.0, 1));
                }
                else if (dt.Hours != 0)
                {
                    return String.Format("{0} h", Math.Round(dt.Hours + dt.Minutes / 60.0, 1));
                }
                else if (dt.Minutes != 0)
                {
                    return String.Format("{0} m", Math.Round(dt.Minutes + dt.Seconds / 60.0, 1));
                }
                else if (dt.Seconds != 0)
                {
                    return String.Format("{0} s", Math.Round(dt.Seconds + dt.Milliseconds / 1000.0, 1));
                }
                else
                {
                    return String.Format("{0} ms", dt.Milliseconds);
                }
            }
            else
            {
                if (dt.Days != 0)
                {
                    return String.Format("{0} days", Math.Round(dt.Days + dt.Hours / 24.0, 1));
                }
                else if (dt.Hours != 0)
                {
                    return String.Format("{0} hours", Math.Round(dt.Hours + dt.Minutes / 60.0, 1));
                }
                else if (dt.Minutes != 0)
                {
                    return String.Format("{0} minutes", Math.Round(dt.Minutes + dt.Seconds / 60.0, 1));
                }
                else if (dt.Seconds != 0)
                {
                    return String.Format("{0} seconds", Math.Round(dt.Seconds + dt.Milliseconds / 1000.0, 1));
                }
                else
                {
                    return String.Format("{0} milliseconds", dt.Milliseconds);
                }
            }

        }
        
    }
}
