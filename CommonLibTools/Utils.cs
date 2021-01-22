using System;

namespace CommonLibTools
{
    public static class Utils

    {
        public static bool IsEmptyString(this string s)
        {
            return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
        }
    }
}
