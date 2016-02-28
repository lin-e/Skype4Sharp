using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Skype4Sharp.Helpers
{
    public class Misc
    {
        public static Int64 getTime()
        {
            Int64 returnValue = 0;
            var startTime = new DateTime(1970, 1, 1);
            TimeSpan timeSpan = (DateTime.Now.ToUniversalTime() - startTime);
            returnValue = (Int64)(timeSpan.TotalMilliseconds + 0.5);
            return returnValue;
        }
        public static byte[] hashMD5_Byte(string strToHash)
        {
            return new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(strToHash));
        }
    }
}
