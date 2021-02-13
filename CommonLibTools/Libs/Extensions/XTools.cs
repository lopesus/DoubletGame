using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommonLibTools.Libs.Extensions
{

    public static class XTools
    {
        public static void PrintObject(this Object o)
        {
            if (o != null)
            {
                System.Diagnostics.Debug.WriteLine(o.ToString());
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("object is null");
            }
        }

        public static int CountCharNumber(this string source, char car)
        {
            if (source == null)
            {
                return 0;
            }
            int count = 0;
            for (int index = 0; index < source.Length; index++)
            {
                char c = source[index];
                if (c == car) count++;
            }

            return count;
        }
        public static string SonifierNomAfro(this string nom)
        {
            //var consonnes = "[b-df-hj-np-tv-xz]";
            //si double consonnes au debut du nom
            if (Regex.IsMatch(nom, "^[b-df-hj-np-tv-z]{2}"))
            {
                //mot style [nm]*
                if (Regex.IsMatch(nom, "^[nm][^hw]"))
                {
                    nom = nom.Substring(1);
                }

            }
            return nom;
        }

        public static List<string> FilterMainEmailProvider(this List<string> emails)
        {
            if (emails == null || emails.Count == 0)
            {
                return emails;
            }

            var filter = emails.Where(e =>
                {
                    var provider =
                        e.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries).Last();
                    if (provider.ToLower().Contains("yahoo"))
                    {
                        return true;
                    }
                    if (provider.ToLowerInvariant().Contains("ymail"))
                    {
                        return true;
                    }
                    if (provider.ToLowerInvariant().Contains("gmail"))
                    {
                        return true;
                    }
                    if (provider.ToLowerInvariant().Contains("hotmail"))
                    {
                        return true;
                    }
                    if (provider.ToLowerInvariant().Contains("live"))
                    {
                        return true;
                    }
                    if (provider.ToLowerInvariant().Contains("outlook"))
                    {
                        return true;
                    }
                    if (provider.ToLowerInvariant().Contains("rocketmail"))
                    {
                        return true;
                    }
                    return false;
                }).ToList();

            return filter;
        }

      
        public static string ToFormattedCurrency(this double prix)
        {
            var s = prix.ToString(CultureInfo.InvariantCulture);
            if (s.IsNotNullOrEmptyString())
            {
                var parts = s.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var entiers = parts.First().ToCharArray().Reverse();
                int count = 0;
                string result = "";
                foreach (var entier in entiers)
                {
                    result += entier;
                    count++;
                    if (count == 3)
                    {
                        count = 0;
                        result += " ";
                    }
                }
                result = result.Reverse().ToString();
                if (parts.Length > 1)
                {
                    var deci = parts.Last();
                    result += "," + deci;
                }
                return result;
            }
            return s;
        }

        public static string ToFormattedCurrency(this int prix)
        {
            var s = prix.ToString(CultureInfo.InvariantCulture);
            if (s.IsNotNullOrEmptyString())
            {
                var parts = s.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var entiers = parts.First().ToCharArray().Reverse();
                int count = 0;
                string result = "";
                foreach (var entier in entiers)
                {
                    result += entier;
                    count++;
                    if (count == 3)
                    {
                        count = 0;
                        result += " ";
                    }
                }
                result = result.Reverse().ToString();
                if (parts.Length > 1)
                {
                    var deci = parts.Last();
                    result += "," + deci;
                }
                return result;
            }
            return s;
        }

        public static string ToFormattedCurrency(this string s)
        {
            if (s.IsNotNullOrEmptyString())
            {
                var parts = s.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var entiers = parts.First().ToCharArray().Reverse();
                int count = 0;
                string result = "";
                foreach (var entier in entiers)
                {
                    result += entier;
                    count++;
                    if (count == 3)
                    {
                        count = 0;
                        result += " ";
                    }
                }
                result = result.Reverse().ToString();
                if (parts.Length > 1)
                {
                    var deci = parts.Last();
                    result += "," + deci;
                }
                return result;
            }
            return s;
        }

        public static bool IsPhoneNumber(this string s)
        {
            //Regex regex = new Regex(@"^ *\d+ *(/ *\d+ *)* *$");
            Regex regex = new Regex(@"\(?\+?\d+\)? *(\d+ *)* *(/ *\(?\+?\d+\)? *(\d+ *)* *)*");
            return regex.IsMatch(s.Trim());
        }

        public static bool IsInteger(this string s)
        {
            int intValue;
            return Int32.TryParse(s, out intValue);
        }

        public static bool ContainsAny(this string aString, string root, string pattern)
        {
            var patternToken = pattern.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (patternToken.Length > 0)
            {
                if (aString.ContainsAll(root))
                {
                    foreach (string s in patternToken)
                    {
                        if (aString.Contains(s)) return true;
                    }
                    return false;
                }
            }
            return aString.ContainsAll(root);
        }

        public static bool ContainsAny(this string aString, string pattern)
        {
            var patternToken = pattern.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (patternToken.Length > 0)
            {

                foreach (string s in patternToken)
                {
                    if (aString.Contains(s)) return true;
                }
                return false;

            }
            return false;
        }

        public static bool ContainsAll(this string aString, string pattern)
        {
            var patternToken = pattern.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (patternToken.Length > 0)
            {
                return patternToken.All(s => aString.Contains(s));
            }
            return aString.Contains(pattern);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aString"></param>
        /// <param name="pattern"> comma separated string</param>
        /// <returns></returns>
        public static bool StartsWithAny(this string aString, string pattern)
        {
            var patternToken = pattern.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (patternToken.Length > 0)
            {

                foreach (string token in patternToken)
                {
                    if (aString.StartsWith(token)) return true;
                }
                return false;

            }
            return false;
        }

        /// <summary>
        /// Truncates a string to a certain length
        /// </summary>
        /// <param name="valueToTruncate"></param>
        /// <param name="maxLength">Maximum length that the string should allow</param>
        /// <param name="options">Option flags to determine how to truncate the string
        ///		<para>&#160;&#160;&#160;&#160;<c>None</c>: Just cut off the word at the maximum length</para>
        ///		<para>&#160;&#160;&#160;&#160;<c>FinishWord</c>: Make sure that the string is not truncated in the middle of a word</para>
        ///		<para>&#160;&#160;&#160;&#160;<c>AllowLastWordToGoOverMaxLength</c>: If FinishWord is set, this allows the string to be longer than the maximum length if there is a word started and not finished before the maximum length</para>
        ///		<para>&#160;&#160;&#160;&#160;<c>IncludeEllipsis</c>: Include an ellipsis HTML character at the end of the truncated string.  This counts as one of the characters for the maximum length</para>
        /// </param>
        /// <returns>Truncated string</returns>
        public static string TruncateString(this string valueToTruncate, int maxLength, TruncateOptions options)
        {
            if (valueToTruncate == null || maxLength <= 0)
            {
                return "";
            }

            if (valueToTruncate.Length <= maxLength)
            {
                return valueToTruncate;
            }

            bool includeEllipsis = (options & TruncateOptions.IncludeEllipsis) == TruncateOptions.IncludeEllipsis;
            bool finishWord = (options & TruncateOptions.FinishWord) == TruncateOptions.FinishWord;
            bool allowLastWordOverflow = (options & TruncateOptions.AllowLastWordToGoOverMaxLength) == TruncateOptions.AllowLastWordToGoOverMaxLength;

            string retValue = valueToTruncate;

            if (includeEllipsis)
            {
                maxLength -= 1;
            }

            int lastSpaceIndex = retValue.LastIndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);

            if (!finishWord)
            {
                retValue = retValue.Remove(maxLength);
            }
            else if (allowLastWordOverflow)
            {
                int spaceIndex = retValue.IndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);
                if (spaceIndex != -1)
                {
                    retValue = retValue.Remove(spaceIndex);
                }
            }
            else if (lastSpaceIndex > -1)
            {
                retValue = retValue.Remove(lastSpaceIndex);
            }

            if (includeEllipsis && retValue.Length < valueToTruncate.Length)
            {
                retValue += "...";
                //retValue += "&hellip;";
            }
            return retValue;
        }
    }
}