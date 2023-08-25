using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevExtensions;
using ICSharpCode.SharpZipLib.BZip2;

public class WikipediaReader
{
    private readonly string _articleDumpPath;
    private readonly string _indexPath;

    private int artciclePerchunk = 1;// Int32.MaxValue;
    private bool savePage = false;
    private int maxChunkCount = Environment.ProcessorCount;
    private int numberOfCores = Environment.ProcessorCount;
    public WikipediaReader(string articleDumpPath, string indexPath,bool processAllChunk, int artciclePerchunk,bool savePage)
    {
        _articleDumpPath = articleDumpPath;
        _indexPath = indexPath;
        this.maxChunkCount = processAllChunk?Int32.MaxValue : Environment.ProcessorCount;
        this.artciclePerchunk = artciclePerchunk;
        this.savePage = savePage;
    }

    public Dictionary<long, (long size, List<(int articleId, string title)> articles)> GetChunkOffsetsAndSizes()
    {
        var offsets = new List<long>();
        var articlesPerOffset = new Dictionary<long, List<(int articleId, string title)>>();

        using var reader = new StreamReader(_indexPath);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(':');
            if (parts.Length >= 3)
            {
                var offset = long.Parse(parts[0]);
                var articleId = int.Parse(parts[1]);
                var title = parts[2];

                if (!articlesPerOffset.ContainsKey(offset))
                {
                    articlesPerOffset[offset] = new List<(int articleId, string title)>();
                    offsets.Add(offset);
                }

                articlesPerOffset[offset].Add((articleId, title));
            }
        }

        var chunkData = new Dictionary<long, (long size, List<(int articleId, string title)> articles)>();
        for (int i = 0; i < offsets.Count - 1; i++)
        {
            var size = offsets[i + 1] - offsets[i];
            chunkData[offsets[i]] = (size, articlesPerOffset[offsets[i]]);
        }

        // For the last chunk, use the size of the BZ2 file
        var fileInfo = new FileInfo(_articleDumpPath);
        var lastSize = fileInfo.Length - offsets[^1];
        chunkData[offsets[^1]] = (lastSize, articlesPerOffset[offsets[^1]]);

        return chunkData;
    }
    

    public bool FilterArticleCriteria((long offset, int articleId, string title) article)
    {
        // Implement your filtering criteria here
        return true;  // Placeholder: currently accepts all articles
    }

    public void ProcessArticle(string articleContent,string path)
    {
        try
        {
            var xml = XElement.Parse(articleContent);

            var titleElement = xml.Element("title");
            var idElement = xml.Element("id");
            var textElement = xml.Element("revision")?.Element("text");

            if (titleElement != null && textElement != null && idElement != null)
            {
                string title = titleElement.Value;
                string text = textElement.Value;
                string id = idElement.Value;
                var builder=new StringBuilder();
                builder.AppendLine(title);
                builder.AppendLine();
                builder.AppendLine(text);

                // Now, you have the title and text. You can process them as needed.
                // For demonstration purposes, let's just print them:
                Console.WriteLine($"Title: {title}");
                Console.WriteLine($"Text: {text}");

                var name = $"{title}-{id}.txt".ConvertToValidFileName();
                var fileName = Path.Combine(path, name);
                File.WriteAllText(fileName, builder.ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing article: {ex.Message}");
        }

       
    }

    public void ExtractAndProcessArticles(string path)
    {
        // var chunkData = GetChunkOffsetsAndSizes();
        var chunkData = GetChunkOffsetsAndSizes().Take(maxChunkCount).ToDictionary(k => k.Key, v => v.Value);

        // Extract and process all chunks in parallel
        Parallel.ForEach(chunkData, new ParallelOptions { MaxDegreeOfParallelism = numberOfCores }, chunkEntry =>
        {
            var offset = chunkEntry.Key;
            var chunkSize = chunkEntry.Value.size;
            var articles = chunkEntry.Value.articles;

            byte[] buffer;
            using (var fileStream = new FileStream(_articleDumpPath, FileMode.Open, FileAccess.Read))
            {
                fileStream.Seek(offset, SeekOrigin.Begin);
                buffer = new byte[chunkSize];
                fileStream.Read(buffer, 0, (int)chunkSize);
            }

            using (var memoryStream = new MemoryStream(buffer))
            using (var bz2Stream = new BZip2InputStream(memoryStream))
            using (var reader = new StreamReader(bz2Stream))
            {
                string line;
                StringBuilder builder = new StringBuilder();
                int pageCount = 0;

                while ((line = reader.ReadLine()) != null && pageCount <= artciclePerchunk)
                {
                    if (line.Trim().StartsWith("<page>"))
                    {
                        builder.Clear();
                        builder.AppendLine(line);

                        while ((line = reader.ReadLine()) != null && !line.Trim().StartsWith("</page>"))
                        {
                            builder.AppendLine(line);
                        }

                        // Ensure the closing tag is also appended
                        if (line != null && line.Trim().StartsWith("</page>"))
                        {
                            builder.AppendLine(line);
                        }

                        // At this point, `builder` contains the content of a single article.
                        // You can process it or store it as needed.
                        string articleContent = builder.ToString();
                        ProcessArticle(articleContent,path);

                        pageCount++;
                    }
                }
            }
        });
    }
}