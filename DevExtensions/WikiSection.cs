using System;
using System.Collections.Generic;

public class WikiSection
{
    // Represents the heading/title of the section
    public string Heading { get; set; }

    // Represents the content of the section (excluding sub-sections)
    public string Content { get; set; } = "";

    // List of sub-sections within this section
    public List<WikiSection> SubSections { get; set; } = new List<WikiSection>();

    public override string ToString()
    {
        return $"Heading: {Heading} - SubSections {SubSections.Count}";
    }
}