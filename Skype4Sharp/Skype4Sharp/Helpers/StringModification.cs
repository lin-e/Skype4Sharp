using System.Linq;
using System.Web;

namespace Skype4Sharp // Used by multiple items not in Skype4Sharp.Helpers
{
    public static class StringModification
    {
        public static string UrlEncode(this string inputString)
        {
            return HttpUtility.UrlEncode(inputString);
        }
        public static string HtmlDecode(this string inputString)
        {
            return HttpUtility.HtmlDecode(inputString);
        }
        public static string JsonEscape(this string inputString)
        {
            string newString = "";
            char[] specialChars = { '\\', '?', ':', '{', '}', '[', ']', '"' };
            foreach (char x in inputString.ToCharArray())
            {
                if ((specialChars.Contains(x)) || x > 127)
                {
                    newString += string.Format(@"\u{0:x4}", (int)x);
                }
                else
                {
                    newString += x.ToString();
                }
            }
            return newString;
        }
        public static string StripTags(this string inputString)
        {
            char[] characterArray = new char[inputString.Length];
            int arrayIndex = 0;
            bool insideTag = false;
            for (int i = 0; i < inputString.Length; i++)
            {
                char checkLetter = inputString[i];
                if (checkLetter == '<')
                {
                    insideTag = true;
                    continue;
                }
                if (checkLetter == '>')
                {
                    insideTag = false;
                    continue;
                }
                if (!insideTag)
                {
                    characterArray[arrayIndex] = checkLetter;
                    arrayIndex++;
                }
            }
            return new string(characterArray, 0, arrayIndex);
        }
    }
}
