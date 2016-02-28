using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ExampleBot
{
    static class Helpers
    {
        public static string HtmlEncode(this string inputString)
        {
            return HttpUtility.HtmlEncode(inputString);
        }
    }
}
