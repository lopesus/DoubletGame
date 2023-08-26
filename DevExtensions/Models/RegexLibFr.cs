using System.Text.RegularExpressions;

namespace WiktionaireParser.Models
{
    public static class RegexLibFr
    {
        public static Regex regexLink = new Regex(@"\[\[(.*?)\]\]", RegexOptions.IgnoreCase);
        public static Regex StartWithLangSectionRegex = new Regex(@"^==\s*{{langue\|fr}}\s*==", RegexOptions.IgnoreCase);
        public static Regex StartWithOtherLangSectionRegex = new Regex(@"^==\s*{{langue\|(?!fr)..}}\s*==", RegexOptions.IgnoreCase);
        public static Regex ContainsLangSectionRegex = new Regex(@"==\s*{{langue\|fr}}\s*==", RegexOptions.IgnoreCase);
        //public static Regex OtherLangSectionRegex = new Regex(@"^==\s*{{langue");
        public static Regex SectionRegex = new Regex(@"^={2,}", RegexOptions.IgnoreCase);

        // verbes
        public static Regex VerbFlexionRegex = new Regex(@"\{\{S\|verbe\|fr\|flexion\b", RegexOptions.IgnoreCase);
        public static Regex VerbRegex = new Regex(@"\{\{S\|verbe\|fr\b", RegexOptions.IgnoreCase);
        public static Regex VerbRegex2 = new Regex(@"\{\{S\|verbe\|num=[1-9]?\|fr\b", RegexOptions.IgnoreCase);

        //nom 
        public static Regex NomCommunRegex = new Regex(@"\{\{S\|nom\|fr\b", RegexOptions.IgnoreCase);
        public static Regex NomCommunRegex2 = new Regex(@"\{\{S\|nom commun\|fr\b", RegexOptions.IgnoreCase);
        public static Regex NomCommunRegex3 = new Regex(@"\{\{S\|nom\|num=[1-9]\|fr\b", RegexOptions.IgnoreCase);

        //adjectif
        public static Regex AdjectifRegex = new Regex(@"\{\{S\|adj\|fr\b");
        public static Regex AdjectifRegex2 = new Regex(@"\{\{S\|(adjectif(exclamatif|indefini|possessif|demonstratif|interrogatif|numeral))\|fr\b");
        public static Regex AdjectifRegex3 = new Regex(@"\{\{S\|adjectif\|fr\}");

        //Adverbe
        public static Regex AdverbeRegex = new Regex(@"\{\{S\|(adv|adverbe)\|fr\b");
        public static Regex AdverbeRegex2 = new Regex(@"\{\{S\|adverbe(interrogatif|relatif)\|fr\b");

        //nom
        //public static Regex NomRegex = new Regex(@"\{\{S\|nom\|fr\b");

        public static Regex PronomRegex = new Regex(@"\{\{S\|pronom\|fr\b");
        public static Regex PronomRegex2 = new Regex(@"\{\{S\|(pronom(demonstratif|indefini|interrogatif|personnel|possessif|relatif))\|fr\b");
        //public static Regex NomCommunRegex = new Regex(@"");
        //public static Regex NomCommunRegex = new Regex(@"");
        //public static Regex NomCommunRegex = new Regex(@"");

    }
}