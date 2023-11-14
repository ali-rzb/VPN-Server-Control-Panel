using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class NumberTypeChecker
    {
        public static bool IsNumber(this string myString)
        {
            try
            {
                // ReSharper disable once SuggestVarOrType_BuiltInTypes
                // ReSharper disable once UnusedVariable
                int a = int.Parse(myString) + 1;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
