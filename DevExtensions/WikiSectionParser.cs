using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;



public static class WikiSectionParser
{
    public static List<WikiSection> ParseWikiText(List<string> lines, int start, int end, int level)
    {
        // Initialize the list to hold the sections
        var sections = new List<WikiSection>();
        WikiSection currentSection = null;

        // Define Regex patterns for different levels of headings
        var regex2 = new Regex(@"^==[^=].*");
        var regex3 = new Regex(@"^===[^=].*");
        var regex4 = new Regex(@"^====[^=].*");
        var regex5 = new Regex(@"^=====[^=].*");

        // Loop through the lines of text
        for (int i = start; i < end; i++)
        {
            string line = lines[i];

            // Determine which Regex pattern to use based on the current level
            Regex currentRegex = level switch
            {
                2 => regex2,
                3 => regex3,
                4 => regex4,
                5 => regex5,
                _ => null
            };

            // Check if the line matches the current level's Regex pattern
            if (currentRegex.IsMatch(line))
            {
                // If a section is already being processed, add it to the list
                if (currentSection != null)
                {
                    sections.Add(currentSection);
                }

                // Start a new section
                currentSection = new WikiSection { Heading = line };
            }
            else if (currentSection != null)
            {
                // Check if the line starts a new section or subsection
                if (line.StartsWith("=="))
                {
                    // Determine the level of the new subsection
                    int nextLevel = line.TakeWhile(c => c == '=').Count();

                    // Find the end index of the subsection
                    int subEnd = i + 1;
                    while (subEnd < end && !lines[subEnd].StartsWith("==", StringComparison.Ordinal))
                    {
                        subEnd++;
                    }

                    // Recursively parse the subsection and add it to the current section
                    currentSection.SubSections.AddRange(ParseWikiText(lines, i, subEnd, nextLevel));

                    // Skip the lines that were part of the subsection
                    i = subEnd - 1;
                }
                else
                {
                    // Add the line to the current section's content
                    currentSection.Content += line + "\n";
                }
            }
        }

        // Add the last section if it exists
        if (currentSection != null)
        {
            sections.Add(currentSection);
        }

        return sections;
    }
}
