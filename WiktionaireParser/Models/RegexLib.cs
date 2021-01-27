using System.Text.RegularExpressions;

namespace WiktionaireParser.Models
{
    public static class RegexLib
    {
        public static Regex regexLink = new Regex(@"\[\[(.*?)\]\]");
        public static Regex StartWithLangSectionRegex = new Regex(@"^==\s*{{langue\|fr}}\s*==");
        public static Regex ContainsLangSectionRegex = new Regex(@"==\s*{{langue\|fr}}\s*==");
        public static Regex OtherLangSectionRegex = new Regex(@"^==\s*{{langue");
    }
}