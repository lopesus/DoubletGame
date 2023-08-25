namespace DevExtensions;

// Usage:
// string validFileName = FileNameUtility.ConvertToValidFileName("CON:Article?");
public static class FileNameUtility
{
    private static readonly HashSet<string> ReservedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

    private static readonly char[] InvalidChars = new[] { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };

    public static string ConvertToValidFileName(this string title)
    {
        // Remove or replace invalid characters
        foreach (var c in InvalidChars)
        {
            title = title.Replace(c, '_'); // Replace invalid characters with underscore
        }

        // Check for reserved names
        if (ReservedNames.Contains(title))
        {
            title = "_" + title; // Prefix with underscore if it's a reserved name
        }

        // Check for length (you can adjust the max length as needed)
        const int maxLength = 250; // Considering some space for the directory path
        if (title.Length > maxLength)
        {
            title = title.Substring(0, maxLength);
        }

        return title;
    }
}