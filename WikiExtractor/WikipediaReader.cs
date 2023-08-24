using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;

public class WikipediaReader
{
    private readonly string _articleDumpPath;
    private readonly string _indexPath;

    public WikipediaReader(string articleDumpPath, string indexPath)
    {
        _articleDumpPath = articleDumpPath;
        _indexPath = indexPath;
    }

    public long GetOffsetForArticle(string title)
    {
        using var reader = new StreamReader(_indexPath);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(':');
            if (parts.Length >= 3 && parts[2] == title)
            {
                return long.Parse(parts[0]);
            }
        }
        throw new Exception($"Article {title} not found in index.");
    }

    public string ExtractArticleContent(string title, int length = 10 * 1024 * 1024)  // default length is 10MB
    {
        var offset = GetOffsetForArticle(title);

        byte[] buffer;
        using (var fileStream = new FileStream(_articleDumpPath, FileMode.Open, FileAccess.Read))
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
            buffer = new byte[length];
            fileStream.Read(buffer, 0, length);
        }

        using var memoryStream = new MemoryStream(buffer);
        using var bz2Stream = new BZip2InputStream(memoryStream);
        using var reader = new StreamReader(bz2Stream);
        var decompressedContent = reader.ReadToEnd();

        var startIdx = decompressedContent.IndexOf($"<title>{title}</title>");
        if (startIdx == -1)
        {
            throw new Exception($"Article {title} not found in the extracted data.");
        }

        var endIdx = decompressedContent.IndexOf("</page>", startIdx);
        if (endIdx == -1)
        {
            throw new Exception($"End of article {title} not found in the extracted data.");
        }

        return decompressedContent.Substring(startIdx, endIdx + "</page>".Length - startIdx);
    }
}