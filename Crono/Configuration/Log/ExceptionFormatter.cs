using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crono.Configuration.Log
{
    public static class ExceptionFormatter
    {
        public static string Format(string message, Exception err)
        {
            return string.Format("{0}: {1} --> {2}", err.GetType().FullName, string.IsNullOrWhiteSpace(message) ? "." : message, err.Message);
        }

        public static string Concat(string message1, string message2)
        {
            return string.Format("{0} --> {1}", message1, message2);
        }
    }
}
