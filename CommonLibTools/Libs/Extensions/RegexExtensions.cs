using System.Text.RegularExpressions;

namespace CommonLibTools.Libs.Extensions
{
    public static class RegexExtensions
    {

        public static string TrimEnd(this string input, string suffixToRemove)
        {
            if (input != null && suffixToRemove != null
              && input.EndsWith(suffixToRemove))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else return input;
        }
        public static int ExtractIntegerInRegexGroup(this string s, string regexPattern)
        {
            if (s.IsNullOrEmptyString())
            {
                return 0;
            }
            int val = 0;

            var match = Regex.Match(s.ToLower()/*.SansAccent()*/, regexPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                s = match.Groups[1].Value;
            }
            int.TryParse(s, out val);
            return val;
        }

        public static string ExtractInRegexGroup(this string s, string regexPattern)
        {
            if (s.IsNullOrEmptyString())
            {
                return s;
            }


            var match = Regex.Match(s.ToLower()/*.SansAccent()*/, regexPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                s = match.Groups[1].Value;
            }
            return s;
        }
    }
}
