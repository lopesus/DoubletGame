using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.BZip2;

public class WikipediaReaderSingle
{
    private readonly string _articleDumpPath;
    private readonly string _indexPath;

    public WikipediaReaderSingle(string articleDumpPath, string indexPath)
    {
        _articleDumpPath = articleDumpPath;
        _indexPath = indexPath;
    }

    public Dictionary<long, long> GetChunkOffsetsAndSizes()
    {
        var offsets = new List<long>();
        using var reader = new StreamReader(_indexPath);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(':');
            if (parts.Length >= 3)
            {
                offsets.Add(long.Parse(parts[0]));
            }
        }

        var dict = new Dictionary<long, long>();
        for (int i = 0; i < offsets.Count - 1; i++)
        {
            var size = offsets[i + 1] - offsets[i];
            dict[offsets[i]] = size;
        }

        // For the last chunk, use the size of the BZ2 file
        var fileInfo = new FileInfo(_articleDumpPath);
        var lastSize = fileInfo.Length - offsets[^1];
        dict[offsets[^1]] = lastSize;

        return dict;
    }


    public string ExtractArticleContent(string title)
    {
        var chunkData = GetChunkOffsetsAndSizes();
        long offset = GetOffsetForArticle(title);

        if (!chunkData.ContainsKey(offset))
        {
            throw new Exception($"Offset for article {title} not found in chunk data.");
        }

        var length = chunkData[offset];

        byte[] buffer;
        using (var fileStream = new FileStream(_articleDumpPath, FileMode.Open, FileAccess.Read))
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
            buffer = new byte[length];
            fileStream.Read(buffer, 0, (int)length);
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
}