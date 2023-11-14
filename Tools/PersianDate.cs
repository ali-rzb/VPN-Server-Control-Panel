using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class PersianDate
    {

        public int Year;
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;
        public int Second;
        public string MonthName;
        public string DayName;

        public PersianDate()
        {

        }

        private static readonly PersianCalendar Pc = new PersianCalendar();
        private static readonly DateTime Now = DateTime.Now;

        static private readonly PersianDate PDate = new PersianDate()
        {
            Year = Pc.GetYear(Now),
            Month = Pc.GetMonth(Now),
            Day = Pc.GetDayOfMonth(Now),
            Hour = Pc.GetHour(Now),
            Minute = Pc.GetMinute(Now),
            Second = Pc.GetSecond(Now),
            MonthName = MonthIndexToName(Pc.GetMonth(Now)),
            DayName = DayIndexToName((int)Pc.GetDayOfWeek(Now))

        };
        public static PersianDate CastToPersian(DateTime dateTime)
        {
            return new PersianDate()
            {
                Year = Pc.GetYear(dateTime),
                Month = Pc.GetMonth(dateTime),
                Day = Pc.GetDayOfMonth(dateTime),
                Hour = Pc.GetHour(dateTime),
                Minute = Pc.GetMinute(dateTime),
                Second = Pc.GetSecond(dateTime),
                MonthName = MonthIndexToName(Pc.GetMonth(dateTime)),
                DayName = DayIndexToName((int)Pc.GetDayOfWeek(dateTime))
            };
        }
        public static PersianDate Parse(string sDate)
        {
            sDate = CheckForPersianNumber(sDate);

            if (sDate.Length == 19)
            {
                if (
                    !(sDate[0] + "" + sDate[1] + "" + sDate[2] + "" + sDate[3]).IsNumber() ||
                    !(sDate[5] + "" + sDate[6]).IsNumber() ||
                    !(sDate[8] + "" + sDate[9]).IsNumber() || !(sDate[11] + "" + sDate[12]).IsNumber() ||
                    !(sDate[14] + "" + sDate[15]).IsNumber() ||
                    !(sDate[17] + "" + sDate[18]).IsNumber() || sDate[4].ToString().IsNumber() ||
                    sDate[7].ToString().IsNumber() ||
                    sDate[10].ToString().IsNumber() || sDate[13].ToString().IsNumber() ||
                    sDate[16].ToString().IsNumber())
                {
                    throw new Exception("ساختار ورودی اشتباه است. ");
                }

                var pdate = new PersianDate()
                {
                    Year =
                        int.Parse(sDate[0] + "" + sDate[1] + "" + sDate[2] + "" + sDate[3]),
                    Month = int.Parse(sDate[5] + "" + sDate[6]),
                    Day = int.Parse(sDate[8] + "" + sDate[9]),
                    Hour = int.Parse(sDate[11] + "" + sDate[12]),
                    Minute = int.Parse(sDate[14] + "" + sDate[15]),
                    Second = int.Parse(sDate[17] + "" + sDate[18]),
                    MonthName = MonthIndexToName(int.Parse(sDate[5] + "" + sDate[6]))
                };
                pdate.DayName =
                    DayIndexToName((int)Pc.GetDayOfWeek(new DateTime(pdate.Year, pdate.Month, pdate.Day, Pc)));

                return pdate;
            }

            if (sDate.Length == 10)
            {
                if (
                    !(sDate[0] + "" + sDate[1] + "" + sDate[2] + "" + sDate[3]).IsNumber() ||
                    !(sDate[5] + "" + sDate[6]).IsNumber() ||
                    !(sDate[8] + "" + sDate[9]).IsNumber() ||
                    sDate[4].ToString() != "/" || sDate[7].ToString() != "/")
                {
                    throw new Exception("ساختار ورودی اشتباه است. ");
                }

                var pdate = new PersianDate()
                {
                    Year =
                        int.Parse(sDate[0] + "" + sDate[1] + "" + sDate[2] + "" + sDate[3]),
                    Month = int.Parse(sDate[5] + "" + sDate[6]),
                    Day = int.Parse(sDate[8] + "" + sDate[9]),
                    Hour = 0,
                    Minute = 0,
                    Second = 0,
                    MonthName = MonthIndexToName(int.Parse(sDate[5] + "" + sDate[6]))
                };
                int dayOfWeek = (int)Pc.GetDayOfWeek(new DateTime(pdate.Year, pdate.Month, pdate.Day, Pc));
                pdate.DayName = DayIndexToName(dayOfWeek);
                return pdate;
            }

            if (sDate.Length == 8)
            {
                if (
                    !(sDate[0] + "" + sDate[1]).IsNumber() ||
                    !(sDate[3] + "" + sDate[4]).IsNumber() ||
                    !(sDate[6] + "" + sDate[7]).IsNumber() ||
                    sDate[2].ToString() != ":" || sDate[5].ToString() != ":")
                {
                    throw new Exception("ساختار ورودی اشتباه است. ");
                }

                var pdate = new PersianDate()
                {
                    Year = 0,
                    Month = 0,
                    Day = 0,
                    Hour = int.Parse(sDate[0] + "" + sDate[1]),
                    Minute = int.Parse(sDate[3] + "" + sDate[4]),
                    Second = int.Parse(sDate[6] + "" + sDate[7]),
                    MonthName = "",
                    DayName = ""
                };
                return pdate;
            }

            if (sDate.Length == 5)
            {
                if (
                    !(sDate[0] + "" + sDate[1]).IsNumber() ||
                    !(sDate[3] + "" + sDate[4]).IsNumber() ||
                    sDate[2].ToString() != ":")
                {
                    throw new Exception("ساختار ورودی اشتباه است. ");
                }

                var pdate = new PersianDate()
                {
                    Year = 0,
                    Month = 0,
                    Day = 0,
                    Hour = int.Parse(sDate[0] + "" + sDate[1]),
                    Minute = int.Parse(sDate[3] + "" + sDate[4]),
                    Second = 0,
                    MonthName = "",
                    DayName = ""
                };
                return pdate;
            }

            if (sDate.Length == 4)
            {
                if (sDate[1] == ':')
                {
                    if (
                        !(sDate[0] + "").IsNumber() ||
                        !(sDate[2] + "" + sDate[3]).IsNumber())
                    {
                        throw new Exception("ساختار ورودی اشتباه است. ");
                    }

                    var pdate = new PersianDate()
                    {
                        Year = 0,
                        Month = 0,
                        Day = 0,
                        Hour = int.Parse(sDate[0] + ""),
                        Minute = int.Parse(sDate[2] + "" + sDate[3]),
                        Second = 0,
                        MonthName = "",
                        DayName = ""
                    };
                    return pdate;
                }
                else if (sDate[2] == ':')
                {
                    if (
                        !(sDate[0] + "" + sDate[1]).IsNumber() ||
                        !(sDate[3] + "").IsNumber())
                    {
                        throw new Exception("ساختار ورودی اشتباه است. ");
                    }

                    var pdate = new PersianDate()
                    {
                        Year = 0,
                        Month = 0,
                        Day = 0,
                        Hour = int.Parse(sDate[0] + "" + sDate[1]),
                        Minute = int.Parse(sDate[3] + ""),
                        Second = 0,
                        MonthName = ""
                    };
                    return pdate;
                }
                else
                {
                    throw new Exception("ساختار ورودی اشتباه است. ");
                }
            }

            if (sDate.Length == 3)
            {
                if (
                    !(sDate[0] + "").IsNumber() ||
                    !(sDate[2] + "").IsNumber() ||
                    sDate[1].ToString() != ":")
                {
                    throw new Exception("ساختار ورودی اشتباه است. ");
                }

                var pdate = new PersianDate()
                {
                    Year = 0,
                    Month = 0,
                    Day = 0,
                    Hour = int.Parse(sDate[0] + ""),
                    Minute = int.Parse(sDate[2] + ""),
                    Second = 0,
                    MonthName = "",
                    DayName = ""
                };
                return pdate;
            }

            return null;
        }
        public static PersianDate Parse(string date,string time)
        {
            try
            {
                return Parse(date + 'T' + time);
            }catch(Exception e)
            {
                throw e;
            }
        }
        public static PersianDate TryParse(string sDate)
        {
            try
            {

                sDate = CheckForPersianNumber(sDate);

                if (sDate.Length == 19)
                {
                    if (
                        !(sDate[0] + "" + sDate[1] + "" + sDate[2] + "" + sDate[3]).IsNumber() ||
                        !(sDate[5] + "" + sDate[6]).IsNumber() ||
                        !(sDate[8] + "" + sDate[9]).IsNumber() || !(sDate[11] + "" + sDate[12]).IsNumber() ||
                        !(sDate[14] + "" + sDate[15]).IsNumber() ||
                        !(sDate[17] + "" + sDate[18]).IsNumber() || sDate[4].ToString().IsNumber() ||
                        sDate[7].ToString().IsNumber() ||
                        sDate[10].ToString().IsNumber() || sDate[13].ToString().IsNumber() ||
                        sDate[16].ToString().IsNumber())
                    {
                        return null;
                    }

                    var pdate = new PersianDate()
                    {
                        Year =
                            int.Parse(sDate[0] + "" + sDate[1] + "" + sDate[2] + "" + sDate[3]),
                        Month = int.Parse(sDate[5] + "" + sDate[6]),
                        Day = int.Parse(sDate[8] + "" + sDate[9]),
                        Hour = int.Parse(sDate[11] + "" + sDate[12]),
                        Minute = int.Parse(sDate[14] + "" + sDate[15]),
                        Second = int.Parse(sDate[17] + "" + sDate[18]),
                        MonthName = MonthIndexToName(int.Parse(sDate[5] + "" + sDate[6]))
                    };
                    pdate.DayName =
                        DayIndexToName((int) Pc.GetDayOfWeek(new DateTime(pdate.Year, pdate.Month, pdate.Day, Pc)));

                    return pdate;
                }

                if (sDate.Length == 10)
                {
                    if (
                        !(sDate[0] + "" + sDate[1] + "" + sDate[2] + "" + sDate[3]).IsNumber() ||
                        !(sDate[5] + "" + sDate[6]).IsNumber() ||
                        !(sDate[8] + "" + sDate[9]).IsNumber() ||
                        sDate[4].ToString() != "/" || sDate[7].ToString() != "/")
                    {
                        return null;
                    }

                    var pdate = new PersianDate()
                    {
                        Year =
                            int.Parse(sDate[0] + "" + sDate[1] + "" + sDate[2] + "" + sDate[3]),
                        Month = int.Parse(sDate[5] + "" + sDate[6]),
                        Day = int.Parse(sDate[8] + "" + sDate[9]),
                        Hour = 0,
                        Minute = 0,
                        Second = 0,
                        MonthName = MonthIndexToName(int.Parse(sDate[5] + "" + sDate[6]))
                    };
                    int dayOfWeek = (int) Pc.GetDayOfWeek(new DateTime(pdate.Year, pdate.Month, pdate.Day, Pc));
                    pdate.DayName = DayIndexToName(dayOfWeek);
                    return pdate;
                }

                if (sDate.Length == 8)
                {
                    if (
                        !(sDate[0] + "" + sDate[1]).IsNumber() ||
                        !(sDate[3] + "" + sDate[4]).IsNumber() ||
                        !(sDate[6] + "" + sDate[7]).IsNumber() ||
                        sDate[2].ToString() != ":" || sDate[5].ToString() != ":")
                    {
                        return null;
                    }

                    var pdate = new PersianDate()
                    {
                        Year = 0,
                        Month = 0,
                        Day = 0,
                        Hour = int.Parse(sDate[0] + "" + sDate[1]),
                        Minute = int.Parse(sDate[3] + "" + sDate[4]),
                        Second = int.Parse(sDate[6] + "" + sDate[7]),
                        MonthName = "",
                        DayName = ""
                    };
                    return pdate;
                }

                if (sDate.Length == 5)
                {
                    if (
                        !(sDate[0] + "" + sDate[1]).IsNumber() ||
                        !(sDate[3] + "" + sDate[4]).IsNumber() ||
                        sDate[2].ToString() != ":")
                    {
                        return null;
                    }

                    var pdate = new PersianDate()
                    {
                        Year = 0,
                        Month = 0,
                        Day = 0,
                        Hour = int.Parse(sDate[0] + "" + sDate[1]),
                        Minute = int.Parse(sDate[3] + "" + sDate[4]),
                        Second = 0,
                        MonthName = "",
                        DayName = ""
                    };
                    return pdate;
                }

                if (sDate.Length == 4)
                {
                    if (sDate[1] == ':')
                    {
                        if (
                            !(sDate[0] + "").IsNumber() ||
                            !(sDate[2] + "" + sDate[3]).IsNumber())
                        {
                            return null;
                        }

                        var pdate = new PersianDate()
                        {
                            Year = 0,
                            Month = 0,
                            Day = 0,
                            Hour = int.Parse(sDate[0] + ""),
                            Minute = int.Parse(sDate[2] + "" + sDate[3]),
                            Second = 0,
                            MonthName = "",
                            DayName = ""
                        };
                        return pdate;
                    }
                    else if (sDate[2] == ':')
                    {
                        if (
                            !(sDate[0] + "" + sDate[1]).IsNumber() ||
                            !(sDate[3] + "").IsNumber())
                        {
                            return null;
                        }

                        var pdate = new PersianDate()
                        {
                            Year = 0,
                            Month = 0,
                            Day = 0,
                            Hour = int.Parse(sDate[0] + "" + sDate[1]),
                            Minute = int.Parse(sDate[3] + ""),
                            Second = 0,
                            MonthName = ""
                        };
                        return pdate;
                    }
                    else
                    {
                        return null;
                    }
                }

                if (sDate.Length == 3)
                {
                    if (
                        !(sDate[0] + "").IsNumber() ||
                        !(sDate[2] + "").IsNumber() ||
                        sDate[1].ToString() != ":")
                    {
                        return null;
                    }

                    var pdate = new PersianDate()
                    {
                        Year = 0,
                        Month = 0,
                        Day = 0,
                        Hour = int.Parse(sDate[0] + ""),
                        Minute = int.Parse(sDate[2] + ""),
                        Second = 0,
                        MonthName = "",
                        DayName = ""
                    };
                    return pdate;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override string ToString()
        {
            if (Hour == 0 && Minute == 0 && Second == 0)
            {
                return $"{Year:D4}/{Month:D2}/{Day:D2}";
            }
            if (Year == 0 && Month == 0 && Day == 0)
            {
                return $"{Hour:D2}:{Minute:D2}:{Second:D2}";
            }
            return $"{Year:D4}-{Month:D2}-{Day:D2}T{Hour:D2}:{Minute:D2}:{Second:D2}";
        }

        public PersianDate PlusHour(int hour)
        {
            var pDate = new PersianDate()
            {
                Year = Year,
                Month = Month,
                Day = Day,
                Hour = Hour,
                Minute = Minute,
                Second = Second
            };
            if (Year == 0 && Month == 0 && Day == 0)
            {
                if ((24 - Hour) > hour)
                {
                    pDate.Hour = Hour + hour;
                }
                else
                {
                    pDate.Hour = hour - (24 - Hour);
                }
            }
            else
            {
                if ((24 - Hour) > hour)
                {
                    pDate.Hour = Hour + hour;
                }
                else
                {
                    if (IsLeaping())
                    {
                        if (Month == 12 && Day == 30)
                        {
                            pDate.Hour = hour - (24 - Hour);
                            pDate.Day = 1;
                            pDate.Month = 1;
                            pDate.MonthName = MonthIndexToName(1);
                            pDate.Year++;
                        }
                        else
                        {
                            if ((Month <= 6 && Day == 31) || Month >= 7 && Day == 30)
                            {
                                pDate.Hour = hour - (24 - Hour);
                                pDate.Day = 1;
                                pDate.Month++;
                                pDate.MonthName = MonthIndexToName(Month + 1);
                            }
                            else
                            {
                                pDate.Hour = hour - (24 - Hour);
                                pDate.Day++;
                            }
                        }
                    }
                    else
                    {
                        if (Month == 12 && Day == 29)
                        {
                            pDate.Hour = hour - (24 - Hour);
                            pDate.Day = 1;
                            pDate.Month = 1;
                            pDate.MonthName = MonthIndexToName(1);
                            pDate.Year++;
                        }
                        else
                        {
                            if ((Month <= 6 && Day == 31) || Month >= 7 && Day == 30)
                            {
                                pDate.Hour = hour - (24 - Hour);
                                pDate.Day = 1;
                                pDate.Month++;
                                pDate.MonthName = MonthIndexToName(Month + 1);

                            }
                            else
                            {
                                pDate.Hour = hour - (24 - Hour);
                                pDate.Day++;
                            }
                        }
                    }
                }
            }
            return pDate;
        }

        public string ToUserFullDate()
        {
            return $"{Day:D2} {MonthName} {Year:D4} ساعت {Hour:D2}:{Minute:D2}";
        }
        public string ToUserDate()
        {
            return $"{Day:D2} {MonthName} {Year:D4}";
        }
        public string ToUserDateWithDayName()
        {
            return $"{DayName} {Day:D2} {MonthName} {Year:D4}";
        }
        public string ToUserTime()
        {
            return $"{Hour:D2}:{Minute:D2}";
        }
        public string ToDate()
        {
            return $"{Year:D4}/{Month:D2}/{Day:D2}";
        }
        public string ToTime()
        {
            return $"{Hour:D2}:{Minute:D2}:{Second:D2}";
        }

        public bool IsLeaping()
        {
            return (((Year + 38) * 31) % 128) <= 30;
        }


        public static string MonthIndexToName(int index, bool finglish = false)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            if (!finglish)
            {
                switch (index)
                {
                    case 1:
                        return "فروردین";
                    case 2:
                        return "اردیبهشت";
                    case 3:
                        return "خرداد";
                    case 4:
                        return "تیر";
                    case 5:
                        return "مرداد";
                    case 6:
                        return "شهریور";
                    case 7:
                        return "مهر";
                    case 8:
                        return "آبان";
                    case 9:
                        return "آذر";
                    case 10:
                        return "دی";
                    case 11:
                        return "بهمن";
                    case 12:
                        return "اسفند";
                }
            }
            else {
                switch (index)
                {
                    case 1:
                        return "Far";
                    case 2:
                        return "Ord";
                    case 3:
                        return "Kho";
                    case 4:
                        return "Tir";
                    case 5:
                        return "Mor";
                    case 6:
                        return "Sha";
                    case 7:
                        return "Meh";
                    case 8:
                        return "Aba";
                    case 9:
                        return "Aza";
                    case 10:
                        return "Dey";
                    case 11:
                        return "Bah";
                    case 12:
                        return "Esp";
                }
            }
            return null;
        }
        public static string DayIndexToName(int index)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (index)
            {
                case 0:
                    return "یکشنبه";
                case 1:
                    return "دوشنبه";
                case 2:
                    return "سه‌شنبه";
                case 3:
                    return "چهارشنبه";
                case 4:
                    return "پنجشنبه";
                case 5:
                    return "جمعه";
                case 6:
                    return "شنبه";
            }
            return null;
        }

        public static PersianDate GetFull()
        {
            if (PDate != null)
            {
                return Parse($"{PDate.Year:D4}-{PDate.Month:D2}-{PDate.Day:D2}T{PDate.Hour:D2}:{PDate.Minute:D2}:{PDate.Second:D2}");
            }
            else
            {
                return new PersianDate();
            }
        }
        public static PersianDate GetDate()
        {
            if (PDate != null)
            {
                return Parse($"{PDate.Year:D4}/{PDate.Month:D2}/{PDate.Day:D2}");
            }
            else
            {
                return new PersianDate();
            }
        }
        public static PersianDate GetTime()
        {
            if (PDate!=null)
            {
                return Parse($"{PDate.Hour:D2}:{PDate.Minute:D2}:{PDate.Second:D2}");
            }
            else
            {
                return new PersianDate();
            }
        }

        public static string CheckForPersianNumber(string String)
        {
            var outPut = "";
            foreach (var s in String)
            {
                if (s >= '۰' && s <= '۹')
                {
                    outPut += (char) (s - '۰' + '0');
                }
                else
                {
                    outPut += s;
                }
            }
            return outPut;
        }


        public static int Compare(PersianDate d1, PersianDate d2)
        {
            //equality
            if (d1.ToString() == d2.ToString())
                return 0;
            //end

            if (d1.Year > d2.Year)
            {
                return 1;
            }
            if (d1.Year < d2.Year)
            {
                return -1;
            }

            // ReSharper disable once InvertIf
            if (d1.Year == d2.Year)
            {
                if (d1.Month > d2.Month)
                {
                    return 1;
                }
                if (d1.Month < d2.Month)
                {
                    return -1;
                }
                // ReSharper disable once InvertIf
                if (d1.Month == d2.Month)
                {
                    if (d1.Day > d2.Day)
                    {
                        return 1;
                    }
                    if (d1.Day < d2.Day)
                    {
                        return -1;
                    }
                    // ReSharper disable once InvertIf
                    if (d1.Day == d2.Day)
                    {
                        if (d1.Hour > d2.Hour)
                        {
                            return 1;
                        }
                        if (d1.Hour < d2.Hour)
                        {
                            return -1;
                        }
                        // ReSharper disable once InvertIf
                        if (d1.Hour == d2.Hour)
                        {
                            if (d1.Minute > d2.Minute)
                            {
                                return 1;
                            }
                            if (d1.Minute < d2.Minute)
                            {
                                return -1;
                            }
                            // ReSharper disable once InvertIf
                            if (d1.Minute == d2.Minute)
                            {
                                if (d1.Second > d2.Second)
                                {
                                    return 1;
                                }
                                if (d1.Second < d2.Second)
                                {
                                    return -1;
                                }
                                if (d1.Second == d2.Second)
                                {
                                    return 0;
                                }
                            }
                        }
                    }
                }
            }
            return -2;
        }

        public static PersianDate GetDuration(PersianDate d1, PersianDate d2)
        {
            //d1 set to first time
            if (Compare(d1, d2) == 1)
            {
                var m = d1;
                d1 = d2;
                d2 = m;
            }

            TimeSpan duration;

            if (d1.Year != 0 && d2.Year != 0)
            {
                duration =
                    new DateTime(d2.Year, d2.Month, d2.Day, d2.Hour, d2.Minute, 0, Pc).Subtract(new DateTime(d1.Year,
                        d1.Month, d1.Day, d1.Hour, d1.Minute, 0, Pc));
            }
            else
            {
                duration =
                    new TimeSpan(d2.Hour, d2.Minute, 0).Subtract(new TimeSpan(d1.Hour, d1.Minute, 0));
            }

            return new PersianDate()
            {
                Day = duration.Days,
                Hour = duration.Hours,
                Minute = duration.Minutes
            };
        }
    }
}
