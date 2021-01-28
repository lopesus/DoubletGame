using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CommonLibTools.Extensions
{
    public static class DataExtraction
    {
        public static List<string> ExtractEmails(string htmlText)
        {
            Regex r = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}", RegexOptions.IgnoreCase);
            Match m;
            var results = new List<string>();
            for (m = r.Match(htmlText); m.Success; m = m.NextMatch())
            {
                if (!(results.Contains(m.Value)))
                    results.Add(m.Value);
            }
            return results;
        }

        public static List<string> ExtractPhoneNumber(string htmlText)
        {
            Match m;
            Regex regex = new Regex(@"\(?\+?\d+\)? *(\d+ *)* *(/ *\(?\+?\d+\)? *(\d+ *)* *)*");
            var results = new List<string>();
            for (m = regex.Match(htmlText); m.Success; m = m.NextMatch())
            {
                if (!(results.Contains(m.Value)))
                {
                    string value = m.Value;
                    var count = value.CountCharNumber('0');
                    if (value.Length >= 8 && count < 5)
                    {
                        results.Add(value);
                    }
                }
            }
            return results;
        }
    }
}