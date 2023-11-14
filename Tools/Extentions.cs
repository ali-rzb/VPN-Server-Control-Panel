using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class Extentions
    {
    }
    public static class LinqExtensions
    {
        public static T MinOrDefault<T>(this IEnumerable<T> source, T defaultValue)
        {
            if (source.Any<T>())
                return source.Min<T>();

            return defaultValue;
        }

        public static T MaxOrDefault<T>(this IEnumerable<T> source, T defaultValue)
        {
            if (source.Any<T>())
                return source.Max<T>();

            return defaultValue;
        }
    }
}
