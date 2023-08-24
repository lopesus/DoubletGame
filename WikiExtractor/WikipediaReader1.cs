using System;
using System.IO;
using ICSharpCode.SharpZipLib.BZip2;

public class WikipediaReader1
{
    private readonly string _articleDumpPath;
    private readonly string _indexPath;

    public WikipediaReader1(string articleDumpPath, string indexPath)
    {
        _articleDumpPath = articleDumpPath;
        _indexPath = indexPath;
    }

    public string ReadArticleAtOffset(long offset)
    {
        using var fileStream = new FileStream(_articleDumpPath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(offset, SeekOrigin.Begin);

        using var bz2Stream = new BZip2InputStream(fileStream);
        using var reader = new StreamReader(bz2Stream);
        return reader.ReadToEnd();
    }

    public long GetOffsetForArticle(string title)
    {
        using var reader = new StreamReader(_indexPath);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            // Adjusted format: "FileOffset:ArticleID:Title"
            var parts = line.Split(':');
            if (parts.Length >= 3 && parts[2] == title)
            {
                return long.Parse(parts[0]);
            }
        }
        throw new Exception($"Article {title} not found in index.");
    }

    public string GetArticle(string title)
    {
        var offset = GetOffsetForArticle(title);
        var article = ReadArticleAtOffset(offset);
        return article;
    }
}