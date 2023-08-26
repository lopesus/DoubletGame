using System.Text.RegularExpressions;

namespace WiktionaireParser.Models;

using System;
using System.Text.RegularExpressions;

public static class TemplateChecker
{
    // Define the regex patterns for each group
    private static Regex languagePattern = new Regex(@"==\s*\{\{langue\|fr\}\}\s*==", RegexOptions.IgnoreCase);
    private static Regex adjectivePattern = new Regex(@"\{\{S\|adj\|fr\b|\{\{S\|(adjectif(exclamatif|indefini|possessif|demonstratif|interrogatif|numeral|relatif))\|fr\b|\{\{S\|adjectif\|fr\b", RegexOptions.IgnoreCase);
    private static Regex adverbPattern = new Regex(@"\{\{S\|(adv|adverbe)\|fr\b|\{\{S\|adverbe(interrogatif|relatif)\|fr\b", RegexOptions.IgnoreCase);
    private static Regex nounPattern = new Regex(@"\{\{S\|nom\|fr\b", RegexOptions.IgnoreCase);
    private static Regex nounCommonPattern = new Regex(@"\{\{S\|nom commun\|fr\b", RegexOptions.IgnoreCase);
    private static Regex nounProperPattern = new Regex(@"\{\{S\|nom propre\|fr\b", RegexOptions.IgnoreCase);
    private static Regex verbPattern = new Regex(@"\{\{S\|verbe\|fr\b", RegexOptions.IgnoreCase);
    private static Regex verbFlexionPattern = new Regex(@"\{\{S\|verbe\|fr\|flexion\b", RegexOptions.IgnoreCase);

    public static bool IsLanguage(string input)
    {
        return languagePattern.IsMatch(input);
    }

    public static bool IsAdjective(string input)
    {
        return adjectivePattern.IsMatch(input);
    }

    public static bool IsAdverb(string input)
    {
        return adverbPattern.IsMatch(input);
    }

    public static bool IsNoun(string input)
    {
        return nounPattern.IsMatch(input);
    }

    public static bool IsNounCommon(string input)
    {
        return nounCommonPattern.IsMatch(input);
    }

    public static bool IsNounProper(string input)
    {
        return nounProperPattern.IsMatch(input);
    }

    public static bool IsVerb(string input)
    {
        return verbPattern.IsMatch(input);
    }

    public static bool IsVerbFlexion(string input)
    {
        return verbFlexionPattern.IsMatch(input);
    }
}

class Program
{
    static void Main()
    {
        string content = "your_content_here";

        Console.WriteLine($"IsLanguage: {TemplateChecker.IsLanguage(content)}");
        Console.WriteLine($"IsAdjective: {TemplateChecker.IsAdjective(content)}");
        Console.WriteLine($"IsAdverb: {TemplateChecker.IsAdverb(content)}");
        Console.WriteLine($"IsNoun: {TemplateChecker.IsNoun(content)}");
        Console.WriteLine($"IsNounCommon: {TemplateChecker.IsNounCommon(content)}");
        Console.WriteLine($"IsNounProper: {TemplateChecker.IsNounProper(content)}");
        Console.WriteLine($"IsVerb: {TemplateChecker.IsVerb(content)}");
        Console.WriteLine($"IsVerbFlexion: {TemplateChecker.IsVerbFlexion(content)}");
    }
}
