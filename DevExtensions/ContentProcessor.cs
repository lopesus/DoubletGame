using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;



public static class ContentProcessor
{
    // Regex pattern to identify the start of a French language section.
    private static Regex languagePattern = new Regex(@"^==\s*\{\{langue\|fr\}\}\s*==", RegexOptions.IgnoreCase);

    // Regex pattern to identify the start of any other language section.
    private static Regex StartWithOtherLangSectionRegex = new Regex(@"^==\s*\{\{langue\|(?!fr)..}}\s*==", RegexOptions.IgnoreCase);

    // Extracts all French language sections from the provided content.
    public static List<LanguageSection> ExtractLanguageSections(string content)
    {
        // Split the content into individual lines.
        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        // Flag to determine if we are currently storing lines for a language section.
        var isStoring = false;

        // List to store all extracted language sections.
        var languageSections = new List<LanguageSection>();

        // Represents the current language section being processed.
        LanguageSection currentSection = null;

        // Iterate through each line of the content.
        foreach (var line in lines)
        {
            // If the line matches the start of a French language section...
            if (languagePattern.IsMatch(line))
            {
                // If we were already processing a section, add it to the list.
                if (currentSection != null)
                {
                    languageSections.Add(currentSection);
                }

                // Start a new language section.
                currentSection = new LanguageSection();
                isStoring = true;
            }

            // If the line matches the start of another language section...
            if (isStoring && StartWithOtherLangSectionRegex.IsMatch(line))
            {
                // Finish the current section and add it to the list.
                if (currentSection != null)
                {
                    languageSections.Add(currentSection);
                }
                currentSection = null;
                isStoring = false;
            }

            // If we are within a language section, store the line.
            if (isStoring && currentSection != null)
            {
                currentSection.Lines.Add(line);
            }
        }

        // If we were processing a section at the end of the content, add it to the list.
        if (currentSection != null)
        {
            languageSections.Add(currentSection);
        }

        return languageSections;
    }

    public static WordInfo ExtractWordInfo(string content)
    {
        WordInfo wordInfo = new WordInfo();

        // Extract word
        var wordMatch = Regex.Match(content, @"^'''(.*?)'''");
        if (wordMatch.Success)
        {
            wordInfo.Word = wordMatch.Groups[1].Value;
        }

        // Extract pronunciation
        var pronMatch = Regex.Match(content, @"{{pron\|(.*?)\|fr}}");
        if (pronMatch.Success)
        {
            wordInfo.Pronunciation = pronMatch.Groups[1].Value;
        }

        // Extract gender and number
        var genderMatch = Regex.Match(content, @"{{(mf|f|m|invar)}}");
        if (genderMatch.Success)
        {
            wordInfo.GenderAndNumber = genderMatch.Groups[1].Value;
        }

        // Extract variability
        var variabilityMatch = Regex.Match(content, @"\({{invar}}\) ou, moins fréquemment, \({{p}} '''\[\[bios#fr\|bios\]\]'''");
        if (variabilityMatch.Success)
        {
            wordInfo.Variability = "Invariable or bios";
        }

        // Extract definitions
        foreach (Match match in Regex.Matches(content, @"#\s*(.*?)\.\n"))
        {
            wordInfo.Definitions.Add(match.Groups[1].Value);
        }

        return wordInfo;
    }

}
