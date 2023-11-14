using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class DialogState
    {
        public static string Error(string errorMsg)
        {
            return string.IsNullOrEmpty(errorMsg) ? "none" : "block";
        }

        public static string Success(string successMsg)
        {
            return string.IsNullOrEmpty(successMsg) ? "none" : "block";
        }
    }
}
