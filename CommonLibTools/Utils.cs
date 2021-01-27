using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CommonLibTools
{
    public static class Utils

    {

        public static string SortString(this string input)
        {
            char[] characters = input.ToCharArray();
            Array.Sort(characters);
            return new string(characters);
        }

        public static bool IsEmptyString(this string s)
        {
            return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
        }

        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
