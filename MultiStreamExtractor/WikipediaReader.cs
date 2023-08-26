using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;
using CommonLibTools.Libs.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using DevExtensions;
using ICSharpCode.SharpZipLib.BZip2;
using MultiStreamExtractor;
using WiktionaireParser.Models;

public class WikipediaReader
{
    private readonly string _articleDumpPath;
    private readonly string _indexPath;

    private int artciclePerchunk = 1;// Int32.MaxValue;
    private bool savePage = false;
    private int maxChunkCount = Environment.ProcessorCount;
    private int numberOfCores = 1;// Environment.ProcessorCount;
    Dictionary<string, bool> officiaScrabbleWordList = new Dictionary<string, bool>();
    SectionBuilder sectionBuilder = new SectionBuilder();

    public List<WikiPage> PagesList = new List<WikiPage>();
    bool checkInValidWordList = false;
    bool saveToDb = false;

    private MultiStreamInfos multiStreamInfos;

    public WikipediaReader(MultiStreamInfos multiStreamInfos, string articleDumpPath,
        string indexPath,
        Dictionary<string, bool> officiaScrabbleWordList,
        bool processAllChunk,
        int numberOfCores,
        int artciclePerchunk,
        bool savePage,
        bool checkInValidWordList,
        bool saveToDb)
    {
        _articleDumpPath = articleDumpPath;
        _indexPath = indexPath;
        this.officiaScrabbleWordList = officiaScrabbleWordList;
        this.maxChunkCount = processAllChunk ? Int32.MaxValue : numberOfCores;
        this.artciclePerchunk = artciclePerchunk;
        this.savePage = savePage;
        this.numberOfCores = numberOfCores;
        this.checkInValidWordList = checkInValidWordList;
        this.saveToDb = saveToDb;
        this.multiStreamInfos = multiStreamInfos;

        //ensure folder exist
        var path = multiStreamInfos.GetRawPageFolder();
        Directory.CreateDirectory(path);
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

    public async void ProcessArticle(string articleContent)
    {
        var rawPageFolder = multiStreamInfos.GetRawPageFolder();
        try
        {
            var xml = XElement.Parse(articleContent);

            var titleElement = xml.Element("title");
            var idElement = xml.Element("id");
            var textElement = xml.Element("revision")?.Element("text");

            if (titleElement != null && textElement != null && idElement != null)
            {
                string title = titleElement.Value.Trim();
                string text = textElement.Value.Trim();
                string id = idElement.Value.Trim();
                var builder = new StringBuilder();
                builder.AppendLine(title);
                builder.AppendLine();
                builder.AppendLine(text);

                var titleInv = title.ToLowerInvariant().RemoveDiacritics();
                if (checkInValidWordList == false || (checkInValidWordList && officiaScrabbleWordList.ContainsKey(titleInv)))
                {
                    var wikiPage = new WikiPage(id, title, text, sectionBuilder);
                    PagesList.Add(wikiPage);

                    //if (savePage)
                    //{
                    //    var name = $"{title}-{id}.txt".ConvertToValidFileName();
                    //    var name2 = $"{title}-{id}_full.txt".ConvertToValidFileName();
                    //    var fileName = Path.Combine(rawPageFolder, name);
                    //    var fileName2 = Path.Combine(rawPageFolder, $"{name2}");

                    //    //await File.WriteAllTextAsync(fileName, builder.ToString());
                    //    //await File.WriteAllTextAsync(fileName2, articleContent); 
                    //    File.WriteAllText(fileName, builder.ToString());
                    //    File.WriteAllText(fileName2, articleContent);
                    //}
                }



            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing article: {ex.Message}");
        }


    }

    public async Task<bool> ExtractAndProcessArticles()
    {
        await Task.Run(() =>
        {

            // var chunkData = GetChunkOffsetsAndSizes();
            var chunkData = GetChunkOffsetsAndSizes().Take(maxChunkCount).ToDictionary(k => k.Key, v => v.Value);

            long count = 0;
            long totalChunk = chunkData.Count;
            // Extract and process all chunks in parallel
            Parallel.ForEach(chunkData, new ParallelOptions { MaxDegreeOfParallelism = numberOfCores }, chunkEntry =>
            {
                //await Task.Delay(500);
                Interlocked.Increment(ref count);
                //Debug.WriteLine($"processing chunk {count}/{totalChunk}");
                WeakReferenceMessenger.Default.Send(new UpdateUIMessage { CurrentChunk = count, TotalChunk = totalChunk });

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
                            ProcessArticle(articleContent);

                            pageCount++;
                        }
                    }
                }
            });

            if (saveToDb)
            {
                WikiPageDataExtractor.SaveToDb(PagesList);
            }

            if (savePage)
            {
                var rawPageFolder = multiStreamInfos.GetRawPageFolder();
                foreach (var wikiPage in PagesList)
                {
                    string title = wikiPage.Title.Trim();
                    string text = wikiPage.Text.Trim();
                    string id = wikiPage.ArticleId.Trim();
                    var builder = new StringBuilder();
                    builder.AppendLine(title);
                    builder.AppendLine();
                    builder.AppendLine(text);

                    var name = $"{title}-{id}.txt".ConvertToValidFileName();
                    var name2 = $"{title}-{id}_full.txt".ConvertToValidFileName();
                    var fileName = Path.Combine(rawPageFolder, name);
                    //var fileName2 = Path.Combine(rawPageFolder, $"{name2}");

                    File.WriteAllText(fileName, builder.ToString());
                }
            }

            string path = multiStreamInfos.GetExtractionFolder();
            var sectionList = new HashSet<string>(sectionBuilder.Sections).ToList();
            sectionList.Sort();
            File.WriteAllLines($"{path}/___sectionList.txt", sectionList);

            var languageFilter = "fr";
            sectionList = sectionBuilder.GetFilterdSections(languageFilter);
            File.WriteAllLines($"{path}/___sectionList_filtered_{languageFilter}.txt", sectionList);

            sectionList = new HashSet<string>(sectionBuilder.SectionsWithNoSpace).ToList();
            sectionList.Sort();
            File.WriteAllLines($"{path}/___SectionsWithNoSpaceList.txt", sectionList);
        });

        return true;
    }



}