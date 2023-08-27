// Represents a section of content for a specific language.

using System.Text;
using System.Text.RegularExpressions;

public class LanguageSection
{
    // Stores the lines of content for this language section.
    public List<string> Lines { get; set; } = new List<string>();

    public string GetContent()
    {
        var builder = new StringBuilder();

        foreach (var line in Lines)
        {
            builder.AppendLine(line);
        }
        return builder.ToString();
    }
}








